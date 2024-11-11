/*
Name: Isabel Loney, Jackson Wunderlich
Date Created: 11/10/2024
Date Revised: 11/10/2024
Purpose: 

Preconditions: MongoDB setup, Tasks table exists, Todo model defined
Postconditions: 
Error and exceptions: 
Side effects: 
Invariants: 
Other faults: 
*/

// Required namespaces for MongoDB functionality and accessing user models
using MongoDB.Driver;       // MongoDB driver for database interactions
using EatYourFeats.Models;  // User model

namespace EatYourFeats.Services {
    // Handles database operations for User entities, including retrieval and insertion
    public class TodoService {
        // MongoDB collection that stores User documents
        private readonly IMongoCollection<Todo> _tasks;

        // Constructor that initializes the collection using a provided MongoDBService instance
        public TodoService(MongoDBService mongoDBService) {
            _tasks = mongoDBService.GetCollection<Todo>("Tasks");
        }

        // gets a list of tasks from the database based on the current user's username
        public async Task<List<Todo>> GetTasksByUsernameAsync(string username) {
            var filter = Builders<Todo>.Filter.Eq(todo => todo.Username, username);
            var help = await _tasks.Find(filter).ToListAsync();
            return help;
        }
        

        // inserts a new task into the database
        public async Task CreateTaskAsync(Todo new_task) =>
            await _tasks.InsertOneAsync(new_task);
    }
}
