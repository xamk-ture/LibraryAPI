﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class Author : BaseModel
    {
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
