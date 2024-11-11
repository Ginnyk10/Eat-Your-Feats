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


// Required namespaces for services, MongoDB, and Razor Pages functionality
using EatYourFeats.Services;                // Provides access to TaskService and UserService for database operations
using EatYourFeats.Models;                  // Task and User models
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support
using System.Collections.Generic;           // Provides List<T> and other collection classes
using System.Security.Claims;               // Provides classes for managing and handling user claims
using System.Threading.Tasks;               // Provides Task-based asynchronous programming

namespace EatYourFeats.Pages
{
    // Class for managing the ToDo page and handling task operations
    public class ManageToDoModel : PageModel
    {
        // Instances of TaskService and UserService used for database operations
        private readonly TaskService _taskService;
        private readonly UserService _userService;

        // Bound properties to hold the list of tasks and selected task IDs
        [BindProperty]
        public List<TaskItem> Tasks { get; set; }    // List of tasks retrieved from the database
        [BindProperty]
        public List<string> SelectedTaskIds { get; set; } // List of selected task IDs

        // Property to hold the total earned points
        public int EarnedPoints { get; set; }

        // Constructor to initialize the TaskService and UserService instances, injected via dependency injection
        public ManageToDoModel(TaskService taskService, UserService userService)
        {
            _taskService = taskService;
            _userService = userService;
            Tasks = new List<TaskItem>();
            SelectedTaskIds = new List<string>();
        }

        // Handles the initial page load; retrieves tasks from the database for the logged-in user
        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            Tasks = await _taskService.GetTasksByUsernameAsync(username);
            EarnedPoints = await _userService.GetUserPointsAsync(username); // Get the user's current points
        }

        // Handles the submission of completed tasks; calculates earned points and updates the database
        public async Task<IActionResult> OnPostMarkCompletedAsync()
        {
            if (SelectedTaskIds.Count > 0)
            {
                var completedTasks = await _taskService.GetTasksByIdsAsync(SelectedTaskIds);
                EarnedPoints = 0;

                foreach (var task in completedTasks)
                {
                    EarnedPoints += task.Points;
                }

                var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
                await _userService.UpdateUserPointsAsync(username, EarnedPoints); // Update the user's points
                await _taskService.DeleteTasksByIdsAsync(SelectedTaskIds); // Delete the completed tasks
            }

            return RedirectToPage(); // Refresh the page to show updated tasks and points
        }
    }
}
