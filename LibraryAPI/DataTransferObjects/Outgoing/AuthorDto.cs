using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class AuthorDto : BaseDtoOut
    {
        public string Name { get; set; }
    }

    public class AuthorDtoProfile : Profile
    {
        public AuthorDtoProfile()
        {
            CreateMap<Models.Author, AuthorDto>().ReverseMap();
        }
    }
}
