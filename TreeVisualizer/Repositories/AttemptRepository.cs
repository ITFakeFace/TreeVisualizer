using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeVisualizer.DTOS;
using TreeVisualizer.Models;

namespace TreeVisualizer.Repositories
{
    internal class AttemptRepository : BaseRepository
    {
        public AttemptRepository()
        {
            // Constructor logic if needed
        }

        public List<Attempt> GetByQuizzId(int quizzId)
        {
            List<Attempt> attempts = new List<Attempt>();
            using (var conn = GetConnection())
            {
                var result = new List<Attempt>();
                conn.Open();
                string sql = "SELECT * FROM attemps WHERE quizz_id = @QuizzId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@QuizzId", quizzId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var attempt = new Attempt
                            {
                                Id = reader.GetInt32("id"),
                                AnsweredBy = reader.GetInt32("answered_by"),
                                QuizzId = reader.GetInt32("quizz_id"),
                                CorrectNumber = reader.IsDBNull(reader.GetOrdinal("correct_number"))
                                                ? 0
                                                : reader.GetInt32("correct_number"),
                                Time = reader.GetTimeSpan("time"),
                                Complete = reader.GetBoolean("complete")
                            };

                            result.Add(attempt);
                        }
                    }
                }
            }
            return attempts;
        }
        public List<Attempt> GetAllAttempts() // Đã đổi tên từ GetAllAttemptsFromDb
        {
            var result = new List<Attempt>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, answered_by, quizz_id, correct_number, `time`, complete FROM attemps";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Map data from reader to Attempt object
                        var attempt = new Attempt
                        {
                            Id = reader.GetInt32("id"),
                            AnsweredBy = reader.GetInt32("answered_by"),
                            QuizzId = reader.GetInt32("quizz_id"),
                            CorrectNumber = reader.IsDBNull(reader.GetOrdinal("correct_number")) ? 0 : reader.GetInt32("correct_number"),
                            Time = reader.GetTimeSpan("time"),
                            Complete = reader.GetBoolean("complete")
                        };
                        result.Add(attempt);
                    }
                }
            }
            return result;
        }
        public int Create(Attempt attempt)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"INSERT INTO multiplechoiceapplication.attemps
                       (answered_by, quizz_id, correct_number, `time`, complete, start_at)
                       VALUES
                       (@AnsweredBy, @QuizzId, @CorrectNumber, @Time, @Complete, @StartAt)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AnsweredBy", attempt.AnsweredBy);
                    cmd.Parameters.AddWithValue("@QuizzId", attempt.QuizzId);
                    cmd.Parameters.AddWithValue("@CorrectNumber", attempt.CorrectNumber);
                    cmd.Parameters.AddWithValue("@Time", attempt.Time);
                    cmd.Parameters.AddWithValue("@Complete", attempt.Complete);
                    cmd.Parameters.AddWithValue("@StartAt", attempt.StartAt);

                    cmd.ExecuteNonQuery();

                    string getIdSql = "SELECT LAST_INSERT_ID()";
                    using (var idCmd = new MySqlCommand(getIdSql, conn))
                    {
                        return Convert.ToInt32(idCmd.ExecuteScalar());
                    }
                }
            }
        }
        public AttemptInfo? GetAttemptInfoByID(int attemptId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT u.username, q.title, a.correct_number, a.`time`, a.complete, a.start_at
            FROM attemps a
            JOIN users u ON u.id = a.answered_by
            JOIN quizzes q ON a.quizz_id = q.id
            WHERE a.id = @AttemptId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AttemptId", attemptId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AttemptInfo
                            {
                                Username = reader.GetString("username"),
                                Title = reader.GetString("title"),
                                CorrectNumber = reader.GetInt32("correct_number"),
                                Time = reader.GetTimeSpan("time"),
                                Complete = reader.GetBoolean("complete"),
                                StartAt = reader.GetDateTime("start_at"),
                            };
                        }
                    }
                }
                return null;
            }
        }
        public List<Attempt> GetAttempsByUserId(int userId)
        {
            var result = new List<Attempt>();

            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = "SELECT id, answered_by, quizz_id, correct_number, `time`, complete FROM attemps WHERE answered_by = @UserId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var attempt = new Attempt
                            {
                                Id = reader.GetInt32("id"),
                                AnsweredBy = reader.GetInt32("answered_by"),
                                QuizzId = reader.GetInt32("quizz_id"),
                                CorrectNumber = reader.IsDBNull(reader.GetOrdinal("correct_number"))
                                                    ? 0
                                                    : reader.GetInt32("correct_number"),
                                Time = reader.GetTimeSpan("time"),
                                Complete = reader.GetBoolean("complete")
                            };

                            result.Add(attempt);
                        }
                    }
                }
            }
            return result;
        }
        public (int correctNumber, int totalQuestions)? GetAttemptScoreDetails(int attemptId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT
                a.correct_number,
                (SELECT COUNT(*)
                 FROM quizzDetails qd
                 WHERE qd.quizz_id = a.quizz_id) AS total_questions
            FROM attemps a
            WHERE a.id = @AttemptId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AttemptId", attemptId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int correct = reader.GetInt32("correct_number");
                            int total = reader.GetInt32("total_questions");
                            return (correct, total);
                        }
                    }
                }
            }
            return null;
        }
        public int CountAttemptsByUserAndQuizz(int userId, int quizzId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"SELECT COUNT(*) as ATTEMPS
                       FROM attemps
                       WHERE answered_by = @UserId AND quizz_id = @QuizzId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@QuizzId", quizzId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int CountUserQuizzAttempts(int userId, int quizzId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"SELECT COUNT(*) as ATTEMPS
                       FROM attemps
                       WHERE answered_by = @UserId AND quizz_id = @QuizzId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@QuizzId", quizzId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        // Place this method within your AttempRepository class
        // (e.g., internal class AttempRepository : BaseRepository)

        public List<AttemptDto> GetRawUserAttemptData(int answeredBy)
        {
            List<AttemptDto> results = new();

            using (var conn = GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT
                a.correct_number,
                a.`time`,
                a.start_at,
                (SELECT q.title
                 FROM quizzes q
                 WHERE q.id = a.quizz_id) AS 'quizz',
                qc.total_number,
                a.complete
            FROM attemps a
            JOIN (
                SELECT qd.quizz_id, COUNT(qd.id) AS total_number
                FROM quizzdetails qd
                GROUP BY qd.quizz_id
            ) AS qc ON a.quizz_id = qc.quizz_id
            WHERE a.answered_by = @answeredBy";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@answeredBy", answeredBy);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new AttemptDto
                            {
                                CorrectNumber = reader.GetInt32("correct_number"),
                                Time = reader.GetTimeSpan("time"),
                                StartAt = reader.GetDateTime("start_at"),
                                QuizzTitle = reader.GetString("quizz"),
                                TotalNumber = reader.GetInt32("total_number"),
                                Complete = reader.GetBoolean("complete")
                            });
                        }
                    }
                }
            }
            return results;
        }

        public int CountQuizzDetailsByAttempt(int userId, int quizzId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT COUNT(*)
            FROM attemps A
            JOIN quizzDetails QD ON A.quizz_id = QD.quizz_id
            WHERE A.answered_by = @UserId AND A.quizz_id = @QuizzId";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@QuizzId", quizzId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

    }
}
