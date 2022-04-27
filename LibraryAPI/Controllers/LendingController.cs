using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using LibraryAPI.DataTransferObjects.Incoming;
using LibraryAPI.DataTransferObjects.Outgoing;
using AutoMapper;
using System.Diagnostics;
using LibraryAPI;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LendingController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public LendingController(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Lendings/5
        [HttpGet("GetLending/{id}")]
        public async Task<ActionResult<LendingDto>> GetLendings(string id)
        {
            var lendings = await _context.Lendings.FindAsync(id);

            if (lendings == null)
            {
                return NotFound();
            }

            return _mapper.Map<LendingDto>(lendings);
        }

        // GET: api/Lendings/5
        [HttpGet("GetUserLendings/{userId}")]
        public async Task<ActionResult<LendingDto[]>> GetUserLendings(string userId)
        {
            var lendings = await _context.Lendings.Where(x => x.UserId == userId).Include(x => x.Book).AsSplitQuery().ToListAsync();;

            if (lendings == null)
            {
                return NotFound();
            }

            return _mapper.Map<LendingDto[]>(lendings);
        }

        // POST: api/Lendings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("LendBook")]
        public async Task<ActionResult<LendingDtoIn>> PostLendings(LendingDtoIn lending)
        {


            //Validate
            User user = _context.Users.Include(x => x.Lendings)
                .FirstOrDefault(x => x.Id == lending.UserId);

            Book book = _context.Books.Include(x => x.Lendings)
                .FirstOrDefault(x => x.Id == lending.BookId);

            if (user == null || book == null)
                return Conflict(new { message = "Book or user does not exist" });

            if (user.IsBanned)
                return Conflict(new { message = "User is banned" });

            if (book.Lendings.Count  >= book.Amount)
                return Conflict(new { message = "No books in stock" });

            if (user.Lendings.Any(x => x.BookId == book.Id))
                return Conflict(new { message = "User has already borrowed this book" });

            var now = DateTime.UtcNow;
            if (user.Lendings.Any(x => (now - x.CreatedAt).TotalDays > LibrarySettings.MaxLendingTimeInDays))
                return Conflict(new { message = "The user has not returned a book that return time has expired" });

            Lending newLending = _mapper.Map<Lending>(lending);
            newLending.Id = Guid.NewGuid().ToString();
            _context.Lendings.Add(newLending);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LendingsExists(lending.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLendings", new { id = lending.Id }, lending);
        }

        // DELETE: api/Lendings/5
        [HttpDelete("ReturnBook/{id}")]
        public async Task<IActionResult> ReturnBook(string id, string userId)
        {
            var lending = await _context.Lendings.FirstOrDefaultAsync(x => x.Id == id);

            if (lending == null)
            {
                return NotFound();
            }

            if (lending.UserId != userId)
            {
                return NotFound();
            }
            

            _context.Lendings.Remove(lending);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LendingsExists(string id)
        {
            return _context.Lendings.Any(e => e.Id == id);
        }
    }
}
