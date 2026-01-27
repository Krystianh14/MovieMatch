using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMatch.Models.Tmdb;
using MovieMatch.Models;
using MovieMatch.Services;

namespace MovieMatch.Controllers
{
    public class SearchController : Controller
    {
        private readonly TmdbService _tmdb;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public SearchController(TmdbService tmdb, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _tmdb = tmdb;
            _userManager = userManager;
            _db = db;
        }

        // pobieramy platformy z profilu zalogowanego użytkownika
        private async Task<List<string>> GetUserPlatformsAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return new List<string>();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrWhiteSpace(user.StreamingPlatforms))
                return new List<string>();

            return user.StreamingPlatforms
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userPlatforms = await GetUserPlatformsAsync();

            var model = new SearchViewModel
            {
                Kind = TitleKind.All,
                Genres = await _tmdb.GetAllGenresAsync(),
                SelectedPlatforms = userPlatforms
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Results(
            string query,
            TitleKind kind = TitleKind.All,
            List<string>? selectedGenres = null,
            double? minRating = null,
            List<string>? selectedPlatforms = null)
        {
            bool noQuery = string.IsNullOrWhiteSpace(query);

            var model = new SearchViewModel
            {
                Query = query ?? string.Empty,
                Kind = kind,
                SelectedGenres = selectedGenres ?? new List<string>(),
                MinRating = minRating,
                SelectedPlatforms = selectedPlatforms ?? new List<string>()
            };

            var allGenres = await _tmdb.GetAllGenresAsync();
            model.Genres = allGenres;

            // ===== FILMY =====
            if (kind == TitleKind.Movie || kind == TitleKind.All)
            {
                List<TmdbMovieDto> movies;

                if (noQuery)
                {
                    if (model.SelectedGenres.Any())
                    {
                        movies = new List<TmdbMovieDto>();

                        foreach (var genreIdStr in model.SelectedGenres)
                        {
                            if (!int.TryParse(genreIdStr, out var gid))
                                continue;

                            var genreName = allGenres.FirstOrDefault(g => g.Id == gid)?.Name;
                            if (string.IsNullOrWhiteSpace(genreName))
                                continue;

                            var results = await _tmdb.SearchMoviesAsync(genreName);
                            if (results != null)
                                movies.AddRange(results);
                        }
                        movies = movies
                            .GroupBy(m => m.Id)
                            .Select(g => g.First())
                            .ToList();
                    }
                    else
                    {
                        movies = await _tmdb.GetPopularMoviesAsync();
                    }
                }
                else
                {
                    movies = await _tmdb.SearchMoviesAsync(query) ?? new List<TmdbMovieDto>();
                }
                if (minRating.HasValue)
                {
                    movies = movies
                        .Where(m => m.VoteAverage >= minRating.Value)
                        .ToList();
                }

                if (model.SelectedGenres != null && model.SelectedGenres.Any())
                {
                    var genreIds = model.SelectedGenres
                        .Select(id => int.TryParse(id, out var gid) ? gid : (int?)null)
                        .Where(gid => gid.HasValue)
                        .Select(gid => gid.Value)
                        .ToList();

                    movies = movies
                        .Where(m => m.GenreIds != null && m.GenreIds.Any(id => genreIds.Contains(id)))
                        .ToList();
                }

                model.Movies = movies;
            }

            // ===== SERIALE =====
            if (kind == TitleKind.Series || kind == TitleKind.All)
            {
                List<TmdbTvDto> series;

                if (noQuery)
                {
                    if (model.SelectedGenres.Any())
                    {
                        series = new List<TmdbTvDto>();

                        foreach (var genreIdStr in model.SelectedGenres)
                        {
                            if (!int.TryParse(genreIdStr, out var gid))
                                continue;

                            var genreName = allGenres.FirstOrDefault(g => g.Id == gid)?.Name;
                            if (string.IsNullOrWhiteSpace(genreName))
                                continue;

                            var results = await _tmdb.SearchSeriesAsync(genreName);
                            if (results != null)
                                series.AddRange(results);
                        }

                        series = series
                            .GroupBy(s => s.Id)
                            .Select(g => g.First())
                            .ToList();
                    }
                    else
                    {
                        series = await _tmdb.GetPopularSeriesAsync();
                    }
                }
                else
                {
                    series = await _tmdb.SearchSeriesAsync(query) ?? new List<TmdbTvDto>();
                }
                if (minRating.HasValue)
                {
                    series = series
                        .Where(s => s.VoteAverage >= minRating.Value)
                        .ToList();
                }

                if (model.SelectedGenres != null && model.SelectedGenres.Any())
                {
                    var genreIds = model.SelectedGenres
                        .Select(id => int.TryParse(id, out var gid) ? gid : (int?)null)
                        .Where(gid => gid.HasValue)
                        .Select(gid => gid.Value)
                        .ToList();

                    series = series
                        .Where(s => s.GenreIds != null && s.GenreIds.Any(id => genreIds.Contains(id)))
                        .ToList();
                }

                model.Series = series;
            }

            // ===== DOŁĄCZ PLATFORMY Z TMDB =====
            if (model.Movies != null && model.Movies.Any())
            {
                model.MovieProviders = new Dictionary<int, List<TmdbWatchProvider>>();
                foreach (var m in model.Movies)
                {
                    var providers = await _tmdb.GetMovieFlatrateProvidersAsync(m.Id);
                    model.MovieProviders[m.Id] = providers;
                }
            }

            if (model.Series != null && model.Series.Any())
            {
                model.SeriesProviders = new Dictionary<int, List<TmdbWatchProvider>>();
                foreach (var s in model.Series)
                {
                    var providers = await _tmdb.GetSeriesFlatrateProvidersAsync(s.Id);
                    model.SeriesProviders[s.Id] = providers;
                }
            }

            // ===== FILTROWANIE PO PLATFORMACH =====
            if (model.SelectedPlatforms != null && model.SelectedPlatforms.Any())
            {
                var selectedSet = model.SelectedPlatforms
                    .Select(NormalizePlatformKey)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (model.Movies != null && model.Movies.Any())
                {
                    model.Movies = model.Movies
                        .Where(m =>
                        {
                            if (!model.MovieProviders.TryGetValue(m.Id, out var providers) ||
                                providers == null || providers.Count == 0)
                                return false;

                            var moviePlatforms = providers
                                .Select(p => NormalizePlatformKey(p.ProviderName));

                            return moviePlatforms.Any(p => selectedSet.Contains(p));
                        })
                        .ToList();
                }

                if (model.Series != null && model.Series.Any())
                {
                    model.Series = model.Series
                        .Where(s =>
                        {
                            if (!model.SeriesProviders.TryGetValue(s.Id, out var providers) ||
                                providers == null || providers.Count == 0)
                                return false;

                            var seriesPlatforms = providers
                                .Select(p => NormalizePlatformKey(p.ProviderName));

                            return seriesPlatforms.Any(p => selectedSet.Contains(p));
                        })
                        .ToList();
                }
            }
            return View("Index", model);
        }

        private static string NormalizePlatformKey(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            name = name.ToLowerInvariant();

            if (name.Contains("netflix"))
                return "Netflix";

            if (name.Contains("hbo") || name.Contains("max"))
                return "HBO Max";

            if (name.Contains("disney"))
                return "Disney+";

            if (name.Contains("amazon") || name.Contains("prime"))
                return "Amazon Prime Video";

            if (name.Contains("apple"))
                return "Apple TV+";

            if (name.Contains("skyshowtime"))
                return "SkyShowtime";

            if (name.Contains("viaplay"))
                return "Viaplay";

            if (name.Contains("player"))
                return "Player";

            if (name.Contains("canal"))
                return "Canal+ Online";

            if (name.Contains("polsat"))
                return "Polsat Box Go";

            if (name.Contains("tvp"))
                return "TVP VOD";

            return name;
        }

        private async Task<UserTitle> GetOrCreateUserTitleAsync(string userId, int tmdbId)
        {
            var existing = await _db.UserTitles
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TmdbId == tmdbId);

            if (existing != null)
                return existing;

            var newItem = new UserTitle
            {
                UserId = userId,
                TmdbId = tmdbId,
                IsFavorite = false,
                WatchLater = false
            };

            _db.UserTitles.Add(newItem);
            await _db.SaveChangesAsync();

            return newItem;
        }

