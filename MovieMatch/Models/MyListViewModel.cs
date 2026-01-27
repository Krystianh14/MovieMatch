using System.Collections.Generic;
using MovieMatch.Models.Tmdb;

namespace MovieMatch.Models
{
    public class MyListViewModel
    {
        public List<TmdbMovieDto> FavoriteMovies { get; set; } = new();
        public List<TmdbTvDto> FavoriteSeries { get; set; } = new();

        public List<TmdbMovieDto> WatchLaterMovies { get; set; } = new();
        public List<TmdbTvDto> WatchLaterSeries { get; set; } = new();
    }
}
