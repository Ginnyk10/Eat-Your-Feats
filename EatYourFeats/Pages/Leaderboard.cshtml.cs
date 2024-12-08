/*
 * Prologue
Name: Dylan Sailors, Isabel Loney, Jackson Wunderlich
Date Created: 12/7/2024
Date Revised: 12/7/2024
Purpose: allows a user to view the leaderboard of all users

Preconditions: user is logged in
Postconditions: creates an ordered list of all users and displays it
Error and exceptions: if no users exist, throws an error (should never happen since you have to log in to get to this page)
Side effects: N/A
Invariants: _gameService field is always initialized with a valid instance
Other faults: N/A
*/

// Required namespaces for services, MongoDB, and Razor Pages functionality
using EatYourFeats.Models;                  // InventoryItem, Game models
using EatYourFeats.Services;                // Provides access to InventoryService
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims

namespace EatYourFeats.Pages
{
    public class LeaderboardModel : PageModel
    {
        private readonly UserService _userService;

        public LeaderboardModel(UserService userService, GameService gameService)
        {
            _userService = userService;
        }

        public List<User> Users { get; set; } // Holds the current game
        public List<User> SortedUsers { get; set; } // Holds the current game

        // Gets a list of all users and orders it on load
        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            Users = await _userService.GetAllUsersAsync(); // Get current game
            SortedUsers = Users.OrderByDescending(user => user.Points).ToList();
        }
    }
}
