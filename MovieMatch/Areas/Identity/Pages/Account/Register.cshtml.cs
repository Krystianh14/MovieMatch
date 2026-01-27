#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MovieMatch.Models;

namespace MovieMatch.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // ===== PLATFORMY =====
        public List<string> AllPlatforms { get; } = new()
        {
            "Netflix",
            "Disney+",
            "HBO Max",
            "Amazon Prime Video",
            "Apple TV+",
            "SkyShowtime",
            "Viaplay",
            "Player",
            "Canal+ Online",
            "Polsat Box Go",
            "TVP VOD"
        };

        [BindProperty]
        public List<string> SelectedPlatforms { get; set; } = new();

        public string IconClass(string platform) => platform switch
        {
            "Netflix" => "fa-solid fa-n",
            "Disney+" => "fa-solid fa-star",
            "HBO Max" => "fa-solid fa-tv",
            "Amazon Prime Video" => "fa-brands fa-amazon",
            "Apple TV+" => "fa-brands fa-apple",
            "SkyShowtime" => "fa-solid fa-clapperboard",
            "Viaplay" => "fa-solid fa-play",
            "Player" => "fa-solid fa-circle-play",
            "Canal+ Online" => "fa-solid fa-tv",
            "Polsat Box Go" => "fa-solid fa-satellite-dish",
            "TVP VOD" => "fa-solid fa-video",
            _ => "fa-solid fa-tv"
        };

        // ===== INPUT =====
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email jest wymagany.")]
            [EmailAddress(ErrorMessage = "Podaj poprawny adres email.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Nick jest wymagany.")]
            [StringLength(20, ErrorMessage = "Nick może mieć maksymalnie {1} znaków.")]
            [Display(Name = "Nick")]
            public string Nick { get; set; }

            [Required(ErrorMessage = "Hasło jest wymagane.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć od {2} do {1} znaków.")]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła nie są takie same.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            SelectedPlatforms ??= new List<string>();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            SelectedPlatforms ??= new List<string>();

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Nick = Input.Nick;

                if (await _userManager.Users.AnyAsync(u => u.Nick == Input.Nick))
                {
                    ModelState.AddModelError("Input.Nick", "Ten nick jest już zajęty.");
                    return Page();
                }

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    if (SelectedPlatforms.Count > 0)
                    {
                        user.StreamingPlatforms = string.Join(";", SelectedPlatforms.Distinct());
                        await _userManager.UpdateAsync(user);
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        null,
                        new { area = "Identity", userId, code, returnUrl },
                        Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            return Activator.CreateInstance<ApplicationUser>();
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("User store does not support email.");

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
