using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.DataTransferObjects.Outgoing
{
    public class AddressDto : BaseDtoOut
    {
        public string Street { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public class AddressDtoProfile : Profile
        {
            public AddressDtoProfile()
            {
                CreateMap<Models.Address, AddressDto>().ReverseMap();
            }
        }
    }
}
