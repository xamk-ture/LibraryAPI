using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class BookDtoIn : BaseDtoIn
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<AuthorDtoIn> Authors { get; set; }
        public int Year { get; set; }
        public virtual ICollection<SubjectDtoIn> Subjects { get; set; }

        public LanguageDtoIn OrginalLanguage { get; set; }

        public LanguageDtoIn Language { get; set; }

        public PublisherDtoIn Publisher { get; set; }

        public ProducerDtoIn Producer { get; set; }

        public string AdditionalInformation { get; set; }
        public string ISBN { get; set; }
    }

    public class BookDtoInProfile : Profile
    {
        public BookDtoInProfile()
        {
            CreateMap<Models.Book, BookDtoIn>().ReverseMap();
        }
    }
}
