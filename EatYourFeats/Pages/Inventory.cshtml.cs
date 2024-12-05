/*
 * Prologue
Name: Dylan Sailors
Date Created: 11/24/2024
Date Revised: 11/24/2024
Purpose: Handles the inventory where users can access the items bought from the shop

Preconditions: InventoryService instance must be properly initialized and injected, username must provide non-null, non-empty values
Postconditions: Purchased items are displayed with their picture. there are selection boxes next to each item that can be selected then the user can confirm-select the selected items which removes the item from their inventory and applies its effects (next sprint)
Error and exceptions: If no item is selected and you try to confirm selection, it throws an error
Side effects: N/A
Invariants: _inventoryService and _gameService field is always initialized with a valid instance
Other faults: N/A
*/

// Required namespaces for services, MongoDB, and Razor Pages functionality
using EatYourFeats.Models;                  // InventoryItem, Game models
using EatYourFeats.Services;                // Provides access to InventoryService
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims

namespace EatYourFeats.Pages
{
    public class InventoryModel : PageModel
    {
        private readonly InventoryService _inventoryService;
        private readonly GameService _gameService;

        public InventoryModel(InventoryService inventoryService, GameService gameService)
        {
            _inventoryService = inventoryService;
            _gameService = gameService;
        }

        public List<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>(); // Holds the list of items from the inventory

        [BindProperty]
        public string SelectedItemId { get; set; } // Holds the selected item ID

        public Game CurrentGame { get; set; } // Holds the current game

        // Loads the inventory items on page load
        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            CurrentGame = await _gameService.GetGameByUsernameAsync(username); // Get current game

            if (CurrentGame != null)
            {
                InventoryItems = await _inventoryService.GetItemsByIdAsync(new List<string> { CurrentGame.Id.ToString() });
            }
        }

        // Handles item selection confirmation
        public async Task<IActionResult> OnPostConfirmSelection()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            CurrentGame = await _gameService.GetGameByUsernameAsync(username); // Get current game
            if (!string.IsNullOrEmpty(SelectedItemId))
            {
                var selectedItem = await _inventoryService.GetItemByItemNameAsync(SelectedItemId); // Fetch the item
                if (selectedItem != null)
                {
                    // if there is a previously equipped item, it is lost
                    InventoryItem cur = await _inventoryService.GetEquippedItemAsync(CurrentGame.Id);
                    if (cur != null)
                    {
                        await _inventoryService.DeleteItemByIdAsync(cur.Id.ToString());
                    }
                    await _inventoryService.SetItemEquippedStatus(SelectedItemId, true); // set item as equipped
                    await _inventoryService.SetItemEquippedTime(SelectedItemId, DateTime.Now);
                    TempData["SelectedItem"] = selectedItem.ItemName; // Store the item name in TempData
                }

                return RedirectToPage("/ManageToDo"); // Redirect back to ManageToDo page
            }

            ModelState.AddModelError(string.Empty, "Please select an item.");
            return Page(); // Redisplay the page with error
        }

    }
}
