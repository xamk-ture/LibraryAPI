using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class LendingDtoIn : BaseDtoIn
    {
        public string BookId { get; set; }
        public string UserId { get; set; }
    }

    public class LendingsDtoInProfile : Profile
    {
        public LendingsDtoInProfile()
        {
            CreateMap<Models.Lending, LendingDtoIn>().ReverseMap();
        }
    }
}
