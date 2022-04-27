using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class BookDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<AuthorDto> Authors { get; set; }
        public int? Year { get; set; }

        public ICollection<SubjectDto> Subjects { get; set; }

        public LanguageDto Language { get; set; }

        public LanguageDto OrginalLanguage { get; set; }

        public PublisherDto Publisher { get; set; }

        public ProducerDto Producer { get; set; }

        public string? AdditionalInformation { get; set; }
        public string? ISBN { get; set; }
    }

    public class BookDtoProfile : Profile
    {
        public BookDtoProfile()
        {
            CreateMap<Models.Book, BookDto>().ReverseMap();
        }
    }
}

