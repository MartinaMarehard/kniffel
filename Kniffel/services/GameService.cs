using System;
using System.IO;
using System.Data.SQLite;
using Kniffel.models;
namespace Kniffel.services;

public static class GameService
{
    private static readonly string DbFilePath = Path.Combine(
        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
        "data", "kniffel.db");
    public static void SaveGameResult(int userId, int score)
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "kniffel.db");
        using var connection = new SQLiteConnection($"Data Source={DbFilePath};Version=3;");
        connection.Open();
        var cmd = new SQLiteCommand("INSERT INTO game_results (user_id, score, date_played) VALUES (@userId, @score, @date)", connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@score", score);
        cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("s")); // ISO-Format
        cmd.ExecuteNonQuery();
    }
}