using System;
using System.Collections.Generic;

namespace BookMagazin.Models;

public partial class Adaptation
{
    public int IdAdaptation { get; set; }

    public int? BookId { get; set; }

    public string? Title { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Link { get; set; }

    public virtual Book? Book { get; set; }
}
