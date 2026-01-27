using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using MovieMatch.Models;
using MovieMatch.Services;

namespace MovieMatch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TmdbService _tmdb;

        public HomeController(ILogger<HomeController> logger, TmdbService tmdb)
        {
            _logger = logger;
            _tmdb = tmdb;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel();

            try
            {
                var movies = await _tmdb.GetTopRatedMoviesAsync();
                vm.TopMovies = movies
                    .OrderByDescending(m => m.VoteAverage)
                    .Take(5)
                    .ToList();

                var series = await _tmdb.GetTopRatedSeriesAsync();
                vm.TopSeries = series
                    .OrderByDescending(s => s.VoteAverage)
                    .Take(5)
                    .ToList();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Bd pobierania danych z TMDB na stronie gwnej.");
                // zostawiamy puste listy  widok pokae komunikat
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nieoczekiwany bd na stronie gwnej.");
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
