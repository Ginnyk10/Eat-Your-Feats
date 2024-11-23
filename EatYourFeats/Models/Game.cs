using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class Game
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Score { get; set; }
    }
}