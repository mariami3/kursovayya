using System;
using System.Collections.Generic;

namespace Magazin.Models;

public partial class CartItem
{
    public int IdCartItem { get; set; }

    public int Quantity { get; set; }

    public int? UserId { get; set; }

    public int? BookId { get; set; }

    public decimal Price { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? User { get; set; }
}
