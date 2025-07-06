using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TreeVisualizer.Models; // Ensure QuizzDetails model is accessible
using System;

namespace TreeVisualizer.Repositories
{
    public class QuizzDetailRepository : BaseRepository
    {
        public bool Create(QuizzDetails detail)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO QuizzDetails
                               (quizz_id, question, answer1, answer2, answer3, answer4, correct_answer)
                               VALUES
                               (@QuizzId, @Question, @Answer1, @Answer2, @Answer3, @Answer4, @CorrectAnswer)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizzId", detail.QuizzId);
                    cmd.Parameters.AddWithValue("@Question", detail.Question);
                    cmd.Parameters.AddWithValue("@Answer1", detail.Answer1);
                    cmd.Parameters.AddWithValue("@Answer2", detail.Answer2);
                    cmd.Parameters.AddWithValue("@Answer3", detail.Answer3);
                    cmd.Parameters.AddWithValue("@Answer4", detail.Answer4);
                    cmd.Parameters.AddWithValue("@CorrectAnswer", detail.CorrectAnswer);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }

        public List<QuizzDetails> GetByQuizzId(int quizzId)
        {
            var list = new List<QuizzDetails>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, quizz_id, question, answer1, answer2, answer3, answer4, correct_answer FROM QuizzDetails WHERE quizz_id = @QuizzId";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizzId", quizzId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new QuizzDetails
                            {
                                Id = reader.GetInt32("id"),
                                QuizzId = reader.GetInt32("quizz_id"),
                                Question = reader.GetString("question"),
                                Answer1 = reader.GetString("answer1"),
                                Answer2 = reader.GetString("answer2"),
                                Answer3 = reader.GetString("answer3"),
                                Answer4 = reader.GetString("answer4"),
                                CorrectAnswer = reader.GetInt16("correct_answer")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Update(QuizzDetails detail)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE QuizzDetails SET
                                question = @Question,
                                answer1 = @Answer1,
                                answer2 = @Answer2,
                                answer3 = @Answer3,
                                answer4 = @Answer4,
                                correct_answer = @CorrectAnswer
                                WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Question", detail.Question);
                    cmd.Parameters.AddWithValue("@Answer1", detail.Answer1);
                    cmd.Parameters.AddWithValue("@Answer2", detail.Answer2);
                    cmd.Parameters.AddWithValue("@Answer3", detail.Answer3);
                    cmd.Parameters.AddWithValue("@Answer4", detail.Answer4);
                    cmd.Parameters.AddWithValue("@CorrectAnswer", detail.CorrectAnswer);
                    cmd.Parameters.AddWithValue("@Id", detail.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM QuizzDetails WHERE id = @Id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetTotalQuizzDetailsCount(int quizzId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"SELECT COUNT(*)
                               FROM quizzDetails
                               WHERE quizz_id = @QuizzId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizzId", quizzId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}