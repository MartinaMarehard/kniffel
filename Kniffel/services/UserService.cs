using System;
using System.IO;
using System.Data.SQLite;
using Kniffel.models;

namespace Kniffel.services
{
    public class UserService
    {
        private static readonly string DbFilePath = Path.Combine(
            Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
            "data", "kniffel.db");

        private static readonly string ConnectionString = $"Data Source={DbFilePath};Version=3;";

        
        static UserService()
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Users (" +
                                        "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                        "username TEXT NOT NULL UNIQUE, " +
                                        "passwordHash TEXT NOT NULL, " +
                                        "registeredAt TEXT)", conn);
            cmd.ExecuteNonQuery();
        }



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
        
        public bool VerifyPassword(string username, string password)
        {
            using var conn = new SQLiteConnection(ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand("SELECT passwordHash FROM Users WHERE username = @name", conn);
            cmd.Parameters.AddWithValue("@name", username);
            var dbHash = cmd.ExecuteScalar()?.ToString();

            if (dbHash == null) return false;

            var inputHash = HashPassword(password);
            return dbHash == inputHash;
        }

    }
}
