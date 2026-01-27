using System.Collections.Generic;
using MovieMatch.Models.Tmdb;

namespace MovieMatch.Models
{
    public class TitleDetailsViewModel
    {
        public int TmdbId { get; set; }
        public TitleKind Kind { get; set; }

        public TmdbMovieDto? Movie { get; set; }
        public TmdbTvDto? Series { get; set; }

        public UserTitle? UserTitle { get; set; }

        public List<MovieComment> Comments { get; set; } = new();

        public double? AverageRating { get; set; }

        // ===== Trailer (z TMDb videos) =====
        public string? TrailerUrl { get; set; }
        public string? TrailerName { get; set; }
    }
}
