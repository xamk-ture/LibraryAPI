using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class PublisherDto : BaseDtoOut
    {
        public string Name { get; set; }
    }

    public class PublisherDtoProfile : Profile
    {
        public PublisherDtoProfile()
        {
            CreateMap<Models.Publisher, PublisherDto>().ReverseMap();
        }
    }
}
