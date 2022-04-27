using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LibraryAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace LibraryAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var _libraryContext = services.GetRequiredService<LibraryContext>();

                _libraryContext.Database.EnsureCreated();

                CreateUsers(_libraryContext);
                CreateBooks(_libraryContext);
                _libraryContext.SaveChanges();
                host.Run();
            }
        }
 

        static void CreateUsers(LibraryContext _libraryContext)
        {
            Random random = new Random();
            var jsonFile = File.ReadAllText("Files/BookData.json");
            var data = JObject.Parse(jsonFile);
            var dataNames = (JArray)data["names"];

            for (var i = 0; i < dataNames.Count; i++)
            {
                string[] name = dataNames[i].ToString().Split(' ');

                if (!_libraryContext.Users.Any(x => x.Firstname == name[0] && x.Lastname == name[1]))
                {

                    var user = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Firstname = name[0],
                        Lastname = name[1],
                        Email = name[0] + name[1] + "@lb.com",
                        PhoneNumber = "123456789",
                        IsBanned = false,
                        IsEmployee = false,
                        Address = new Address()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Postcode = random.Next(10000, 99999).ToString(),
                            City = "Kaupunki",
                            Country = "Maa",
                            Street = "Jokukatu 12"

                        }
                    };

                    _libraryContext.Users.Add(user);
                }
            }

        }

        static void CreateBooks(LibraryContext _libraryContext)
        {
            var jsonFile = File.ReadAllText("Files/BookData.json");
            var data = JObject.Parse(jsonFile);
            var dataTitles = (JArray)data["titles"];
            var dataDescriptions = (JArray)data["descriptions"];
            var dataLanguages = (JArray)data["languages"];
            var dataPublishers = (JArray)data["publishers"];
            var dataPrinters = (JArray)data["printers"];
            var dataSubjects = (JArray)data["subjects"];
            var dataNames = (JArray)data["names"];
            var dataProducers = (JArray)data["publishers"];

            Random random = new Random();

            foreach (var dataTitle in dataTitles)
            {
                var title = dataTitle.ToString();

                if (!_libraryContext.Books.Any(x => x.Name == title))
                {
                    List<Author> authors = new List<Author>(CreateAuthors(_libraryContext, dataNames));
                    List<Subject> subjects = new List<Subject>(CreateSubjects(_libraryContext, dataSubjects));
                    Language newLanguage = SelectLanguage(_libraryContext, dataLanguages);
                    Language newOrginalLanguage = SelectLanguage(_libraryContext, dataLanguages);
                    Publisher newPublisher = SelectPublisher(_libraryContext, dataPublishers);
                    Producer newProducer = SelectProducer(_libraryContext, dataProducers);

                    var newBook = new Book()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = title,
                        Description = dataDescriptions[random.Next(0, dataDescriptions.Count)].ToString(),
                        Year = random.Next(1900, 2021),
                        Authors = authors,
                        Subjects = subjects,
                        Language = newLanguage,
                        OrginalLanguage = newOrginalLanguage,
                        Publisher = newPublisher,
                        Producer = newProducer,
                        ISBN = random.Next(100000000, 999999999).ToString(),
                        AdditionalInformation = dataDescriptions[random.Next(0, dataDescriptions.Count)].ToString(),
                        Amount = random.Next(1, 5)
                    };

                    _libraryContext.Books.Add(newBook);
                    _libraryContext.SaveChanges();
                }
            }
        }

        static Language SelectLanguage(LibraryContext _libraryContext, JArray languages)
        {
            Random random = new Random();
            string newLanguageName = languages[random.Next(0, languages.Count)].ToString();
            Language newLanguage = _libraryContext.Languages
                                            .Where(x => x.Name == newLanguageName)
                                            .FirstOrDefault();
            if (newLanguage == null)
            {
                newLanguage = new Language()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = newLanguageName
                };

                _libraryContext.Languages.Add(newLanguage);
            }
            return newLanguage;
        }

        static Publisher SelectPublisher(LibraryContext _libraryContext, JArray publishers)
        {
            Random random = new Random();
            string newPublisherName = publishers[random.Next(0, publishers.Count)].ToString();
            Publisher newPublisher = _libraryContext.Publishers
                                            .Where(x => x.Name == newPublisherName)
                                            .FirstOrDefault();
            if (newPublisher == null)
            {
                newPublisher = new Publisher()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = newPublisherName
                };

                _libraryContext.Publishers.Add(newPublisher);
            }

            return newPublisher;
        }
        static Producer SelectProducer(LibraryContext _libraryContext, JArray producers)
        {
            Random random = new Random();
            string newProducerName = producers[random.Next(0, producers.Count)].ToString();
            Producer newProducer = _libraryContext.Producers
                                            .Where(x => x.Name == newProducerName)
                                            .FirstOrDefault();
            if (newProducer == null)
            {
                newProducer = new Producer()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = newProducerName
                };

                _libraryContext.Producers.Add(newProducer);
            }

            return newProducer;
        }

        static List<Subject> CreateSubjects(LibraryContext _libraryContext, JArray subjects)
        {

            var bookSubjects = new List<Subject>();

            Random random = new Random();

            int subjectCount = random.Next(1, 3);

            for (var i=0; i < subjectCount; i++)
            {
                int randomSubject = random.Next(0, subjects.Count);
                string randomSubjectName = subjects[randomSubject].ToString();

                Subject newSubject = null;

                foreach (var currentSubject in _libraryContext.Subjects)
                {
                    if (currentSubject.Name == randomSubjectName)
                    {
                        newSubject = currentSubject;
                        break;
                    }
                }

                if (newSubject == null)
                {
                    newSubject = new Subject()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = randomSubjectName
                    };

                    _libraryContext.Subjects.Add(newSubject);
                }

                bookSubjects.Add(newSubject);
            }

            return bookSubjects;
        }

        static List<Author> CreateAuthors(LibraryContext _libraryContext, JArray authors)
        {
            var bookAuthors = new List<Author>();

            Random random = new Random();

            int authorCount = random.Next(1, 3);

            for (var i = 0; i < authorCount; i++)
            {
                int randomAuthor = random.Next(0, authors.Count);
                string randomAuthorName = authors[randomAuthor].ToString();

                Author newAuthor = null;

                foreach (var currentAuthor in _libraryContext.Authors)
                {
                    if (currentAuthor.Name == randomAuthorName)
                    {
                        newAuthor = currentAuthor;
                        break;
                    }
                }

                if (newAuthor == null)
                {
                    newAuthor = new Author()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = randomAuthorName
                    };

                    _libraryContext.Authors.Add(newAuthor);
                }

                bookAuthors.Add(newAuthor);
            }

            return bookAuthors;
        }

    

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
