using System.Collections.Generic;
using MovieMatch.Models.Tmdb;

namespace MovieMatch.Models
{
    public class HomeViewModel
    {
        public List<TmdbMovieDto> TopMovies { get; set; } = new();
        public List<TmdbTvDto> TopSeries { get; set; } = new();
    }
}
