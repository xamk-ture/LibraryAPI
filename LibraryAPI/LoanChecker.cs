using LibraryAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/*
    Skripti käy läpi lainatut kirjat jokapäivä klo 8utc ja lähettää asiakkaalle kootun listan lainatuista kirjoista ja tiedon jäljellä olevasta palautusajasta,
    jos yhdenkään kirjan palautusaikaa on jäljellä <= 7 päivää tai se on myöhässä (vastoin ohjeistusta, mutta tuntui järkevämmältä).

    Mitä vois parantaa?
    - Tietokantoihin vois logittaa aina kun toimenpide on suoritettu, nyt mahdollista että viestit lähtee useempaan kertaan jos järjestelmä kaatuu ja käynnistyy uudestaan
    
*/

namespace LibraryAPI
{

    public class LoanChecker : IHostedService
    {
        private Timer _timer;
        DateTime lastDate;

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly int timeToCheckLoans = 8;

        public LoanChecker(IServiceScopeFactory _scopeFactory)
        {
            this._scopeFactory = _scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckLoans, null, 0, 60000);
            return Task.CompletedTask;
        }

        void CheckLoans(object state)
        {
            var dateTimeNow = DateTime.UtcNow;
            var dateNow = DateTime.Now.Date;

            if (dateTimeNow.Hour == timeToCheckLoans && lastDate != dateNow)
            {
                lastDate = dateNow;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                    var lendings = _context.Lendings.ToArray();
                    IDictionary<string, List<Lending>> lendingData = GenerateDictionaryFromLendings(lendings);



                    foreach (KeyValuePair<string, List<Lending>> userLendings in lendingData)
                    {
                        var userLendingsSorted = SortLendings(userLendings.Value, dateTimeNow);

                        if (AnyBookIsAboutToExpireOrBookIsLate(userLendingsSorted, dateTimeNow))
                        {
                            var user = _context.Users.FindAsync(userLendings.Key);

                            var message = $"Hei {user.Result.Firstname} {user.Result.Lastname}\nTässä lista lainatuista kirjoistasi\n";

                            for (var i = 0; i < userLendingsSorted.Count; i++)
                            {
                                var bookName = _context.Books.FindAsync(userLendingsSorted[i].BookId).Result.Name;
                                var timeLeft = (int)(LibrarySettings.MaxLendingTimeInDays - (dateTimeNow - userLendingsSorted[i].CreatedAt).TotalDays);

                                if (timeLeft > 0)
                                {
                                    message += $"{bookName}\nPalautusaikaa jäljellä {timeLeft} päivää\n";
                                }
                                else
                                {
                                    message += $"{bookName}\nKirja on myöhässä {((timeLeft < 0) ? timeLeft * -1 : timeLeft)} päivää\n";
                                }
                            }

                            SendMessage(message);
                        }

                    }

                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private static void SendMessage(string message)
        {
            Debug.WriteLine(message);
        }

        private static bool AnyBookIsAboutToExpireOrBookIsLate(List<Lending> lendings, DateTime dateTimeNow)
        {
            foreach (Lending lending in lendings)
            {
                if ((int) (LibrarySettings.MaxLendingTimeInDays - (dateTimeNow - lending.CreatedAt).TotalDays) <= 7)
                {
                    return true;
                }
            }

            return false;
        }
        private static List<Lending> SortLendings(List<Lending> lendings, DateTime dateTimeNow)
        {
           lendings.Sort(delegate (Lending x, Lending y)
            {
                return ((dateTimeNow - x.CreatedAt).TotalMinutes).CompareTo((dateTimeNow - y.CreatedAt).TotalMinutes);
            });

            return lendings;
        }

        private static IDictionary<string, List<Lending>> GenerateDictionaryFromLendings(Lending[] lendings)
        {
            IDictionary<string, List<Lending>> lendingData = new Dictionary<string, List<Lending>>();

            foreach (Lending lending in lendings)
            {
                if (!lendingData.ContainsKey(lending.UserId))
                {
                    lendingData.Add(lending.UserId, new List<Lending>());
                }

                lendingData[lending.UserId].Add(lending);
            }

            return lendingData;
        }
    }
}