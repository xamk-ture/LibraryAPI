using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using AutoMapper;
using LibraryAPI.DataTransferObjects.Outgoing;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public DataController(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("GetData")]
        public async Task<ActionResult<object>> GetData(bool subject, bool author, bool producer, bool publisher, bool language)
        {
            Dictionary<string, object> requestedData = new Dictionary<string, object>();

            if (subject)
            {
                var subjects = _mapper.Map<SubjectDto[]>(_context.Subjects);
                requestedData["subjects"] = subjects;
            }

            if (author)
            {
                var authors = _mapper.Map<AuthorDto[]>(_context.Authors);
                requestedData["authors"] = authors;
            }

            if (producer)
            {
                var producers = _mapper.Map<ProducerDto[]>(_context.Producers);
                requestedData["producers"] = producers;
            }

            if (publisher)
            {
                var publishers = _mapper.Map<PublisherDto[]>(_context.Publishers);
                requestedData["publishers"] = publishers;
            }

            if (language)
            {
                var languages = _mapper.Map<LanguageDto[]>(_context.Languages);
                requestedData["languages"] = languages;
            }



            return requestedData;
        }

    }
}
