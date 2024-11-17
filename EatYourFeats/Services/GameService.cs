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
using MongoDB.Bson;
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

        public async Task<Game> GetGameByUsernameAsync(string username)
        {
            var filter = Builders<Game>.Filter.Eq(g => g.Username, username);
            return await _games.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateGameScoreAsync(string gameId, int points)
        {
            var objectId = ObjectId.Parse(gameId);
            var update = Builders<Game>.Update.Inc(g => g.Score, points);
            await _games.UpdateOneAsync(g => g.Id == objectId, update);
        }

        public async Task<int> GetGameScoreAsync(string gameId)
        {
            var objectId = ObjectId.Parse(gameId);
            var filter = Builders<Game>.Filter.Eq(g => g.Id, objectId);
            var game = await _games.Find(filter).FirstOrDefaultAsync();

            return game?.Score ?? 0;
        }

        public async Task DeleteGameByIdAsync(string gameId)
        {
            var objectId = ObjectId.Parse(gameId);
            var filter = Builders<Game>.Filter.Eq(g => g.Id, objectId);
            await _games.DeleteOneAsync(filter);
        }
    }
}
