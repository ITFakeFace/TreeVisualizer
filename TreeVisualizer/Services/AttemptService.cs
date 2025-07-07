using MySql.Data.MySqlClient;
using System.Windows;
using TreeVisualizer.Models;
using TreeVisualizer.Repositories;

namespace TreeVisualizer.Services
{
    internal class AttemptServices : BaseRepository
    {
        private readonly AttemptRepository _attemptRepository;
        public AttemptServices()
        {
            _attemptRepository = new AttemptRepository();
        }
        public List<Attempt> GetByQuizzId(int quizzId)
        {
            var result = new List<Attempt>(); // Initialize result outside try-catch

            try
            {
                result = _attemptRepository.GetByQuizzId(quizzId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving attempts by QuizzId {quizzId}: {ex.Message}");
                MessageBox.Show($"There was an error retrieving quiz attempts. Please try again later. Details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Attempt>(); 
            }
            return result;
        }
        public List<Attempt> GetAll()
        {
            var result = new List<Attempt>();

            using (var conn = GetConnection())
            {
                try
                {
                    result = _attemptRepository.GetAllAttempts();
                }
                catch (Exception ex)
                {
                    return null!;
                }
            }

            return result;
        }
        public int Create(Attempt attempt)
        {
            try
            {
                return _attemptRepository.Create(attempt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        public AttemptInfo? GetAttemptInfoByID(int attemptId)
        {
            try
            {
                return _attemptRepository.GetAttemptInfoByID(attemptId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving attempt info for ID {attemptId}: {ex.Message}");
                return null;
            }
        }
        public List<Attempt> GetByUserId(int userId)
        {
            var result = new List<Attempt>();

            try
            {
                result = _attemptRepository.GetAttempsByUserId(userId);
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
            }

            return result;
        }
        public DetailsScore? GetDetailsScore(int attemptId)
        {
            try
            {
                return _attemptRepository.GetDetailsScore(attemptId);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public int GetAttempsOfUserByQuizzId(int userId, int quizzId)
        {
            try
            {
                return _attemptRepository.CountAttemptsByUserAndQuizz(userId, quizzId);
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                return -1; // Trả về -1 nếu lỗi
            }
        }
        public int GetHistoryAttempts(int userId, int quizzId)
        {
            try
            {
                return _attemptRepository.GetHistoryAttempts(userId, quizzId);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public List<UserAttempt> GetAttemptsByUserID(int answeredBy)
        {
            try
            {
                return _attemptRepository.GetAttemptsByUserID(answeredBy);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }
        public int GetTotalQuizzDetails(int userId, int quizzId)
        {
            try
            {
                return _attemptRepository.CountQuizzDetailsByAttempt(userId, quizzId);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
