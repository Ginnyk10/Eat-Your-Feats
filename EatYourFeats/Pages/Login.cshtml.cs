/*
Name: Isabel Loney
Date Created: 11/7/2024
Date Revised: 11/8/2024
Purpose: Handles user login functionality by validating email and password input, checking if the user exists in the database, 
and signing in the user upon successful authentication. Displays appropriate error messages for invalid login attempts.

Preconditions: UserService instance must be properly initialized and injected, login form must provide non-null, non-empty values for Email and Password, User model must be correctly defined
Postconditions: user is authenticated and signed in if the email and password are valid, user is redirected to the Dashboard page upon successful login, rror messages are displayed for invalid login attempts
Error and exceptions: ArgumentNullException (thrown if the Email or Password properties are null)
Side effects: N/A
Invariants: _userService field is always initialized with a valid instance
Other faults: N/A
*/


// Required namespaces for services, authentication, and Razor Pages functionality
using EatYourFeats.Services;                // Provides access to UserService for database operations
using EatYourFeats.Utilities;               // Contains utility methods, such as PasswordHelper for password verification
using Microsoft.AspNetCore.Authentication;  // Enables authentication functionalities
using Microsoft.AspNetCore.Authentication.Cookies;  // Defines cookie-based authentication scheme
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims

namespace EatYourFeats.Pages
{
    // Class for managing the Login page and handling login operations
    public class LoginModel : PageModel
    {
        // Instance of UserService used for user-related database operations
        private readonly UserService _userService;

        // Bound properties to hold email and password input from the login form
        [BindProperty]
        public string Email { get; set; }    // User-provided email address for login
        [BindProperty]
        public string Password { get; set; } // User-provided password for login

        // Constructor to initialize the UserService instance, injected via dependency injection
        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        // Handles login submission; verifies user credentials and initiates authentication if valid
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if Email or Password fields are empty and return error if so
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Email and password cannot be empty.");
                return Page(); // Return the same page with error messages
            }

            // Attempt to retrieve the user by email from the database
            var user = await _userService.GetUserByEmailAsync(Email);
            if (user == null) // If no user is found, display an error message
            {
                ModelState.AddModelError(string.Empty, "No account associated with this email.");
                return Page();
            }

            // Verify that the provided password matches the stored password hash
            if (!PasswordHelper.VerifyPassword(Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Incorrect password."); // Error for incorrect password
                return Page();
            }

            // If credentials are valid, create a list of claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username), // User's username
                new Claim(ClaimTypes.Email, user.Email)    // User's email
            };

            // Create a ClaimsIdentity using the claims and cookie authentication scheme
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user with the created ClaimsPrincipal, which contains their identity information
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Redirect to the home page after successful login
            return RedirectToPage("/Dashboard");
        }
    }
}
