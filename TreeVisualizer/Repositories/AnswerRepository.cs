using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TreeVisualizer.Models;

namespace TreeVisualizer.Repositories
{
    internal class AnswerRepository : BaseRepository
    {
        public bool CreateAnswers(List<Answer> answers)
        {
            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO multiplechoiceapplication.answers
                                   (question_id, attemp_id, answer)
                                   VALUES 
                                   (@QuestionId, @AttempID, @Answer)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        foreach (var answer in answers)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
                            cmd.Parameters.AddWithValue("@AttempID", answer.AttempId);
                            cmd.Parameters.AddWithValue("@Answer", answer.SelectedAnswer);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public List<AnswerResult> GetAnswersByAttemptAndQuiz(int attemptId, int quizId)
        {
            List<AnswerResult> result = new List<AnswerResult>();

            using (var conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = @"
                        SELECT qd.id, qd.question, qd.answer1, qd.answer2, qd.answer3, qd.answer4, qd.correct_answer, a.answer
                        FROM quizzdetails qd
                        LEFT JOIN answers a ON qd.id = a.question_id AND a.attemp_id = @AttemptId
                        WHERE qd.quizz_id = @QuizId";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                        cmd.Parameters.AddWithValue("@QuizId", quizId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var answerResult = new AnswerResult
                                {
                                    Question = reader.GetString("question"),
                                    Answer1 = reader.GetString("answer1"),
                                    Answer2 = reader.GetString("answer2"),
                                    Answer3 = reader.GetString("answer3"),
                                    Answer4 = reader.GetString("answer4"),
                                    CorrectAnswer = reader.GetInt32("correct_answer"),
                                    Answer = reader.IsDBNull(reader.GetOrdinal("answer")) ? null : reader.GetInt32("answer").ToString(),
                                };

                                result.Add(answerResult);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DB Error: {ex.Message}");
                }
            }

            return result;
        }
    }
}
