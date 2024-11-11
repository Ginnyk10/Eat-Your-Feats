/*
Name: Isabel Loney, Jackson Wunderlich
Date Created: 11/10/2024
Date Revised: 11/10/2024
Purpose: 

Preconditions: 
Postconditions: 
Error and exceptions: 
Side effects: 
Invariants: 
Other faults: 
*/


// Required namespaces for services, authentication, and Razor Pages functionality
using EatYourFeats.Models;                  // Todo model for creating task instances
using EatYourFeats.Services;                // Provides access to TodoService for database operations
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support

namespace EatYourFeats.Pages {
    // manages adding tasks to a list
    public class TodoModel : PageModel {
        // creates new instances of TodoService for use in this file
        private readonly TodoService _todoService;
        public List<Todo> TaskList { get; set; }

        // creates bound properties for task name and point value
        [BindProperty]
        public string TaskName { get; set; }
        [BindProperty]
        public int TaskPoints { get; set; }

        // initializes UserService and TodoService
        public TodoModel(TodoService todoService) {
            _todoService = todoService;
        }

        public async Task OnGetAsync()
        {
            TaskList = await _todoService.GetTasksByUsernameAsync(User.Identity.Name);
        }

        // handles how tasks are added when the button is clicked
        public async Task<IActionResult> OnPostAsync() {
            // if name or points are empty, add an error
            if (string.IsNullOrEmpty(TaskName) || string.IsNullOrEmpty(TaskPoints.ToString())) {
                ModelState.AddModelError(string.Empty, "Name and points cannot be empty.");
                return Page();
            }

            // creates a new task
            var new_task = new Todo {
                Username = User.Identity.Name,
                Name = TaskName,
                Points = TaskPoints
            };

            // adds task to database
            await _todoService.CreateTaskAsync(new_task);

            // gets tasks based on the current user's username
            TaskList = await _todoService.GetTasksByUsernameAsync(User.Identity.Name);

            return Page();
        }
    }
}
