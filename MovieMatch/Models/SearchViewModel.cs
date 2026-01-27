using System.Collections.Generic;
using MovieMatch.Models.Tmdb;

namespace MovieMatch.Models
{
    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public TitleKind Kind { get; set; } = TitleKind.All;
        public double? MinRating { get; set; }
        public List<string> SelectedGenres { get; set; } = new();

        public List<TmdbGenreDto> Genres { get; set; } = new();
        public List<TmdbMovieDto> Movies { get; set; } = new();
        public List<TmdbTvDto> Series { get; set; } = new();

        public List<string> SelectedPlatforms { get; set; } = new();
        public Dictionary<int, List<TmdbWatchProvider>> MovieProviders { get; set; } = new();
        public Dictionary<int, List<TmdbWatchProvider>> SeriesProviders { get; set; } = new();
    }
}
