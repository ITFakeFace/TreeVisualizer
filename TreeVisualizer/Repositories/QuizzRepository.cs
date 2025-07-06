using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using TreeVisualizer.Models; // Ensure Quizz model is accessible

namespace TreeVisualizer.Repositories
{
    public class QuizzRepository : BaseRepository
    {
        public bool Create(Quizz quizz)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO Quizzes
                               (title, type, is_random, attemp_number, created_by, is_result_showable, start_at, end_at, time_limit)
                               VALUES
                               (@Title, @Type, @IsRandom, @AttempNumber, @CreatedBy, @IsResultShowable, @StartAt, @EndAt, @TimeLimit)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", quizz.Title);
                    cmd.Parameters.AddWithValue("@Type", quizz.Type);
                    cmd.Parameters.AddWithValue("@IsRandom", quizz.IsRandom);
                    cmd.Parameters.AddWithValue("@AttempNumber", quizz.AttempNumber);
                    cmd.Parameters.AddWithValue("@CreatedBy", quizz.CreatedBy);
                    cmd.Parameters.AddWithValue("@IsResultShowable", quizz.IsResultShowable);
                    cmd.Parameters.AddWithValue("@StartAt", quizz.StartAt);
                    cmd.Parameters.AddWithValue("@EndAt", quizz.EndAt);
                    cmd.Parameters.AddWithValue("@TimeLimit", quizz.TimeLimit);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }

        public Quizz? GetById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, title, type, is_random, attemp_number, created_by, is_result_showable, start_at, end_at, time_limit FROM Quizzes WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Quizz
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Type = reader.GetString("type"),
                                IsRandom = reader.GetBoolean("is_random"),
                                AttempNumber = reader.IsDBNull("attemp_number") ? null : reader.GetInt32("attemp_number"),
                                CreatedBy = reader.GetInt32("created_by"),
                                IsResultShowable = reader.GetBoolean("is_result_showable"),
                                StartAt = reader.IsDBNull("start_at") ? null : reader.GetDateTime("start_at"),
                                EndAt = reader.IsDBNull("end_at") ? null : reader.GetDateTime("end_at"),
                                TimeLimit = reader.IsDBNull("time_limit") ? null : reader.GetTimeSpan("time_limit")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Quizz> GetAll()
        {
            var list = new List<Quizz>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, title, type, is_random, attemp_number, created_by, is_result_showable, start_at, end_at, time_limit FROM Quizzes";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Quizz
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Type = reader.GetString("type"),
                            IsRandom = reader.GetBoolean("is_random"),
                            AttempNumber = reader.IsDBNull("attemp_number") ? null : reader.GetInt32("attemp_number"),
                            CreatedBy = reader.GetInt32("created_by"),
                            IsResultShowable = reader.GetBoolean("is_result_showable"),
                            StartAt = reader.IsDBNull("start_at") ? null : reader.GetDateTime("start_at"),
                            EndAt = reader.IsDBNull("end_at") ? null : reader.GetDateTime("end_at"),
                            TimeLimit = reader.IsDBNull("time_limit") ? null : reader.GetTimeSpan("time_limit")
                        });
                    }
                }
            }
            return list;
        }

        public List<Quizz> GetByTopic(string topic)
        {
            var list = new List<Quizz>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, title, type, is_random, attemp_number, created_by, is_result_showable, start_at, end_at, time_limit FROM Quizzes WHERE type = @Topic";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Topic", topic);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Quizz
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Type = reader.GetString("type"),
                                IsRandom = reader.GetBoolean("is_random"),
                                AttempNumber = reader.IsDBNull("attemp_number") ? null : reader.GetInt32("attemp_number"),
                                CreatedBy = reader.GetInt32("created_by"),
                                IsResultShowable = reader.GetBoolean("is_result_showable"),
                                StartAt = reader.IsDBNull("start_at") ? null : reader.GetDateTime("start_at"),
                                EndAt = reader.IsDBNull("end_at") ? null : reader.GetDateTime("end_at"),
                                TimeLimit = reader.IsDBNull("time_limit") ? null : reader.GetTimeSpan("time_limit")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Quizz> GetByUserId(int userId)
        {
            var list = new List<Quizz>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, title, type, is_random, attemp_number, created_by, is_result_showable, start_at, end_at, time_limit FROM Quizzes WHERE created_by = @UserId";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Quizz
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Type = reader.GetString("type"),
                                IsRandom = reader.GetBoolean("is_random"),
                                AttempNumber = reader.IsDBNull("attemp_number") ? null : reader.GetInt32("attemp_number"),
                                CreatedBy = reader.GetInt32("created_by"),
                                IsResultShowable = reader.GetBoolean("is_result_showable"),
                                StartAt = reader.IsDBNull("start_at") ? null : reader.GetDateTime("start_at"),
                                EndAt = reader.IsDBNull("end_at") ? null : reader.GetDateTime("end_at"),
                                TimeLimit = reader.IsDBNull("time_limit") ? null : reader.GetTimeSpan("time_limit")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Update(Quizz quizz)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE Quizzes SET
                                title = @Title,
                                type = @Type,
                                is_random = @IsRandom,
                                attemp_number = @AttempNumber,
                                created_by = @CreatedBy,
                                is_result_showable = @IsResultShowable,
                                start_at = @StartAt,
                                end_at = @EndAt,
                                time_limit = @TimeLimit
                                WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", quizz.Title);
                    cmd.Parameters.AddWithValue("@Type", quizz.Type);
                    cmd.Parameters.AddWithValue("@IsRandom", quizz.IsRandom);
                    cmd.Parameters.AddWithValue("@AttempNumber", quizz.AttempNumber);
                    cmd.Parameters.AddWithValue("@CreatedBy", quizz.CreatedBy);
                    cmd.Parameters.AddWithValue("@IsResultShowable", quizz.IsResultShowable);
                    cmd.Parameters.AddWithValue("@StartAt", quizz.StartAt);
                    cmd.Parameters.AddWithValue("@EndAt", quizz.EndAt);
                    cmd.Parameters.AddWithValue("@TimeLimit", quizz.TimeLimit);
                    cmd.Parameters.AddWithValue("@Id", quizz.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM Quizzes WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}