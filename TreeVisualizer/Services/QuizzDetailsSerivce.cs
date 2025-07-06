using TreeVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Windows; // Required for MessageBox.Show
using TreeVisualizer.Repositories; // Ensure QuizzDetailRepository is accessible

namespace TreeVisualizer.Services
{
    public class QuizzDetailsService
    {
        private readonly QuizzDetailRepository _quizzDetailRepository;

        public QuizzDetailsService()
        {
            _quizzDetailRepository = new QuizzDetailRepository();
        }

        public bool Create(QuizzDetails detail)
        {
            try
            {
                // You can add business validation here before calling the repository
                if (detail.QuizzId <= 0 || string.IsNullOrWhiteSpace(detail.Question) || detail.CorrectAnswer < 1 || detail.CorrectAnswer > 4)
                {
                    MessageBox.Show("Invalid QuizzDetails data provided.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                return _quizzDetailRepository.Create(detail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create quiz detail: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public List<QuizzDetails> GetByQuizzId(int quizzId)
        {
            try
            {
                // Add any business logic/filtering/sorting here if needed
                return _quizzDetailRepository.GetByQuizzId(quizzId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve quiz details: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<QuizzDetails>(); // Return empty list on error
            }
        }

        public void Update(QuizzDetails detail)
        {
            try
            {
                // Add business validation before updating
                if (detail.Id <= 0 || string.IsNullOrWhiteSpace(detail.Question) || detail.CorrectAnswer < 1 || detail.CorrectAnswer > 4)
                {
                    MessageBox.Show("Invalid QuizzDetails data provided for update.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _quizzDetailRepository.Update(detail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update quiz detail: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                _quizzDetailRepository.Delete(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete quiz detail: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public int GetTotalQuizzDetails(int quizzId)
        {
            try
            {
                // No specific business logic needed here, just pass through
                return _quizzDetailRepository.GetTotalQuizzDetailsCount(quizzId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to get total quiz details count: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }
    }
}