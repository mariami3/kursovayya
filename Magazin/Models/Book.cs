using System;
using System.Collections.Generic;

namespace Magazin.Models;

public partial class Book
{
    public int IdBook { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? BookUrl { get; set; }

    public decimal Price { get; set; }

    public string? BookType { get; set; }

    public int? Pages { get; set; }

    public int? WeightGr { get; set; }

    public string? Size { get; set; }

    public int? GenreId { get; set; }

    public int? AuthorId { get; set; }

    public string? PublisherBrand { get; set; }

    public int? PublicationYear { get; set; }

    public bool IsFavorite { get; set; }

    public int CartQuantity { get; set; }

    public bool IsActive { get; set; }

    public string? AgeLimit { get; set; }

    public int? EditionYear { get; set; }

    public virtual ICollection<Adaptation> Adaptations { get; set; } = new List<Adaptation>();

    public virtual Author? Author { get; set; }

    public virtual ICollection<BookVoice> BookVoices { get; set; } = new List<BookVoice>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<FavoriteItem> FavoriteItems { get; set; } = new List<FavoriteItem>();

    public virtual Genre? Genre { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
