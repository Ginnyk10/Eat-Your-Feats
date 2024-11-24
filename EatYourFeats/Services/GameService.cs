/*
 * Prologue
Name: Anakha Krishna
Date Created: 11/16/2024
Date Revised: 11/16/2024
Purpose: Methods for accessing Game records in MongoDB, initialize game db

Preconditions: MongoDB setup, Games table exists, Games model defined
Postconditions: Game retrieval by Username or GameId, game creation, game deletion, update game score
Error and exceptions: MongoDB.Driver.MongoException (thrown if there is an issue with the MongoDB connection or operations), ArgumentNullException (thrown if the method parameters are null)
Side effects: N/A
Invariants: _games collection is always initialized with the "Games" collection from the MongoDB database
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