using MongoDB.Bson;

namespace EatYourFeats.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // Store hashed passwords for security
    }

}
