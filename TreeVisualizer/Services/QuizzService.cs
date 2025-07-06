using TreeVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Windows; // Required for MessageBox.Show
using TreeVisualizer.Repositories; // Ensure QuizzRepository is accessible

namespace TreeVisualizer.Services
{
    public class QuizzService
    {
        private readonly QuizzRepository _quizzRepository;

        public QuizzService()
        {
            _quizzRepository = new QuizzRepository();
        }

        public bool Create(Quizz quizz)
        {
            try
            {
                // Add business validation here (e.g., check if title is not empty)
                if (string.IsNullOrWhiteSpace(quizz.Title) || quizz.CreatedBy <= 0)
                {
                    MessageBox.Show("Invalid quiz data provided. Title and Creator are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                return _quizzRepository.Create(quizz);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create quiz: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public Quizz? GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    MessageBox.Show("Invalid ID provided.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                return _quizzRepository.GetById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve quiz: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<Quizz> GetAll()
        {
            try
            {
                // No specific business logic for GetAll, just pass through
                return _quizzRepository.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve all quizzes: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Quizz>(); // Return empty list on error
            }
        }

        public List<Quizz> GetByTopic(string topic)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(topic))
                {
                    MessageBox.Show("Topic cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return new List<Quizz>();
                }
                return _quizzRepository.GetByTopic(topic);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve quizzes by topic: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Quizz>();
            }
        }

        public List<Quizz> GetByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    MessageBox.Show("Invalid User ID provided.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return new List<Quizz>();
                }
                return _quizzRepository.GetByUserId(userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve quizzes by user ID: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Quizz>();
            }
        }

        public void Update(Quizz quizz)
        {
            try
            {
                // Add business validation before updating
                if (quizz.Id <= 0 || string.IsNullOrWhiteSpace(quizz.Title) || quizz.CreatedBy <= 0)
                {
                    MessageBox.Show("Invalid quiz data provided for update. ID, Title, and Creator are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _quizzRepository.Update(quizz);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update quiz: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    MessageBox.Show("Invalid ID provided for deletion.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _quizzRepository.Delete(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete quiz: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}