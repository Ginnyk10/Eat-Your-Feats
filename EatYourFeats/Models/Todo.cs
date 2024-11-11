using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class Todo // User model
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public bool IsCompleted { get; set; }
    }
}
