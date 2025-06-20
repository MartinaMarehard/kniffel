using System;
using System.Data.SQLite;
using Kniffel.models;

namespace Kniffel.Services
{
    public class UserService
    {
        private const string ConnectionString = "Data Source=./data/kniffel.db";

        public bool UserExists(string username)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE username = @name", conn);
            cmd.Parameters.AddWithValue("@name", username);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public void AddUser(User user)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand("INSERT INTO Users (username, passwordHash, registeredAt) VALUES (@name, @hash, @zeit)", conn);
            cmd.Parameters.AddWithValue("@name", user.Username);
            cmd.Parameters.AddWithValue("@hash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@zeit", user.RegisteredAt);
            cmd.ExecuteNonQuery();
        }

        public string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
