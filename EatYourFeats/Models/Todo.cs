using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class Todo // Todo model
    {
        public ObjectId Id { get; set; } // unique object id
        public string Username { get; set; } // username depends on which user added the task
        public string Name { get; set; }
        public int Points { get; set; }
        public bool IsCompleted { get; set; }
        public ObjectId GameId { get; set; }
    }
}
