using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class Lending : BaseModel
    {
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string BookId { get; set; }
        public Book Book { get; set; }
        public bool IsLate { get; set; }

    }
}
