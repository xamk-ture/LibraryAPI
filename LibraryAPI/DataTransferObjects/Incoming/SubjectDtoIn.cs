using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class SubjectDtoIn : BaseDtoIn
    {
        public string Name { get; set; }
    }

    public class SubjectDtoInProfile : Profile
    {
        public SubjectDtoInProfile()
        {
            CreateMap<Models.Subject, SubjectDtoIn>().ReverseMap();
        }
    }
}
