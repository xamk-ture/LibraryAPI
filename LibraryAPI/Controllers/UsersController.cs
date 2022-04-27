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

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public UsersController(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Users
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return _mapper.Map<UserDto[]>(_context.Users.Include(x => x.Address));
        }

        // GET: api/Users/5
        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _context.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<UserDto>(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("EditUser/{id}")]
        public async Task<IActionResult> PutUser(string id, UserDtoIn user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                var userEntity = await _context.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == user.Id);

                user.Address.Id = userEntity.Address.Id;

                userEntity = _mapper.Map(user, userEntity);


                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("BanUser")]
        [AuthorizationAttribute]
        public async Task<IActionResult> ChangeUserRights(string id, bool isBanned)
        {

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return BadRequest();
            }

            user.IsBanned = isBanned;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> PostUser(UserDtoIn user)
        {
            var entityUser = _mapper.Map<User>(user);

            entityUser.Id = Guid.NewGuid().ToString();
            entityUser.Address.Id = Guid.NewGuid().ToString();

            _context.Users.Add(entityUser);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = entityUser.Id }, _mapper.Map<UserDtoIn>(entityUser));
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
