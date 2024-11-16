/*
Name: Anakha Krishna
Date Created: 11/16/2024
Date Revised: 11/16/2024
Purpose: 

Preconditions:
Postconditions:
Error and exceptions:
Side effects:
Invariants:
Other faults:
*/
using EatYourFeats.Models;
using MongoDB.Driver;

namespace EatYourFeats.Services
{
    public class GameService
    {
        private readonly IMongoCollection<Game> _games;

        public GameService(MongoDBService mongoDBService)
        {
            _games = mongoDBService.GetCollection<Game>("Games");
        }

        public async Task CreateGameAsync(Game game) =>
            await _games.InsertOneAsync(game);
    }
}
