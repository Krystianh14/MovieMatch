using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using MovieMatch.Models.Tmdb;

namespace MovieMatch.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");

            _apiKey = configuration["Tmdb:ApiKey"]
                      ?? throw new InvalidOperationException("Brak klucza TMDB w appsettings.json");
        }

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<TmdbMovieDto>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<TmdbMovieDto>();

            var url = $"search/movie?api_key={_apiKey}&language=pl-PL&query={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<TmdbSearchMovieResponse>(json, _jsonOptions);

            return data?.Results ?? new List<TmdbMovieDto>();
        }

        public async Task<TmdbMovieDto> GetMovieDetailsAsync(int tmdbId)
        {
            var url = $"movie/{tmdbId}?api_key={_apiKey}&language=pl-PL";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var movie = JsonSerializer.Deserialize<TmdbMovieDto>(json, _jsonOptions);

            return movie ?? new TmdbMovieDto();
        }

        public async Task<List<TmdbTvDto>> SearchSeriesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<TmdbTvDto>();

            var url = $"search/tv?api_key={_apiKey}&language=pl-PL&query={Uri.EscapeDataString(query)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<TmdbSearchTvResponse>(json, _jsonOptions);

            return data?.Results ?? new List<TmdbTvDto>();
        }

        public async Task<TmdbTvDto> GetSeriesDetailsAsync(int tmdbId)
        {
            var url = $"tv/{tmdbId}?api_key={_apiKey}&language=pl-PL";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var series = JsonSerializer.Deserialize<TmdbTvDto>(json, _jsonOptions);

            return series ?? new TmdbTvDto();
        }

        public async Task<List<TmdbGenreDto>> GetMovieGenresAsync()
        {
            var url = $"genre/movie/list?api_key={_apiKey}&language=pl-PL";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbGenreList>(json, _jsonOptions);

            return data?.Genres ?? new List<TmdbGenreDto>();
        }

        public async Task<List<TmdbGenreDto>> GetSeriesGenresAsync()
        {
            var url = $"genre/tv/list?api_key={_apiKey}&language=pl-PL";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbGenreList>(json, _jsonOptions);

            return data?.Genres ?? new List<TmdbGenreDto>();
        }

        public async Task<List<TmdbGenreDto>> GetAllGenresAsync()
        {
            var movieGenres = await GetMovieGenresAsync();
            var seriesGenres = await GetSeriesGenresAsync();

            var all = movieGenres
                .Concat(seriesGenres)
                .GroupBy(g => g.Id)
                .Select(g => g.First())
                .OrderBy(g => g.Name)
                .ToList();

            return all;
        }

        public async Task<List<TmdbWatchProvider>> GetMovieFlatrateProvidersAsync(int tmdbId, string countryCode = "PL")
        {
            var url = $"movie/{tmdbId}/watch/providers?api_key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbWatchProvidersResponse>(json, _jsonOptions);

            if (data == null || data.Results == null) return new List<TmdbWatchProvider>();

            if (data.Results.TryGetValue(countryCode, out var country))
            {
                return country.Flatrate ?? new List<TmdbWatchProvider>();
            }

            return new List<TmdbWatchProvider>();
        }

        public async Task<List<TmdbWatchProvider>> GetSeriesFlatrateProvidersAsync(int tmdbId, string countryCode = "PL")
        {
            var url = $"tv/{tmdbId}/watch/providers?api_key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbWatchProvidersResponse>(json, _jsonOptions);

            if (data == null || data.Results == null) return new List<TmdbWatchProvider>();

            if (data.Results.TryGetValue(countryCode, out var country))
            {
                return country.Flatrate ?? new List<TmdbWatchProvider>();
            }

            return new List<TmdbWatchProvider>();
        }

        public async Task<List<TmdbMovieDto>> GetPopularMoviesAsync()
        {
            var url = $"movie/popular?api_key={_apiKey}&language=pl-PL&page=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbSearchMovieResponse>(json, _jsonOptions);

            return data?.Results ?? new List<TmdbMovieDto>();
        }

        public async Task<List<TmdbTvDto>> GetPopularSeriesAsync()
        {
            var url = $"tv/popular?api_key={_apiKey}&language=pl-PL&page=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbSearchTvResponse>(json, _jsonOptions);

            return data?.Results ?? new List<TmdbTvDto>();
        }

        public async Task<List<TmdbMovieDto>> GetTopRatedMoviesAsync()
        {
            var url = $"movie/top_rated?api_key={_apiKey}&language=pl-PL&page=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbSearchMovieResponse>(json, _jsonOptions);

            return data?.Results ?? new List<TmdbMovieDto>();
        }

        public async Task<List<TmdbTvDto>> GetTopRatedSeriesAsync()
        {
            var url = $"tv/top_rated?api_key={_apiKey}&language=pl-PL&page=1";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbSearchTvResponse>(json, _jsonOptions);

            return data?.Results ?? new List<TmdbTvDto>();
        }

        public Task<TmdbTrailerInfo?> GetMovieTrailerAsync(int tmdbId)
            => GetTrailerAsync(mediaType: "movie", tmdbId: tmdbId);

        public Task<TmdbTrailerInfo?> GetSeriesTrailerAsync(int tmdbId)
            => GetTrailerAsync(mediaType: "tv", tmdbId: tmdbId);

        private async Task<TmdbTrailerInfo?> GetTrailerAsync(string mediaType, int tmdbId)
        {
            var plVideos = await GetVideosAsync(mediaType, tmdbId, "pl-PL");
            var best = PickBestVideo(plVideos);

            if (best == null)
            {
                var enVideos = await GetVideosAsync(mediaType, tmdbId, "en-US");
                best = PickBestVideo(enVideos);
            }

            if (best == null)
                return null;

            var url = BuildExternalVideoUrl(best.Site, best.Key);
            if (string.IsNullOrWhiteSpace(url))
                return null;

            return new TmdbTrailerInfo
            {
                Url = url,
                Name = best.Name,
                Site = best.Site,
                Type = best.Type
            };
        }

        private async Task<List<TmdbVideoDto>> GetVideosAsync(string mediaType, int tmdbId, string language)
        {
            var url = $"{mediaType}/{tmdbId}/videos?api_key={_apiKey}&language={language}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbVideosResponse>(json, _jsonOptions);

            return data?.Results ?? new List<TmdbVideoDto>();
        }

        private static TmdbVideoDto? PickBestVideo(IEnumerable<TmdbVideoDto> videos)
        {
            if (videos == null)
                return null;

            return videos
                .Where(v =>
                    !string.IsNullOrWhiteSpace(v.Key) &&
                    !string.IsNullOrWhiteSpace(v.Site))
                .Select(v => new
                {
                    Video = v,
                    Score = ScoreVideo(v),
                    Published = TryParseDate(v.PublishedAt)
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Published ?? DateTime.MinValue)
                .Select(x => x.Video)
                .FirstOrDefault();
        }

        private static int ScoreVideo(TmdbVideoDto v)
        {
            int score = 0;

            // Preferuj YouTube
            if (v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
                score += 1000;
            else if (v.Site.Equals("Vimeo", StringComparison.OrdinalIgnoreCase))
                score += 900;

            // Preferuj Trailer > Teaser > reszta
            if (v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase))
                score += 200;
            else if (v.Type.Equals("Teaser", StringComparison.OrdinalIgnoreCase))
                score += 150;
            else if (v.Type.Equals("Clip", StringComparison.OrdinalIgnoreCase))
                score += 50;

            // Oficjalne minimalnie wyżej
            if (v.Official)
                score += 20;

            return score;
        }

        private static DateTime? TryParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, out var dt))
                return dt;

            return null;
        }

        private static string? BuildExternalVideoUrl(string site, string key)
        {
            if (string.IsNullOrWhiteSpace(site) || string.IsNullOrWhiteSpace(key))
                return null;

            if (site.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
                return $"https://www.youtube.com/watch?v={key}";

            if (site.Equals("Vimeo", StringComparison.OrdinalIgnoreCase))
                return $"https://vimeo.com/{key}";

            // inne serwisy – na razie brak
            return null;
        }
    }
}
