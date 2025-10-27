using System;
using System.Collections.Generic;

namespace Magazin.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public int? UserId { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalSum { get; set; }

    public string StatusOrders { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User? User { get; set; }
}
