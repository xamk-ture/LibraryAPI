using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class ProducerDtoIn : BaseDtoIn
    {
        public string Name { get; set; }
    }

    public class ProducerDtoInProfile : Profile
    {
        public ProducerDtoInProfile()
        {
            CreateMap<Models.Producer, ProducerDtoIn>().ReverseMap();
        }
    }
}
