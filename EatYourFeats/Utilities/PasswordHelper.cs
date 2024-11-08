/*
Name: Isabel Loney
Date Created: 11/7/2024
Date Revised: 11/8/2024
Purpose: Provides utility methods for hashing and verifying passwords using BCrypt.
*/

namespace EatYourFeats.Utilities
{
    // Static class containing helper methods for password management
    public static class PasswordHelper
    {
        // Hashes a plaintext password using BCrypt for secure storage
        public static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        // Verifies a plaintext password against a hashed password to check for a match
        public static bool VerifyPassword(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
