using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MovieMatch.Models;

namespace MovieMatch.Areas.Identity.Pages.Account.Manage
{
    public class StreamingPlatformsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public StreamingPlatformsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // lista dostpnych platform (takich samych jak w wyszukiwarce)
        public List<string> AllPlatforms { get; } = new()
{
    "Netflix",
    "HBO Max",
    "Amazon Prime Video",
    "Disney+",
    "Apple TV+",
    "SkyShowtime",
    "Viaplay",
    "Player",
    "Canal+ Online",
    "Polsat Box Go",
    "TVP VOD"
};

        // to, co jest zaznaczone w formularzu
        [BindProperty]
        public List<string> SelectedPlatforms { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Nie udao si zaadowa uytkownika.");
            }

            if (!string.IsNullOrWhiteSpace(user.StreamingPlatforms))
            {
                SelectedPlatforms = user.StreamingPlatforms
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Nie udao si zaadowa uytkownika.");
            }

            var normalized = (SelectedPlatforms ?? new List<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .ToList();

            user.StreamingPlatforms = string.Join(";", normalized);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            TempData["StatusMessage"] = "Platformy streamingowe zostay zaktualizowane.";
            return RedirectToPage();
        }
    }
}
