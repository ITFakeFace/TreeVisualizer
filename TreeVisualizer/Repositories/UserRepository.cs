using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeVisualizer.Models;

namespace TreeVisualizer.Repositories
{
    internal class UserRepository : BaseRepository
    {
        public UserRepository()
        {

        }
        public bool CreateUser(User user)
        {
            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    // Câu lệnh SQL chèn dữ liệu vào bảng users
                    string sql = @"INSERT INTO users (username, password,email) 
                                   VALUES (@Username, @Password,@Email)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public User GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM Users WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("id"),
                                Email = reader.GetString("email"),
                                Username = reader.GetString("username"),
                                Password = reader.GetString("password")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User GetByUsername(string username)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM Users WHERE username = @Username";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("id"),
                                Email = reader.GetString("email"),
                                Username = reader.GetString("username"),
                                Password = reader.GetString("password")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User GetByEmail(string email)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM Users WHERE email = @Email";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("id"),
                                Email = reader.GetString("email"),
                                Username = reader.GetString("username"),
                                Password = reader.GetString("password")
                            };
                        }
                    }
                }
            }
            return null;
        }
        public List<User> GetAll()
        {
            var list = new List<User>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM Users";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            Id = reader.GetInt32("id"),
                            Email = reader.GetString("email"),
                            Username = reader.GetString("username"),
                            Password = reader.GetString("password")
                        });
                    }
                }
            }
            return list;
        }

        public void Update(User user)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Users SET email = @Email, username = @Username, password = @Password WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM Users WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