        public async Task<IActionResult> Details(int tmdbId, TitleKind kind = TitleKind.All)
        {
            TmdbMovieDto? movie = null;
            TmdbTvDto? series = null;

            // Trailer info
            TmdbTrailerInfo? trailer = null;

            if (kind == TitleKind.Series)
            {
                series = await _tmdb.GetSeriesDetailsAsync(tmdbId);
                if (series == null) return NotFound();

                trailer = await _tmdb.GetSeriesTrailerAsync(tmdbId);
            }
            else
            {
                movie = await _tmdb.GetMovieDetailsAsync(tmdbId);
                if (movie == null) return NotFound();

                trailer = await _tmdb.GetMovieTrailerAsync(tmdbId);
            }

            // Komentarze
            var comments = await _db.MovieComments
                .Where(c => c.TmdbId == tmdbId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            // Status użytkownika (ulubiony, do obejrzenia, ocena)
            UserTitle? userTitle = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                userTitle = await _db.UserTitles
                    .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TmdbId == tmdbId);
            }

            // Średnia ocena
            double? avg = null;
            var ratingsQuery = _db.UserTitles
                .Where(ut => ut.TmdbId == tmdbId && ut.UserRating.HasValue);

            if (await ratingsQuery.AnyAsync())
            {
                avg = await ratingsQuery.AverageAsync(ut => ut.UserRating!.Value);
            }

