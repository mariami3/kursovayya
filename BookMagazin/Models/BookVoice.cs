using System;
using System.Collections.Generic;

namespace BookMagazin.Models;

public partial class BookVoice
{
    public int IdVoice { get; set; }

    public int? BookId { get; set; }

    public string Title { get; set; } = null!;

    public string VoiceUrl { get; set; } = null!;

    public int? DurationSeconds { get; set; }

    public string? Format { get; set; }

    public virtual Book? Book { get; set; }
}
