/*
Name: Isabel Loney
Date Created: 11/7/2024
Date Revised: 11/8/2024
Purpose: Provides data access methods for user-related operations in the MongoDB database, including creating users and retrieving them by email or username.
*/

// Required namespaces for MongoDB functionality and accessing user models
using MongoDB.Driver;       // MongoDB driver for database interactions
using EatYourFeats.Models;  // User model

namespace EatYourFeats.Services
{
    // Handles database operations for User entities, including retrieval and insertion
    public class UserService
    {
        // MongoDB collection that stores User documents
        private readonly IMongoCollection<User> _users;

        // Constructor that initializes the collection using a provided MongoDBService instance
        public UserService(MongoDBService mongoDBService)
        {
            _users = mongoDBService.GetCollection<User>("Users"); // Access "Users" collection from MongoDB
        }

        // Retrieves a User document based on the provided email; returns null if not found
        public async Task<User> GetUserByEmailAsync(string email) =>
            await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

        // Retrieves a User document based on the provided username; returns null if not found
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _users.Find(user => user.Username == username).FirstOrDefaultAsync();
        }

        // Inserts a new User document into the collection asynchronously
        public async Task CreateUserAsync(User user) =>
            await _users.InsertOneAsync(user);
    }
}
