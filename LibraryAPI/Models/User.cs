using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class User : BaseModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsBanned { get; set; }
        public ICollection<Lending> Lendings { get; set; }

        public class UserProfile : Profile
        {
            public UserProfile()
            {
                CreateMap<User, User>().ReverseMap();
            }
        }
    }
}
