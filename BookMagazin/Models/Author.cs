﻿using System;
using System.Collections.Generic;

namespace BookMagazin.Models;

public partial class Author
{
    public int IdAuthor { get; set; }

    public string NameAuthor { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
