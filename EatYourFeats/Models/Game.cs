using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class Game
    {
        public ObjectId Id { get; set; } // unique game id
        public string Username { get; set; } // username of person owning game
        public DateTime StartTime { get; set; } // game start time
        public DateTime EndTime { get; set; } // game end time
        public int Score { get; set; } // game score
        public string Character {  get; set; } // chosen character
        public string Food { get; set; } // chosen food
    }
}