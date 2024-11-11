using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class User // User model
    {
        public ObjectId Id { get; set; } // Unique User object Id
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // Store hashed passwords for security
        public int Points { get; set; } // Add this property
    }
}
