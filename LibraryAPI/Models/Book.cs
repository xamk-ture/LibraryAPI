using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class Book : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
        public int Year { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }

        public Language OrginalLanguage { get; set; }

        public Language Language { get; set; }

        public Publisher Publisher { get; set; }

        public Producer Producer { get; set; }

        public string AdditionalInformation { get; set; }
        public string ISBN { get; set; }

        public int Amount { get; set; }

        public ICollection<Lending> Lendings { get; set; }

        public class BookProfile : Profile
        {
            public BookProfile()
            {
                CreateMap<Book, Book>().ReverseMap();
            }
        }
    }
}
