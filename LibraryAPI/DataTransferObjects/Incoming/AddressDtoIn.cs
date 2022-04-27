using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LibraryAPI.DataTransferObjects.Incoming
{
    public class AddressDtoIn : BaseDtoIn
    {
        public string Street { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }

    public class AddressDtoInProfile : Profile
    {
        public AddressDtoInProfile()
        {
            CreateMap<Models.Address, AddressDtoIn>().ReverseMap();
        }
    }
}