            var vm = new TitleDetailsViewModel
            {
                TmdbId = tmdbId,
                Kind = kind,
                Movie = movie,
                Series = series,
                UserTitle = userTitle,
                Comments = comments,
                AverageRating = avg,

                TrailerUrl = trailer?.Url,
                TrailerName = trailer?.Name
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int tmdbId, TitleKind kind, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction(nameof(Details), new { tmdbId, kind });

            var userId = _userManager.GetUserId(User);

            var comment = new MovieComment
            {
                TmdbId = tmdbId,
                UserId = userId,
                Content = content.Trim()
            };

            _db.MovieComments.Add(comment);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { tmdbId, kind });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int tmdbId, TitleKind kind)
        {
            var userId = _userManager.GetUserId(User);
            var status = await GetOrCreateUserTitleAsync(userId, tmdbId);

            status.IsFavorite = !status.IsFavorite;
            status.LastUpdated = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { tmdbId, kind });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWatchLater(int tmdbId, TitleKind kind)
        {
            var userId = _userManager.GetUserId(User);
            var status = await GetOrCreateUserTitleAsync(userId, tmdbId);

            status.WatchLater = !status.WatchLater;
            status.LastUpdated = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { tmdbId, kind });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateTitle(int tmdbId, TitleKind kind, int rating)
        {
            if (rating < 1 || rating > 5)
                return RedirectToAction(nameof(Details), new { tmdbId, kind });

            var userId = _userManager.GetUserId(User);
            var status = await GetOrCreateUserTitleAsync(userId, tmdbId);

            status.UserRating = rating;
            status.LastUpdated = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { tmdbId, kind });
        }

        [Authorize]
        public async Task<IActionResult> MyList()
        {
            var userId = _userManager.GetUserId(User);

            var userTitles = await _db.UserTitles
                .Where(ut => ut.UserId == userId && (ut.IsFavorite || ut.WatchLater))
                .ToListAsync();

            var vm = new MyListViewModel();

            foreach (var ut in userTitles)
            {
                TmdbMovieDto? movie = null;
                TmdbTvDto? series = null;

                try
                {
                    movie = await _tmdb.GetMovieDetailsAsync(ut.TmdbId);
                }
                catch (HttpRequestException)
                {
                }

                if (movie != null)
                {
                    if (ut.IsFavorite)
                        vm.FavoriteMovies.Add(movie);
                    if (ut.WatchLater)
                        vm.WatchLaterMovies.Add(movie);

                    continue;
                }

                try
                {
                    series = await _tmdb.GetSeriesDetailsAsync(ut.TmdbId);
                }
                catch (HttpRequestException)
                {
                }

                if (series != null)
                {
                    if (ut.IsFavorite)
                        vm.FavoriteSeries.Add(series);
                    if (ut.WatchLater)
                        vm.WatchLaterSeries.Add(series);
                }
            }

            return View(vm);
        }
    }
}
