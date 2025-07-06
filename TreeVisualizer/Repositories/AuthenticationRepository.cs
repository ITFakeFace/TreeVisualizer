using MultipleChoice.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeVisualizer.Models;

namespace TreeVisualizer.Repositories
{
    internal class AuthenticationRepository : BaseRepository
    {
        public AuthenticationRepository()
        {
            // Constructor logic if needed
        }
        public int Login(string email, string password)
        {
            var passUtil = new SecurityUtil();
            using (var conn = GetConnection())
            {
                User user;
                conn.Open();
                string sql = "SELECT * FROM Users WHERE email = @Email";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("id"),
                                Email = reader.GetString("email"),
                                Username = reader.GetString("username"),
                                Password = reader.GetString("password")
                            };
                            if (passUtil.HashPassword(password).Equals(user.Password))
                            {
                                return user.Id;
                            }
                            return -1;
                        }
                    }
                }
                conn.Close();
            }
            return -1;

        }
    }
}
