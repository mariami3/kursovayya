using System;
using System.Collections.Generic;

namespace Magazin.Models;

public partial class Review
{
    public int IdReview { get; set; }

    public string UserName { get; set; } = null!;

    public int Rating { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime? DateCreated { get; set; }

    public int? BookId { get; set; }

    public int? UserId { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? User { get; set; }
}
