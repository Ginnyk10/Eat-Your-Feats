/*
 * Prologue
Name: Dylan Sailors, Anakha Krishna, Isabel Loney
Date Created: 11/10/2024
Date Revised: 12/6/2024
Purpose: Handles the todo list in the sense that it takes the list from the user from the database, then prints out the tasks with the assigned point values. Once that happens, it gives the user the option to 
check off the tasks they want to mark as complete then prints how many points the user has.

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
using EatYourFeats.Services;                // Provides access to services for database operations
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims

namespace EatYourFeats.Pages
{
    public class ManageToDoModel : PageModel
    {
        private readonly TodoService _todoService;
        private readonly UserService _userService;
        private readonly GameService _gameService;
        private readonly InventoryService _inventoryService; // Inject InventoryService

        [BindProperty]
        public List<Todo> Tasks { get; set; }
        [BindProperty]
        public List<string> SelectedTaskIds { get; set; }
        public int EarnedPoints { get; set; }
        public Game CurrentGame { get; set; }
        public int CompletedTaskPoints { get; set; }
        [BindProperty]
        public InventoryItem EquippedItem { get; set; }

        // Constructor with InventoryService
        public ManageToDoModel(TodoService todoService, UserService userService, GameService gameService, InventoryService inventoryService)
        {
            _todoService = todoService;
            _userService = userService;
            _gameService = gameService;
            _inventoryService = inventoryService; // Initialize InventoryService
            Tasks = new List<Todo>();
            SelectedTaskIds = new List<string>();
        }

        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            Tasks = await _todoService.GetTasksByUsernameAsync(username);
            CurrentGame = await _gameService.GetGameByUsernameAsync(username);
            EarnedPoints = await _userService.GetUserPointsAsync(username);
            EquippedItem = await _inventoryService.GetEquippedItemAsync(CurrentGame.Id);
        }

        public IActionResult OnGetOpenInventory()
        {
            return RedirectToPage("/Inventory");
        }

        public async Task<IActionResult> OnPostMarkCompletedAsync()
        {
            if (SelectedTaskIds.Count > 0)
            {
                var completedTasks = await _todoService.GetTasksByIdsAsync(SelectedTaskIds);
                int totalGamePoints = 0;

                foreach (var task in completedTasks)
                {
                    totalGamePoints += task.Points;
                }


                var username = User.FindFirstValue(ClaimTypes.Name);
                CurrentGame = await _gameService.GetGameByUsernameAsync(username);
                EquippedItem = await _inventoryService.GetEquippedItemAsync(CurrentGame.Id);

                // Double points of highest task selected if water equipped
                if (EquippedItem?.ItemName == "Water")
                {
                    var highestPointTask = completedTasks.OrderByDescending(task => task.Points).First();
                    // Previous loop already added points once, so add again
                    totalGamePoints += highestPointTask.Points;

                    // delete equipped water from inventory
                    await _inventoryService.DeleteItemByIdAsync(EquippedItem.Id.ToString());
                }

                // Triple Highest Point
                if (EquippedItem?.ItemName == "Sketchy Catabolic Supplement")
                {
                    // check if item is Sketchy Catabolic Supplement; if so, check if expired
                    if ((DateTime.Now.ToUniversalTime() - EquippedItem.TimeEquipped >= TimeSpan.FromHours(12)))
                    {
                        // if expired, remove the item
                        await _inventoryService.DeleteItemByIdAsync(EquippedItem.Id.ToString());
                    }
                    else
                    {
                        //item not expired, use as normal
                        var allTasks = await _todoService.GetTasksByGameIdAsync(CurrentGame.Id.ToString());
                        var highestPoints = allTasks.OrderByDescending(task => task.Points).First().Points;

                        // Previous loop already added points once, so additional points
                        foreach (var task in completedTasks)
                        {
                            if (task.Points == highestPoints)
                            {
                                totalGamePoints += 2 * highestPoints;
                                // delete equipped item from inventory
                                await _inventoryService.DeleteItemByIdAsync(EquippedItem.Id.ToString());
                                break;
                            }
                        }
                    }
                }

                await _gameService.UpdateGameScoreAsync(CurrentGame.Id.ToString(), totalGamePoints);
                await _todoService.DeleteTasksByIdsAsync(SelectedTaskIds);

                TempData["CompletedTaskPoints"] = totalGamePoints;

                var remainingTasks = await _todoService.GetTasksByGameIdAsync(CurrentGame.Id.ToString());
                if (remainingTasks.Count == 0 || CurrentGame.EndTime <= DateTime.UtcNow)
                {
                    return RedirectToPage("/FinalGameScreen");
                }
            }

            return RedirectToPage();
        }
    }
}

