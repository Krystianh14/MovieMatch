// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MovieMatch.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        public string Nick { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Nick")]
            public string Nick { get; set; }

            [Phone]
            [Required(ErrorMessage = "Wzór: +48 123 456 789")]
            [Display(Name = "Phone number")]
            [RegularExpression(@"^(\+\d{1,3}\s?)?\d{3}[-\s]?\d{3}[-\s]?\d{3}$",
                    ErrorMessage = "Wzór: +[nr kierunkowy] 123 456 789. Nr kierunkowy = 1–3 cyfry.")]
            public string PhoneNumber { get; set; }
        }

        private string FormatPhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var normalized = input.Replace(" ", "");

            if (!normalized.StartsWith("+"))
                return normalized;

            var digitsLen = normalized.Length - 1;

            int prefixDigits;

            if (digitsLen == 10)
                prefixDigits = 1;
            else if (digitsLen == 11)
                prefixDigits = 2;
            else if (digitsLen == 12)
                prefixDigits = 3;
            else
                return normalized;

            for (int i = 1; i < normalized.Length; i++)
            {
                if (!char.IsDigit(normalized[i]))
                    return normalized;
            }

            var prefixEnd = 1 + prefixDigits;
            var prefix = normalized[..prefixEnd];
            var number = normalized[prefixEnd..];

            if (number.Length != 9)
                return normalized;

            return $"{prefix} {number[..3]} {number[3..6]} {number[6..]}";
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            Nick = user.Nick;

            Input = new InputModel
            {
                PhoneNumber = string.IsNullOrEmpty(phoneNumber) ? string.Empty : phoneNumber,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var formattedPhone = FormatPhoneNumber(Input.PhoneNumber);
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, formattedPhone);

                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDeletePhoneAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, null);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to remove phone number.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your phone number has been removed.";
            return RedirectToPage();
        }

    }
}
