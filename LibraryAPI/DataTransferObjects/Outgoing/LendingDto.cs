using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class LendingDto
    {
        public string Id { get; set; }
        public string BookName { get; set; }
        public DateTime Lended { get; set; }
        public string UserId { get; set; }
        public string BookId { get; set; }
        public bool IsLate { get; set; }
    }

    public class LendingsDtoProfile : Profile
    {
        public LendingsDtoProfile()
        {
            CreateMap<Models.Lending, LendingDto>()
                .ForMember(lendingDto => lendingDto.BookName, x => x.MapFrom(lending => lending.Book.Name))
                .ForMember(LendingDto => LendingDto.Lended, x => x.MapFrom(lending => lending.CreatedAt));
        }
    }
}
