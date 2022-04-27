using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class UserDtoIn : BaseDtoIn
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public AddressDtoIn Address { get; set; }
    }

    public class UserDtoInProfile : Profile
    {
        public UserDtoInProfile()
        {
            CreateMap<Models.User, UserDtoIn>().ReverseMap();
        }
    }
}
