using System;

namespace MovieMatch.Models
{
    public class MovieComment
    {
        public int Id { get; set; }

        // Id filmu/serialu z TMDb
        public int TmdbId { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
