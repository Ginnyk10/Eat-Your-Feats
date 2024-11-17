/*
Name: Dylan Sailors
Date Created: 11/10/2024
Date Revised: 11/10/2024
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
        private readonly TodoService _todoService;
        private readonly UserService _userService;

        // Bound properties to hold the list of tasks and selected task IDs
        [BindProperty]
        public List<Todo> Tasks { get; set; }    // List of tasks retrieved from the database
        [BindProperty]
        public List<string> SelectedTaskIds { get; set; } // List of selected task IDs

        // Property to hold the total earned points
        public int EarnedPoints { get; set; }

        // Constructor to initialize the TaskService and UserService instances, injected via dependency injection
        public ManageToDoModel(TodoService todoService, UserService userService)
        {
            _todoService = todoService;
            _userService = userService;
            Tasks = new List<Todo>();
            SelectedTaskIds = new List<string>();
        }

        // Handles the initial page load; retrieves tasks from the database for the logged-in user
        public async Task OnGetAsync()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
            Tasks = await _todoService.GetTasksByUsernameAsync(username);
            EarnedPoints = await _userService.GetUserPointsAsync(username); // Get the user's current points
        }

        // Handles the submission of completed tasks; calculates earned points and updates the database
        public async Task<IActionResult> OnPostMarkCompletedAsync()
        {
            if (SelectedTaskIds.Count > 0)
            {
                var completedTasks = await _todoService.GetTasksByIdsAsync(SelectedTaskIds);
                EarnedPoints = 0;

                foreach (var task in completedTasks)
                {
                    EarnedPoints += task.Points;
                }

                var username = User.FindFirstValue(ClaimTypes.Name); // Get the logged-in user's username
                await _userService.UpdateUserPointsAsync(username, EarnedPoints); // Update the user's points
                await _todoService.DeleteTasksByIdsAsync(SelectedTaskIds); // Delete the completed tasks
            }

            return RedirectToPage(); // Refresh the page to show updated tasks and points
        }
    }
}
