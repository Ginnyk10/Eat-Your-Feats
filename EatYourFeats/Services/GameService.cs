/*
 * Prologue
Name: Anakha Krishna, Jackson Wunderlich
Date Created: 11/16/2024
Date Revised: 11/24/2024
Purpose: Methods for accessing Game records in MongoDB, initialize game db

Preconditions: MongoDB setup, Games table exists, Game model defined
Postconditions: Game retrieval by Username or GameId, game creation, game deletion, update game score, shop list creation
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

        // creates a list of ShopItem objects
        public async Task<List<ShopItem>> CreateShop()
        {
            return await Task.Run(() =>
            {
                List<ShopItem> Items = new List<ShopItem>();

                // creates a new ShopItem
                var water = new ShopItem
                {
                    ItemName = "Water",
                    ItemPrice = 3,
                    ItemEffect = "DoubleNext"
                };
                Items.Add(water); // adds the new item to the list

                var coupon = new ShopItem
                {
                    ItemName = "Coupon",
                    ItemPrice = 2,
                    ItemEffect = "AddTask"
                };
                Items.Add(coupon);

                var supplement = new ShopItem
                {
                    ItemName = "Sketchy Catabolic Supplement",
                    ItemPrice = 5,
                    ItemEffect = "TripleHighest"
                };
                Items.Add(supplement);

                var fortune_cookie = new ShopItem
                {
                    ItemName = "Fortune Cookie",
                    ItemPrice = 1,
                    ItemEffect = "Random"
                };
                Items.Add(fortune_cookie);

                return Items;
            });
        }
    }
}