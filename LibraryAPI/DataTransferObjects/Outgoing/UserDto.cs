using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class UserDto : BaseDtoOut
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public AddressDto Address { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsBanned { get; set; }

        public class UserDtoProfile : Profile
        {
            public UserDtoProfile()
            {
                CreateMap<Models.User, UserDto>().ReverseMap();
            }
        }
    }
}
