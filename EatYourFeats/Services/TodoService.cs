/*
 * Prologue
Name: Isabel Loney, Jackson Wunderlich
Date Created: 11/10/2024
Date Revised: 11/10/2024
Purpose: Allows the Tasks collection in the database to be accessed, so the task list of a user can be retrieved and tasks can be added

Preconditions: MongoDB setup, Tasks table exists, Todo model defined
Postconditions: Tasks retrieved based on provided username, add a task to database
Error and exceptions: ArgumentNullException can occur if the username is null
Side effects: N/A
Invariants: _tasks is always initialized as the Tasks collection in MongoDB
Other faults: N/A
*/

// Required namespaces for MongoDB functionality and accessing task models
using MongoDB.Driver;       // MongoDB driver for database interactions
using MongoDB.Bson;
using EatYourFeats.Models;  // Todo model

namespace EatYourFeats.Services {
    // Handles database operations for Todo entities, including retrieval and insertion
    public class TodoService {
        // MongoDB collection that stores Todo documents
        private readonly IMongoCollection<Todo> _tasks;

        // Constructor that initializes the tasks collection using a provided MongoDBService instance
        public TodoService(MongoDBService mongoDBService) {
            _tasks = mongoDBService.GetCollection<Todo>("Tasks");
        }

        // gets a list of tasks from the database based on the current user's username
        public async Task<List<Todo>> GetTasksByUsernameAsync(string username) {
            var filter = Builders<Todo>.Filter.Eq(todo => todo.Username, username);
            var help = await _tasks.Find(filter).ToListAsync();
            return help;
        }

        public async Task<List<Todo>> GetTasksByIdsAsync(List<string> taskIds)
        {
            var objectIds = taskIds.Select(id => ObjectId.Parse(id)).ToList();
            return await _tasks.Find(task => objectIds.Contains(task.Id)).ToListAsync();
        }

        public async Task UpdateTaskAsync(Todo task) =>
            await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);

        // Deletes the task from the database
        public async Task DeleteTasksByIdsAsync(List<string> taskIds)
        {
            var objectIds = taskIds.Select(id => ObjectId.Parse(id)).ToList();
            await _tasks.DeleteManyAsync(task => objectIds.Contains(task.Id));
        }

        // inserts a new task into the database
        public async Task CreateTaskAsync(Todo new_task) =>
            await _tasks.InsertOneAsync(new_task);

        public async Task<List<Todo>> GetTasksByGameIdAsync(string gameId)
        {
            var objectId = ObjectId.Parse(gameId);
            var filter = Builders<Todo>.Filter.Eq(t => t.GameId, objectId);
            return await _tasks.Find(filter).ToListAsync();
        }
    }
}
