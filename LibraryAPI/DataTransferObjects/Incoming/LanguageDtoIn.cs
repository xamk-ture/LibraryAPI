using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class LanguageDtoIn : BaseDtoIn
    {
        public string Name { get; set; }
    }

    public class LanguageDtoInProfile : Profile
    {
        public LanguageDtoInProfile()
        {
            CreateMap<Models.Language, LanguageDtoIn>().ReverseMap();
        }
    }
}
