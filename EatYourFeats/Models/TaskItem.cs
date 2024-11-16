using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EatYourFeats.Models
{
    public class TaskItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }        // Task ID
        public string Username { get; set; }  // Username of the task owner
        public string Name { get; set; }      // Task name
        public int Points { get; set; }       // Point value assigned to the task
        public bool IsCompleted { get; set; } // Indicates if the task is completed
        public ObjectId GameId { get; set; }
    }
}
