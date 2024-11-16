/*
Name: Dylan Sailors
Date Created: 11/10/2024
Date Revised: 11/10/2024
Purpose: Provides data access methods for task-related operations in the MongoDB database, including retrieving tasks by username, retrieving tasks by ID list, updating tasks, and deleting tasks by ID list.

Preconditions: MongoDB setup, Tasks collection exists, TaskItem model defined.
Postconditions: Tasks retrieved, updated, or deleted as requested based on username or task ID list.
Error and exceptions: MongoDB.Driver.MongoException (thrown if there is an issue with MongoDB connection or operations), ArgumentNullException (thrown if task, username, or taskIds parameters are null).
Side effects: N/A
Invariants: _tasks collection is always initialized with the "Tasks" collection from the MongoDB database.
Other faults: N/A
*/

// Required namespaces for MongoDB functionality and accessing task models
using EatYourFeats.Models;  // Task model
using MongoDB.Driver;       // MongoDB driver for database interactions

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
