﻿using System;
using System.Collections.Generic;

namespace dz_13._03.Models;

public partial class Author
{
    public int Id { get; set; }

    public string? Name { get; set; }   

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
