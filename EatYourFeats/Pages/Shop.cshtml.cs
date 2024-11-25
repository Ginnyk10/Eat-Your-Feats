/*
 * Prologue
Name: Dylan Sailors, Anakha Krishna, Jackson Wunderlich
Date Created: 11/10/2024
Date Revised: 11/24/2024
Purpose: Handles the shop, where users can buy items and store them in their inventory

Preconditions: TaskService instance must be properly initialized and injected, username must provide non-null, non-empty values, Task Item model must be correctly defined
Postconditions: tasks are displayed with their point values. there are selection boxes next to each task that can be selected then the user can mark the selected tasks as completed which removes the task from
the database and the list while also adding the respective points from the task to the user's point total
Error and exceptions: None (no way to bring up an error here as the code doesn't even allow an error to occur. if the mark tasks as completed button is hit with no tasks selected the page just ignores it)
Side effects: N/A
Invariants: _taskService field is always initialized with a valid instance
Other faults: N/A
*/

// Required namespaces for services, MongoDB, and Razor Pages functionality
using EatYourFeats.Models;                  // Task and User models
using EatYourFeats.Services;                // Provides access to TaskService and UserService for database operations
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims

namespace EatYourFeats.Pages
{
    // Class for managing the ToDo page and handling task operations
    public class ShopModel : PageModel
    {
        // Instances of TaskService and UserService used for database operations
        private readonly TodoService _todoService;
        private readonly UserService _userService;
        private readonly GameService _gameService;

        // Bound properties to hold the list of tasks and selected task IDs
        [BindProperty]
        public List<ShopItem> Items { get; set; }    // List of tasks retrieved from the database
        [BindProperty]
        public List<string> SelectedItems { get; set; } // List of selected task IDs

        // Property to hold the total earned points
        public int EarnedPoints { get; set; }
        public Game CurrentGame { get; set; } // current game
        public int SpentPoints { get; set; } // amount of points for items bought

        // Constructor to initialize the TaskService and UserService instances, injected via dependency injection
        public ShopModel(TodoService todoService, UserService userService, GameService gameService)
        {
            _todoService = todoService;
            _userService = userService;
            Items = new List<ShopItem>();
            SelectedItems = new List<string>();
            _gameService = gameService;
        }

        // Handles the initial page load; retrieves tasks from the database for the logged-in user
        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            CurrentGame = await _gameService.GetGameByUsernameAsync(username); // get current game
            EarnedPoints = await _userService.GetUserPointsAsync(username); // get the user's highest score
            Items = await _gameService.CreateShop();
        }

        // Handles the submission of completed tasks; calculates earned points and updates the database
        public async Task<IActionResult> OnPostBuyAsync()
        {
            if (SelectedItems.Count > 0) // if there are selected items,
            {
                Items = await _gameService.CreateShop();

                var boughtItems = new List<ShopItem>();
                foreach (var item in Items)
                {
                    if (SelectedItems.Contains(item.ItemName))
                    {
                        boughtItems.Add(item);
                    }
                }
                var username = User.FindFirstValue(ClaimTypes.Name);
                CurrentGame = await _gameService.GetGameByUsernameAsync(username);

                int currentPoints = CurrentGame.Score; // initialize point total
                int pointsToSpend = 0;

                foreach (var item in boughtItems) // for each selected task
                {
                    pointsToSpend -= item.ItemPrice; // add pts for each task to total game score
                }

                if ((currentPoints - pointsToSpend) > 0)
                {
                    await _gameService.UpdateGameScoreAsync(CurrentGame.Id.ToString(), pointsToSpend); // update game score
                } else
                {
                    ModelState.AddModelError(string.Empty, "Not enough points. " + pointsToSpend);
                    return Page(); // Return the same page with error messages
                }
                
                TempData["SpentPoints"] = currentPoints - pointsToSpend; // set tempdata of points that were just completed for alert message
            }

            return RedirectToPage(); // Refresh the page to show updated tasks and points
        }
    }
}
