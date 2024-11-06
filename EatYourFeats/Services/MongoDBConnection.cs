/*
Name: Ginny Ke
Date Created: 10/27/24
Date Revised: 11/6/24
Purpose: Establish connection between MongoDB database and project
*/

//All of this code was given by MongoDB when I created the database.
using MongoDB.Bson;
using MongoDB.Driver; // uses mongoDB driver

namespace EatYourFeats.Services
{
    public class MongoDBService
    {
        private readonly MongoClient _client;

        public MongoDBService(string connectionUri)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            _client = new MongoClient(settings);
        }

        public void PingDatabase()
        {
            try
            {
                var result = _client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!"); // prints if successful connection
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
