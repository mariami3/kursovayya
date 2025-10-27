using System;
using System.Collections.Generic;

namespace Magazin.Models;

public partial class FavoriteItem
{
    public int IdFavoriteItem { get; set; }

    public int? UserId { get; set; }

    public int? BookId { get; set; }

    public decimal Price { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? User { get; set; }
}
