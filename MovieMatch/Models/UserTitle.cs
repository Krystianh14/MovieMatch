using System;
using MovieMatch.Models;

namespace MovieMatch.Models
{
    public class UserTitle
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public int TmdbId { get; set; }

        public bool IsFavorite { get; set; }
        public bool WatchLater { get; set; }
        public int? UserRating { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; }
    }
}
