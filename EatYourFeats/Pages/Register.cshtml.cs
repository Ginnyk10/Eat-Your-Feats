/*
Name: Isabel Loney
Date Created: 11/7/2024
Date Revised: 11/8/2024
Purpose: Handles user registration by creating new accounts, validating inputs, checking for existing accounts, and signing in users automatically.

Preconditions: MongoDBService and UserService instances properly initialized and injected, User model must be correctly defined
Postconditions: new user account is created and stored in the MongoDB database if the inputs are valid and the email/username are not already in use, user is automatically signed in and redirected to the Dashboard page upon successful registration
Error and exceptions: ArgumentNullException (Email, Username, or Password properties are null), 
Side effects: N/A
Invariants: _mongoDBService and _userService fields are always initialized with valid instances, OnPostAsync method always returns an IActionResult
Other faults: N/A
*/

// Required namespaces for accessing models, services, and utilities
using EatYourFeats.Models;                    // User model for creating user instances
using EatYourFeats.Services;                  // Services for database operations
using EatYourFeats.Utilities;                 // Utility for password hashing
using Microsoft.AspNetCore.Authentication;    // Authentication-related functionalities
using Microsoft.AspNetCore.Authentication.Cookies; // Cookie-based authentication scheme
using Microsoft.AspNetCore.Mvc;               // Provides attributes and controller functionalities
using Microsoft.AspNetCore.Mvc.RazorPages;    // Required for Razor Pages functionality
using System.Security.Claims;                 // Used for managing user claims

namespace EatYourFeats.Pages
{
    // Manages the Register page, including validation, user creation, and login operations
    public class RegisterModel : PageModel
    {
        // Injected services for database operations and user-specific operations
        private readonly MongoDBService _mongoDBService; // MongoDB connection and operations
        private readonly UserService _userService;       // Service for handling user-specific actions

        // Constructor to initialize services via dependency injection
        public RegisterModel(MongoDBService mongoDBService, UserService userService)
        {
            _mongoDBService = mongoDBService;
            _userService = userService;
        }

        // Bound properties to hold input values from the registration form
        [BindProperty]
        public string Username { get; set; }    // Username provided by the user
        [BindProperty]
        public string Email { get; set; }       // Email address provided by the user
        [BindProperty]
        public string Password { get; set; }    // Password provided by the user

        // Processes the registration form submission
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if all inputs meet validation requirements
            if (ModelState.IsValid)
            {
                // Ensure all required fields are filled
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    ModelState.AddModelError(string.Empty, "Username, email, and password cannot be empty.");
                    return Page(); // Return the same page with error messages
                }

                // Check if an account already exists with the given email
                var existingUser = await _userService.GetUserByEmailAsync(Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already registered.");
                    return Page(); // Display error if email is already in use
                }

                // Check if an account already exists with the given username
                var existingUserByUsername = await _userService.GetUserByUsernameAsync(Username);
                if (existingUserByUsername != null)
                {
                    ModelState.AddModelError(string.Empty, "This username is already in use.");
                    return Page(); // Display error if username is already taken
                }

                // Create a new user instance with the provided data
                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    PasswordHash = PasswordHelper.HashPassword(Password) // Hash the password before storing
                };

                // Add the new user to the database
                await _userService.CreateUserAsync(user);

                // Generate claims for the new user to support authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username), // Claim for the user's name
                    new Claim(ClaimTypes.Email, Email)    // Claim for the user's email
                };

                // Create an identity and sign in the user automatically after registration
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Redirect to the home page after successful registration and login
                return RedirectToPage("/Dashboard");
            }
            // If validation fails, return the page with validation errors
            return Page();
        }
    }
}
