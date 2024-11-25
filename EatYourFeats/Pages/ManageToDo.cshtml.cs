/*
 * Prologue
Name: Dylan Sailors, Anakha Krishna
Date Created: 11/10/2024
Date Revised: 11/24/2024
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
using EatYourFeats.Services;                // Provides access to TaskService and UserService for database operations
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Security.Claims;               // Provides classes for managing and handling user claims

namespace EatYourFeats.Pages
{
    // Class for managing the ToDo page and handling task operations
    public class ManageToDoModel : PageModel
    {
        // Instances of TaskService and UserService used for database operations
        private readonly TodoService _todoService;
        private readonly UserService _userService;
        private readonly GameService _gameService;

        // Bound properties to hold the list of tasks and selected task IDs
        [BindProperty]
        public List<Todo> Tasks { get; set; }    // List of tasks retrieved from the database
        [BindProperty]
        public List<string> SelectedTaskIds { get; set; } // List of selected task IDs

        // Property to hold the total earned points
        public int EarnedPoints { get; set; }
        public Game CurrentGame { get; set; } // current game
        public int CompletedTaskPoints { get; set; } // amount of points for tasks that were just completed

        // Constructor to initialize the TaskService and UserService instances, injected via dependency injection
        public ManageToDoModel(TodoService todoService, UserService userService, GameService gameService)
        {
            _todoService = todoService;
            _userService = userService;
            Tasks = new List<Todo>();
            SelectedTaskIds = new List<string>();
            _gameService = gameService;
        }

        [BindProperty]
        public string SelectedItemFromInventory { get; set; } // Stores the selected inventory item

        // Handles the initial page load; retrieves tasks from the database for the logged-in user
        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            Tasks = await _todoService.GetTasksByUsernameAsync(username); // get their tasks
            CurrentGame = await _gameService.GetGameByUsernameAsync(username); // get current game
            EarnedPoints = await _userService.GetUserPointsAsync(username); // get the user's highest score
         // Check if an item has been selected from inventory
            if (TempData["SelectedItem"] != null)
            {
                SelectedItemFromInventory = TempData["SelectedItem"].ToString();
            }
        }
        public IActionResult OnGetOpenInventory()
        {
            // Redirect to the Inventory page when the button is clicked
            return RedirectToPage("/Inventory");
        }

        // Handles the submission of completed tasks; calculates earned points and updates the database
        public async Task<IActionResult> OnPostMarkCompletedAsync()
        {
            if (SelectedTaskIds.Count > 0) // if selected tasks,
            {
                var completedTasks = await _todoService.GetTasksByIdsAsync(SelectedTaskIds); // get tasks
                int totalGamePoints = 0; // initialize point total

                foreach (var task in completedTasks) // for each selected task
                {
                    totalGamePoints += task.Points; // add pts for each task to total game score
                }

                var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
                CurrentGame = await _gameService.GetGameByUsernameAsync(username); // get current game

                await _gameService.UpdateGameScoreAsync(CurrentGame.Id.ToString(), totalGamePoints); // update game score
                await _todoService.DeleteTasksByIdsAsync(SelectedTaskIds); // Delete the completed tasks

                TempData["CompletedTaskPoints"] = totalGamePoints; // set tempdata of points that were just completed for alert message

                var remainingTasks = await _todoService.GetTasksByGameIdAsync(CurrentGame.Id.ToString()); // get remaining tasks
                if (remainingTasks.Count == 0 || CurrentGame.EndTime <= DateTime.UtcNow) // if remaining tasks is 0 or the 3 day timer is up,
                {
                    if (CurrentGame.Score > EarnedPoints) // if the game's final score is higher than the user's highest score (EarnedPoints)
                    {
                        await _userService.UpdateUserPointsAsync(username, CurrentGame.Score); // update the user's highest score to be the current game score
                    }

                    await _gameService.DeleteGameByIdAsync(CurrentGame.Id.ToString()); // delete the game

                    TempData["CompletedTaskPoints"] = 0; // reset tempdata

                    return RedirectToPage("/FinalGameScreen"); // redirect to final game screen
                }
            }

            return RedirectToPage(); // Refresh the page to show updated tasks and points
        }
    }
}
