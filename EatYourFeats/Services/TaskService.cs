/*
Name: Isabel Loney
Date Created: 11/7/2024
Date Revised: 11/8/2024
Purpose: Provides data access methods for user-related operations in the MongoDB database, including creating users and retrieving them by email or username.

Preconditions: MongoDB setup, Users table exists, User model defined
Postconditions: User retrieved by email or username, new users can be inserted
Error and exceptions: MongoDB.Driver.MongoException (thrown if there is an issue with the MongoDB connection or operations), ArgumentNullException (thrown if the email, username, or user parameter is null)
Side effects: N/A
Invariants: _users collection is always initialized with the "Users" collection from the MongoDB database
Other faults: N/A
*/

// Required namespaces for MongoDB functionality and accessing task models
using MongoDB.Driver;       // MongoDB driver for database interactions
using EatYourFeats.Models;  // Task model
using System.Collections.Generic; // Provides List<T> and other collection classes
using System.Threading.Tasks;     // Provides Task-based asynchronous programming

namespace EatYourFeats.Services
{
    // Handles database operations for Task entities, including retrieval and updates
    public class TaskService
    {
        // MongoDB collection that stores Task documents
        private readonly IMongoCollection<TaskItem> _tasks;

        // Constructor that initializes the collection using a provided MongoDBService instance
        public TaskService(MongoDBService mongoDBService)
        {
            _tasks = mongoDBService.GetCollection<TaskItem>("Tasks"); // Access "Tasks" collection from MongoDB
        }

        // Retrieves all Task documents for a specific user based on the provided username
        public async Task<List<TaskItem>> GetTasksByUsernameAsync(string username) =>
            await _tasks.Find(task => task.Username == username).ToListAsync();

        // Retrieves Task documents based on the provided list of task IDs
        public async Task<List<TaskItem>> GetTasksByIdsAsync(List<string> taskIds) =>
            await _tasks.Find(task => taskIds.Contains(task.Id)).ToListAsync();

        // Updates a Task document in the collection asynchronously
        public async Task UpdateTaskAsync(TaskItem task) =>
            await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);

        // Deletes the task from the database
        public async Task DeleteTasksByIdsAsync(List<string> taskIds) =>
            await _tasks.DeleteManyAsync(task => taskIds.Contains(task.Id));

    }
}
