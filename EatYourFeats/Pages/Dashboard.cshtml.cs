// 4GL for Dashboard page
/*  Programmers: Ginny Ke
    Date created: 11 / 9 / 24
    Date modified: 11 / 10 / 24
    Purpose: provides necessary access to various AspNetCore libraries. Connects Logout method. Please note only the public async method was added to the template file
             the rest of the code was a part of the original template code

   Preconditions: User must be authenticated with a valid cookie; authentication services must be configured. 
   Postconditions: User is logged out, authentication cookie is removed, and they are redirected to the Splash page. 
   Error and Exceptions: May throw `ArgumentNullException` or errors related to `HttpContext.SignOutAsync()` if authentication is misconfigured. 
   Side Effects: Removes authentication cookie and redirects user to the Splash page. 
   Invariants: `_logger` is always initialized, `HttpContext.SignOutAsync()` is always called during logout. 
   Other Faults: N/A */
   
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