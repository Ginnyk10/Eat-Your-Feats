using EatYourFeats.Models;
using EatYourFeats.Services; // Add necessary services
using Microsoft.AspNetCore.Mvc; // Required for Razor Pages
using Microsoft.AspNetCore.Mvc.RazorPages; // Razor Pages support
using System.Security.Claims; // For user claims

namespace EatYourFeats.Pages
{
    public class FinalGameScreenModel : PageModel
    {
        private readonly GameService _gameService;
        private readonly UserService _userService;

        public string RemainingTime { get; set; }
        public int FinalScore { get; set; }
        public string username { get; set; }
        public Game CurrentGame { get; set; }
        public int EarnedPoints { get; set; }



        public FinalGameScreenModel(GameService gameService, UserService userService)
        {
            _gameService = gameService;
            _userService = userService;
        }

        public async Task OnGetAsync()
        {
            username = User.FindFirstValue(ClaimTypes.Name);
            var currentGame = await _gameService.GetGameByUsernameAsync(username);

            // Calculate remaining time
            var timeLeft = currentGame.EndTime - DateTime.UtcNow;
            RemainingTime = timeLeft > TimeSpan.Zero
                ? $"{timeLeft.Days}d {timeLeft.Hours}h {timeLeft.Minutes}m {timeLeft.Seconds}s"
                : "0d 0h 0m 0s";

            FinalScore = currentGame.Score;
        }

        public async Task<IActionResult> OnPostReturnToDashboardAsync()
        {
            username = User.FindFirstValue(ClaimTypes.Name);
            CurrentGame = await _gameService.GetGameByUsernameAsync(username);

            if (string.IsNullOrEmpty(username))
            {
                // Log or handle missing username
                throw new NullReferenceException("Username is null or empty.");
            }

            var currentGame = await _gameService.GetGameByUsernameAsync(username);

            if (currentGame == null)
            {
                // Log or handle missing game
                throw new NullReferenceException($"No game found for user {username}.");
            }

            await _gameService.DeleteGameByIdAsync(currentGame.Id.ToString());
            if (CurrentGame.Score > EarnedPoints)
            {
                await _userService.UpdateUserPointsAsync(username, CurrentGame.Score);
            }
            return RedirectToPage("/Dashboard");
        }

    }
}
