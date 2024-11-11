// 4GL for Dashboard page

using Microsoft.AspNetCore.Mvc.ActionConstraints;
using EatYourFeats.Services;                // Provides access to UserService for database operations
using EatYourFeats.Utilities;               // Contains utility methods, such as PasswordHelper for password verification
using Microsoft.AspNetCore.Authentication;  // Enables authentication functionalities
using Microsoft.AspNetCore.Authentication.Cookies;  // Defines cookie-based authentication scheme
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims


namespace EatYourFeats.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        //method to log user out and return to splash page
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            //sign user out and remove cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Splash");
        }
    }
}
