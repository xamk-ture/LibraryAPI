using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class PublisherDtoIn : BaseDtoIn
    {
        public string Name { get; set; }
    }

    public class PublisherDtoInProfile : Profile
    {
        public PublisherDtoInProfile()
        {
            CreateMap<Models.Publisher, PublisherDtoIn>().ReverseMap();
        }
    }
}