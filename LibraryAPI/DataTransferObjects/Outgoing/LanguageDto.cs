using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class LanguageDto
    {
        public string Name { get; set; }
    }

    public class LanguageDtoProfile : Profile
    {
        public LanguageDtoProfile()
        {
            CreateMap<Models.Language, LanguageDto>().ReverseMap();
        }
    }
}
