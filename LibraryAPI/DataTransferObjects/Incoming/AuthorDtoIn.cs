using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class AuthorDtoIn : BaseDtoIn
    {
        public string Name { get; set; }
    }

    public class AuthorDtoInProfile : Profile
    {
        public AuthorDtoInProfile()
        {
            CreateMap<Models.Author, AuthorDtoIn>().ReverseMap();
        }
    }
}
