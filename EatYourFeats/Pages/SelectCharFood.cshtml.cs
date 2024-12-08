/*
 * Prologue
Name: Isabel Loney, Jackson Wunderlich, Anakha Krishna
Date Created: 12/7/2024
Date Revised: 12/7/2024
Purpose: allows a user to select a desired character and food

Preconditions: MongoDBService and TodoService instances properly initialized, Game model correctly defined
Postconditions: current game is updated with chosen character and food
Error and exceptions: None
Side effects: N/A
Invariants: _todoService is always a valid instance
Other faults: N/A
*/


// Required namespaces for services, authentication, and Razor Pages functionality
using EatYourFeats.Models;                  // Game model for getting current game
using EatYourFeats.Services;                // Provides access to GameService for database operations
using Microsoft.AspNetCore.Mvc;             // Provides attributes and functionality for controllers
using Microsoft.AspNetCore.Mvc.RazorPages;  // Required for Razor Pages support

namespace EatYourFeats.Pages {
    // manages selecting character and food
    public class SelectionModel : PageModel {
        // creates new instances of GameService for use in this file
        private readonly GameService _gameService;
        public Game CurrentGame { get; set; }
        [BindProperty]
        public string ChosenCharacter { get; set; }
        [BindProperty]
        public string ChosenFood { get; set; }
        [BindProperty]
        public List<string> CharOptions { get; set; } = new List<string> { "char1", "char2", "char3" };
        [BindProperty]
        public List<string> FoodOptions { get; set; } = new List<string> { "burger", "melon", "pizza", "soup", "sushi" };

        // initializes GameService
        public SelectionModel(TodoService todoService, GameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<IActionResult> OnPostStartGameAsync()
        {
            // get the current game
            CurrentGame = await _gameService.GetGameByUsernameAsync(User.Identity.Name);

            // update the character and food selections for the user's current game
            await _gameService.UpdateCharFoodAsync(CurrentGame.Id.ToString(), ChosenCharacter, ChosenFood);

            // redirect to the in-progress game page
            return RedirectToPage("/ManageToDo");
        }
    }
}
