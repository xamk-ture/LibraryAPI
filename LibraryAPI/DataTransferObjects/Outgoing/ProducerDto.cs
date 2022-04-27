using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class ProducerDto : BaseDtoOut
    {
        public string Name { get; set; }
    }

    public class ProducerDtoProfile : Profile
    {
        public ProducerDtoProfile()
        {
            CreateMap<Models.Producer, ProducerDto>().ReverseMap();
        }
    }
}
