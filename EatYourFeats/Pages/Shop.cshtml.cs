/*
 * Prologue
Name: Dylan Sailors, Anakha Krishna, Jackson Wunderlich
Date Created: 11/10/2024
Date Revised: 11/24/2024
Purpose: Handles the shop, where users can buy items and store them in their inventory

Preconditions: TaskService instance must be properly initialized and injected, username must provide non-null, non-empty values
Postconditions: items are displayed with their costs. there are selection boxes next to each item that can be selected then the user can buy the selected items which adds them to their
game's inventory and subtracts the spent number of points from their score
Error and exceptions: None (no way to bring up an error here as the code doesn't even allow an error to occur. if the buy items button is hit with no items selected the page just ignores it)
Side effects: N/A
Invariants: _taskService field is always initialized with a valid instance
Other faults: N/A
*/

// Required namespaces for services, MongoDB, and Razor Pages functionality
using EatYourFeats.Models;                  // Task, User, and ShopItem models
using EatYourFeats.Services;                // Provides access to multiple services for database operations
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims

namespace EatYourFeats.Pages
{
    // Class for managing the Shop page and handling shop operations
    public class ShopModel : PageModel
    {
        // Instances of TodoService, UserService, and GameService used for database operations
        private readonly TodoService _todoService;
        private readonly UserService _userService;
        private readonly GameService _gameService;
        private readonly InventoryService _inventoryService;

        // Bound properties to hold the list of all items and selected items
        [BindProperty]
        public List<ShopItem> Items { get; set; } // List of items created by GameService
        [BindProperty]
        public List<string> SelectedItems { get; set; } // List of selected item names

        public Game CurrentGame { get; set; } // current game
        public int SpentPoints { get; set; } // amount of points for items bought

        // Constructor to initialize the service instances, injected via dependency injection
        public ShopModel(TodoService todoService, UserService userService, GameService gameService, InventoryService inventoryService)
        {
            _todoService = todoService;
            _userService = userService;
            Items = new List<ShopItem>();
            SelectedItems = new List<string>();
            _gameService = gameService;
            _inventoryService = inventoryService;
        }

        // Handles the initial page load; retrieves possible item purchases
        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            CurrentGame = await _gameService.GetGameByUsernameAsync(username); // get current game
            Items = await _gameService.CreateShop(); // create the list of buyable items
        }

        // Handles the submission of bought items, calculates spent points and adds new items to the database
        public async Task<IActionResult> OnPostBuyAsync()
        {
            if (SelectedItems.Count > 0) // if there are selected items,
            {
                Items = await _gameService.CreateShop();

                var boughtItems = new List<ShopItem>();

                // loop through each item in the total list of items
                foreach (var item in Items)
                {
                    // loop through the list of selected items
                    foreach (var selectedItem in SelectedItems)
                    {
                        // if the selected item is found in the list, 
                        if (selectedItem == item.ItemName)
                        {
                            boughtItems.Add(item); // add it to the list of bought items
                        }
                    }
                }

                // gets the current game
                var username = User.FindFirstValue(ClaimTypes.Name);
                CurrentGame = await _gameService.GetGameByUsernameAsync(username);

                int currentPoints = CurrentGame.Score; // initialize point total
                int pointsToSpend = 0;
                var inventoryItems = new List<InventoryItem>();

                foreach (var item in boughtItems) // for each selected item
                {
                    pointsToSpend -= item.ItemPrice; // subtract points spent per item

                    // create a new InventoryItem from the existing item and add it to the list
                    var newInventoryItem = new InventoryItem
                    {
                        ItemName = item.ItemName,
                        GameId = CurrentGame.Id
                    };
                    inventoryItems.Add(newInventoryItem);
                }

                // if the total number of points - spent points is not negative, 
                if ((currentPoints + pointsToSpend) >= 0)
                {
                    await _gameService.UpdateGameScoreAsync(CurrentGame.Id.ToString(), pointsToSpend); // update game score

                    foreach (var item in inventoryItems)
                    {
                        await _inventoryService.AddInventoryItemAsync(item);
                    }
                } 
                else
                {
                    ModelState.AddModelError(string.Empty, "Not enough points."); // create an error stating that there are not enough points to make the purchase
                    return Page(); // return the same page with error messages
                }
                
                TempData["SpentPoints"] = currentPoints + pointsToSpend; // set tempdata of points remaining for alert message
            }

            return RedirectToPage(); // refresh the page to show updated tasks and points
        }
    }
}
