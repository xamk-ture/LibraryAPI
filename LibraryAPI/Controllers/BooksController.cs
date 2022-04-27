using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using System.Diagnostics;
using LibraryAPI.DataTransferObjects.Outgoing;
using AutoMapper;
using LibraryAPI.DataTransferObjects.Incoming;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public BooksController(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetBook/{id}")]
        public async Task<ActionResult<BookDto>> GetBook(string id)
        {
            var book = await _context.Books.Include(x => x.Authors)
                                            .Include(x => x.Subjects)
                                            .Include(x => x.Language)
                                            .Include(x => x.OrginalLanguage)
                                            .Include(x => x.Producer)
                                            .Include(x => x.Publisher).SingleOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return _mapper.Map<BookDto>(book);
        }

        [HttpGet("GetBooks")]
        public async Task<ActionResult<IEnumerable<BookDto>>> FindBooks([FromQuery] string name, [FromQuery] string subject, [FromQuery] string publisher, [FromQuery] string producer,
                                                                        [FromQuery] string author, [FromQuery] string year, [FromQuery] string language)
        {

            string[] names = name != null ? name.Split(',') : null;
            string[] years = year != null ? year.Split(',') : null;
            string[] languages = language != null ? language.Split(',') : null;
            string[] subjects = subject != null ? subject.Split(',') : null;
            string[] authors = author != null ? author.Split(',') : null;
            string[] publishers = publisher != null ? publisher.Split(',') : null;
            string[] producers = producer != null ? producer.Split(',') : null;


            //NOTE: On ihan toimiva tapa. Alla on myös toinen tapa, jolla voi hyvin pilkkoa haun osiksi
            var books = await _context.Books.Where(x => (names != null ? names.Contains(x.Name) : true)
                                                && (years != null ? years.Contains(x.Year.ToString()) : true)
                                                && (languages != null ? languages.Contains(x.Language.Name) : true)
                                                && (subjects != null ? x.Subjects.Any(y => subjects.Contains(y.Name)) : true)
                                                && (authors != null ? x.Authors.Any(y => authors.Contains(y.Name)) : true)
                                                && (publishers != null ? publishers.Contains(x.Publisher.Name) : true)
                                                && (producers != null ? producers.Contains(x.Producer.Name) : true))
                                            .Include(x => x.Authors)
                                            .Include(x => x.Subjects)
                                            .Include(x => x.Language)
                                            .Include(x => x.OrginalLanguage)
                                            .Include(x => x.Producer)
                                            .Include(x => x.Publisher).AsSplitQuery().ToListAsync();

            //Toinen tapa
            //AsQueryablen avulla voidaan haku koota dynaamisesti ja haku suoritetaan vasta sitten, kun sitä kutsutaan listaksi tai muuta vastaavaa tehdään
            //Lisää infoa esim täällä https://emolike.net/entity-framework-tolist-vs-asenumerable-vs-asqueryable
            var bookQuery = _context.Books.AsQueryable();

            //Rakennetaan queryä
            if (subjects.Any())
            {
                bookQuery = _context.Books.Where(x => x.Subjects.Any(y => subjects.Contains(y.Name)));
            }

            //Rakennetaan queryä
            if (publishers.Any())
            {
                bookQuery = _context.Books.Where(x => publishers.Contains(x.Publisher.Name));
            }

            //Suoritetaan query, eli tässä vaiheessa tekee tietokantaan kyselyn
            var result = await bookQuery.ToListAsync();


            return _mapper.Map<BookDto[]>(books);
        }




        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("EditBook/{id}")]
        public async Task<IActionResult> PutBook(string id, BookDtoIn book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            try
            {
                var bookEntity = await _context.Books.Where(x => x.Id == book.Id).Include(x => x.Authors)
                                            .Include(x => x.Subjects)
                                            .Include(x => x.Language)
                                            .Include(x => x.OrginalLanguage)
                                            .Include(x => x.Producer)
                                            .Include(x => x.Publisher).FirstOrDefaultAsync();

                var entityBook = _mapper.Map<Book>(book);

                entityBook.Publisher = HandlePublisher(entityBook);
                entityBook.Producer = HandleProducer(entityBook);
                entityBook.OrginalLanguage = HandleLanguage(entityBook.OrginalLanguage);
                entityBook.Language = HandleLanguage(entityBook.Language);
                entityBook.Authors = HandleAuthors(entityBook);
                entityBook.Subjects = HandleSubjects(entityBook);

                bookEntity = _mapper.Map(entityBook, bookEntity);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateBook")]
        public async Task<ActionResult<BookDtoIn>> PostBook(BookDtoIn book)
        {
            var entityBook = _mapper.Map<Book>(book);

            entityBook.Id = Guid.NewGuid().ToString();
            entityBook.Authors = HandleAuthors(entityBook);
            entityBook.Subjects = HandleSubjects(entityBook);
            entityBook.Publisher = HandlePublisher(entityBook);
            entityBook.Producer = HandleProducer(entityBook);
            entityBook.OrginalLanguage = HandleLanguage(entityBook.OrginalLanguage);
            entityBook.Language = HandleLanguage(entityBook.Language);

            _context.Books.Add(entityBook);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = entityBook.Id }, _mapper.Map<BookDtoIn>(entityBook));
        }

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private Publisher HandlePublisher(Book entityBook)
        {
            Publisher publisherById = _context.Publishers
                                            .Where(x => x.Id == entityBook.Publisher.Id)
                                            .FirstOrDefault();
            if (publisherById != null)
            {
                return publisherById;
            }

            Publisher publisher = _context.Publishers
                                            .Where(x => x.Name == entityBook.Publisher.Name)
                                            .FirstOrDefault();
            if (publisher == null)
            {
                publisher = new Publisher()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = entityBook.Publisher.Name
                };

                _context.Publishers.Add(publisher);
            }

            return publisher;
        }

        private Producer HandleProducer(Book entityBook)
        {
            Producer producerById = _context.Producers
                                            .Where(x => x.Id == entityBook.Producer.Id)
                                            .FirstOrDefault();
            if (producerById != null)
            {
                return producerById;
            }

            Producer producer = _context.Producers
                                            .Where(x => x.Name == entityBook.Producer.Name)
                                            .FirstOrDefault();
            if (producer == null)
            {
                producer = new Producer()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = entityBook.Producer.Name
                };

                _context.Producers.Add(producer);
            }

            return producer;
        }

        private Language HandleLanguage(Language _language)
        {
            Language languageById = _context.Languages
                                            .Where(x => x.Id == _language.Id)
                                            .FirstOrDefault();
            if (languageById != null)
            {
                return languageById;
            }

            Language language = _context.Languages
                                            .Where(x => x.Name == _language.Name)
                                            .FirstOrDefault();
            if (language == null)
            {
                language = new Language()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = _language.Name
                };

                _context.Languages.Add(language);
                _context.SaveChanges();
            }

            return language;
        }

        private List<Author> HandleAuthors(Book entityBook)
        {
            List<Author> authors = new List<Author>();

            for (var i = 0; i < entityBook.Authors.Count; i++)
            {
                Author author = entityBook.Authors.ElementAt(i);

                Author authorById = _context.Authors
                                            .Where(x => x.Id == author.Id)
                                            .FirstOrDefault();
                if (authorById != null)
                {
                    authors.Add(authorById);
                }
                else
                {
                    Author _author = _context.Authors
                                                .Where(x => x.Name == author.Name)
                                                .FirstOrDefault();

                    if (_author == null)
                    {
                        _author = new Author()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = author.Name
                        };

                        _context.Authors.Add(_author);
                    }

                    authors.Add(_author);
                }  
            }

            return authors;
        }

        private List<Subject> HandleSubjects(Book entityBook)
        {
            List<Subject> subjects = new List<Subject>();

            for (var i = 0; i < entityBook.Subjects.Count; i++)
            {
                Subject subject = entityBook.Subjects.ElementAt(i);
                Subject subjectById = _context.Subjects
                                            .Where(x => x.Id == subject.Id)
                                            .FirstOrDefault();
                if (subjectById != null)
                {
                    subjects.Add(subjectById);
                }
                else
                {
                    Subject _subject = _context.Subjects
                                                .Where(x => x.Name == subject.Name)
                                                .FirstOrDefault();
                    if (_subject == null)
                    {
                        _subject = new Subject()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = subject.Name
                        };

                        _context.Subjects.Add(_subject);
                    }

                    subjects.Add(_subject);
                }
            }

            return subjects;
        }
    }
}
