using System;
using System.Collections.Generic;

namespace BookMagazin.Models;

public partial class Genre
{
    public int IdGenre { get; set; }

    public string NameGenre { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
