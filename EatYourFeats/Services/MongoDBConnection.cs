/*
Name: Ginny Ke
Date Created: 10/27/24
Date Revised: 11/6/24
Purpose: Establishes a connection to the MongoDB database and provides methods for accessing collections and verifying the connection.
*/

using MongoDB.Bson;
using MongoDB.Driver; // MongoDB driver for .NET

namespace EatYourFeats.Services
{
    // Service class to manage MongoDB connections and provide access to collections
    public class MongoDBService
    {
        private readonly MongoClient _client;       // MongoClient instance for managing the database connection
        private readonly IMongoDatabase _database;  // Reference to the specified MongoDB database

        // Constructor that initializes MongoDB client and connects to the database
        public MongoDBService(string connectionUri)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionUri); // Configures client settings with URI
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);                // Sets the server API version
            _client = new MongoClient(settings);                                    // Instantiates the MongoClient with settings
            _database = _client.GetDatabase("EatMyFeats");                          // Connects to the specific database by name
        }

        // Generic method to retrieve a collection by name and type, allowing for CRUD operations on that collection
        public IMongoCollection<T> GetCollection<T>(string collectionName) =>
            _database.GetCollection<T>(collectionName);

        // Verifies the database connection by pinging the admin database and prints a success message if connected
        public void PingDatabase()
        {
            try
            {
                var result = _client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1)); // Ping command
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!"); // Confirmation message on success
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // Logs any exception if the connection fails
            }
        }
    }
}
