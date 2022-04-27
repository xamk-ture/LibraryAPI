using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class SubjectDto : BaseDtoOut
    {
        public string Name { get; set; }
    }

    public class SubjectDtoProfile : Profile
    {
        public SubjectDtoProfile()
        {
            CreateMap<Models.Subject, SubjectDto>().ReverseMap();
        }
    }
}
