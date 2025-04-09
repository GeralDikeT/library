using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Library.DateBase
{
    public class DatabaseHelper
    {
        private const string DbFile = "library.db";

        public DatabaseHelper()
        {
            if (!File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);    
                CreateTable();
            }
        }

        private SQLiteConnection GetConnection() =>
            new SQLiteConnection($"Data Source={DbFile};Version=3;");

        private void CreateTable()
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new SQLiteCommand(@"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Author TEXT,
                    Year INTEGER,
                    Description TEXT
                )", conn);
            cmd.ExecuteNonQuery();
        }

        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            using var conn = GetConnection();
            conn.Open();
            var cmd = new SQLiteCommand("SELECT * FROM Books", conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Title = reader["Title"].ToString(),
                    Author = reader["Author"].ToString(),
                    Year = Convert.ToInt32(reader["Year"]),
                    Description = reader["Description"].ToString()
                });
            }
            return books;
        }

        public int AddBook(Book book)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new SQLiteCommand("INSERT INTO Books (Title, Author, Year, Description) VALUES (@Title, @Author, @Year, @Description); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@Year", book.Year);
            cmd.Parameters.AddWithValue("@Description", book.Description);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void UpdateBook(Book book)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new SQLiteCommand("UPDATE Books SET Title = @Title, Author = @Author, Year = @Year, Description = @Description WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", book.Id);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@Year", book.Year);
            cmd.Parameters.AddWithValue("@Description", book.Description);
            cmd.ExecuteNonQuery();
        }

        public void DeleteBook(int id)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = new SQLiteCommand("DELETE FROM Books WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
