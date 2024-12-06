/*
 * Prologue
Name: Isabel Loney, Jackson Wunderlich, Anakha Krishna
Date Created: 12/6/2024
Date Revised: 12/6/2024
Purpose: implements functionality for the coupon item where users can add a new task to their list mid-game

Preconditions: MongoDBService and TodoService instances properly initialized, Todo model correctly defined
Postconditions: new task is created for the given game when "Add Task" is clicked
Error and exceptions: ArgumentNullException can occur if name or points are null
Side effects: N/A
Invariants: _todoService is always a valid instance
Other faults: N/A
*/


// Required namespaces for services, authentication, and Razor Pages functionality
using EatYourFeats.Models;                  // Todo model for creating task instances
using EatYourFeats.Services;                // Provides access to TodoService for database operations
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support

namespace EatYourFeats.Pages {
    // manages adding tasks to a list
    public class AddCouponTaskModel : PageModel {
        // creates new instances of TodoService for use in this file
        private readonly TodoService _todoService;
        private readonly GameService _gameService;
        public List<Todo> TaskList { get; set; }

        // creates bound properties for task name and point value
        [BindProperty]
        public string TaskName { get; set; }
        [BindProperty]
        public int TaskPoints { get; set; }

        // initializes TodoService
        public AddCouponTaskModel(TodoService todoService, GameService gameService)
        {
            _todoService = todoService;
            _gameService = gameService;
        }

        // retrieves a user's tasks on GET
        public async Task OnGetAsync()
        {
            TaskList = await _todoService.GetTasksByUsernameAsync(User.Identity.Name);
        }

        // handles how the new task is added when the button is clicked
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

            // return to game page once new task is added
            return RedirectToPage("/ManageToDo");
        }
    }
}
