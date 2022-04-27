using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class Address : BaseModel
    {
        public string Street { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }
}
