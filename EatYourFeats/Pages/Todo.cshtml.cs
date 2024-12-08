/*
 * Prologue
Name: Isabel Loney, Jackson Wunderlich, Anakha Krishna
Date Created: 11/10/2024
Date Revised: 12/7/2024
Purpose: implements functionality where users can add a task to their list with a name and points, and view their task list while creating it

Preconditions: MongoDBService and TodoService instances properly initialized, Todo model correctly defined
Postconditions: task list specific to user is retrieved from database, tasks are created when the submit task button is pressed
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
    public class TodoModel : PageModel {
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
        public TodoModel(TodoService todoService, GameService gameService)
        {
            _todoService = todoService;
            _gameService = gameService;
        }

        // retrieves a user's tasks on GET
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

            // return the same page to the user after adding task
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmListAsync()
        {
            // get the user's tasks
            TaskList = await _todoService.GetTasksByUsernameAsync(User.Identity.Name);

            // create a new game
            var newGame = new Game
            {
                Username = User.Identity.Name,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(3),
                Score = 0,
                Character = "",
                Food = ""
            };

            // add the game to the database
            await _gameService.CreateGameAsync(newGame);

            // update each task with the new game's ID
            foreach (var task in TaskList)
            {
                task.GameId = newGame.Id;
                await _todoService.UpdateTaskAsync(task);
            }

            // redirect to the character and food selection page
            return RedirectToPage("/SelectCharFood");
        }
    }
}
