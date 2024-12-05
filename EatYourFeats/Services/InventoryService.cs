/*
 * Prologue
Name: Jackson Wunderlich, Dylan Sailors
Date Created: 11/24/2024
Date Revised: 11/24/2024
Purpose: Methods for accessing InventoryItems in MongoDB database

Preconditions: MongoDB setup, Inventory table exists, Inventory model defined
Postconditions: Item retrieval by GameId, adding/removing items to/from the database
Error and exceptions: MongoDB.Driver.MongoException (thrown if there is an issue with the MongoDB connection or operations), ArgumentNullException (thrown if the method parameters are null)
Side effects: N/A
Invariants: _inventory collection is always initialized with the "Inventory" collection from the MongoDB database
Other faults:
*/
using EatYourFeats.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EatYourFeats.Services
{
    public class InventoryService
    {
        private readonly IMongoCollection<InventoryItem> _inventory;

        public InventoryService(MongoDBService mongoDBService)
        {
            _inventory = mongoDBService.GetCollection<InventoryItem>("Inventory");
        }

        public async Task AddInventoryItemAsync(InventoryItem item) =>
            await _inventory.InsertOneAsync(item); // adds a new InventoryItem to the database

        public async Task<List<InventoryItem>> GetItemsByIdAsync(List<string> itemIds) // gets the item by id
        {
            var objectIds = itemIds.Select(id => ObjectId.Parse(id)).ToList();
            return await _inventory.Find(item => objectIds.Contains(item.GameId)).ToListAsync();
        }

        public async Task<InventoryItem> GetItemByItemNameAsync(string itemId) // gets the item by name
        {
            var objectId = ObjectId.Parse(itemId);
            return await _inventory.Find(item => item.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task<InventoryItem> GetEquippedItemAsync(ObjectId gameId)
        {
            return await _inventory.Find(item => item.IsEquipped && item.GameId == gameId).FirstOrDefaultAsync();
        }

        public async Task SetItemEquippedStatus(string itemId, bool equipped)
        {
            var objectId = ObjectId.Parse(itemId);
            var update = Builders<InventoryItem>.Update.Set(i => i.IsEquipped, equipped);
            await _inventory.UpdateOneAsync(i => i.Id == objectId, update);
        }

        public async Task SetItemEquippedTime(string itemId, DateTime time)
        {
            var objectId = ObjectId.Parse(itemId);
            var update = Builders<InventoryItem>.Update.Set(i => i.TimeEquipped, time);
            await _inventory.UpdateOneAsync(i => i.Id == objectId, update);
        }

        public async Task DeleteItemByIdAsync(string itemId)
        {
            var objectId = ObjectId.Parse(itemId);
            var filter = Builders<InventoryItem>.Filter.Eq(i => i.Id, objectId);
            await _inventory.DeleteOneAsync(filter);
        }
    }
}