using TreeVisualizer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeVisualizer.Repositories;

namespace TreeVisualizer.Services
{
    internal class AttempServices : BaseRepository
    {
        AttemptRepository _attemptRepository;
        public AttempServices()
        {
            _attemptRepository = new AttemptRepository();
        }
        public List<Attempt> GetByQuizzId(int quizzId)
        {
            return _attemptRepository.GetByQuizzId(quizzId);
        }
        public List<Attempt> GetAll()
        {
            try
            {
                List<Attempt> attempts = _attemptRepository.GetAllAttempts(); // Gọi _attempRepo.GetAllAttempts()
                return attempts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting all attempts: {ex.Message}");
                return new List<Attempt>();
            }
        }
       
        public int Create(Attempt attempt)
        {
            if (attempt.AnsweredBy <= 0 || attempt.QuizzId <= 0)
            {
                Console.WriteLine("Validation Error: AnsweredBy and QuizzId must be positive integers.");
                return -3; 
            }

            try
            {
                int newAttemptId = _attemptRepository.Create(attempt);

                if (newAttemptId > 0)
                {
                    Console.WriteLine($"Successfully created attempt with ID: {newAttemptId}");
                }
                else
                {
                    Console.WriteLine("Attempt creation failed (Repository returned 0 or a non-positive ID).");
                }

                return newAttemptId;
            }
            catch (MySql.Data.MySqlClient.MySqlException dbEx)
            {
                Console.WriteLine($"Database Error creating attempt: {dbEx.Message}. SQL State: {dbEx.SqlState}");
                return -2;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while creating attempt: {ex.Message}");
                return -1;
            }
        }
        public AttemptInfo? GetAttemptDetailInfo(int attemptId)
        {
            try
            {
                AttemptInfo? attemptInfo = _attemptRepository.GetAttemptInfoByID(attemptId);


                return attemptInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving attempt info: {ex.Message}");
                return null;
            }
        }
        public List<Attempt> GetAttemptsForUser(int userId)
        {
            try
            {
                List<Attempt> attempts = _attemptRepository.GetAttempsByUserId(userId);
                return attempts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving attempts for user {userId}: {ex.Message}");
                return new List<Attempt>();
            }
        }
        public DetailsScore? CalculateAttemptScore(int attemptId)
        {
            try
            {
                (int correctNumber, int totalQuestions)? details = _attemptRepository.GetAttemptScoreDetails(attemptId);

                if (details.HasValue)
                {
                    int correct = details.Value.correctNumber;
                    int total = details.Value.totalQuestions;
                    int score = total > 0 ? correct * 10 / total : 0;

                    return new DetailsScore
                    {
                        CorrectNumber = correct,
                        TotalQuestion = total,
                        Score = score
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating score for attempt {attemptId}: {ex.Message}");
            }
            return null;
        }

        public int GetAttempsOfUserByQuizzId(int userId, int quizzId)
        {
            try
            {
                return _attemptRepository.CountAttemptsByUserAndQuizz(userId, quizzId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting attempt count for user {userId} on quizz {quizzId}: {ex.Message}");
                return -1;
            }
        }
        public int GetHistoryAttempts(int userId, int quizzId)
        {
            try
            {
                return _attemptRepository.CountUserQuizzAttempts(userId, quizzId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving attempt history for user {userId} on quizz {quizzId}: {ex.Message}");
                return -1;
            }
        }
        // Place this method within your AttempService class
        // (e.g., internal class AttempService)
        // Ensure _attempRepo is initialized, e.g., in the constructor: _attempRepo = new AttempRepository();

        public List<UserAttempt> GetUserAttempts(int userId)
        {
            List<UserAttempt> userAttempts = new();

            try
            {
                var attemptDtos = _attemptRepository.GetRawUserAttemptData(userId);

                foreach (var data in attemptDtos)
                {
                    float score = data.TotalNumber > 0
                                  ? (float)Math.Round(((float)data.CorrectNumber * 10 / data.TotalNumber), 2)
                                  : 0;

                    string isCompletedStatus = data.Complete ? "Completed" : "Not Completed";

                    userAttempts.Add(new UserAttempt()
                    {
                        Quizz = data.QuizzTitle,
                        Score = score,
                        Time = data.Time,
                        StartAt = data.StartAt,
                        IsCompleted = isCompletedStatus
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user attempts for user ID {userId}: {ex.Message}");
                return new List<UserAttempt>();
            }

            return userAttempts;
        }
        public int GetTotalQuizzDetails(int userId, int quizzId)
        {
            try
            {
                return _attemptRepository.CountQuizzDetailsByAttempt(userId, quizzId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting total quiz details for user {userId} and quizz {quizzId}: {ex.Message}");
                return -1;
            }
        }
    }
}
