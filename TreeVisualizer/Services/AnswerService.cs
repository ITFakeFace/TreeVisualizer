using System.Collections.Generic;
using TreeVisualizer.Models;
using TreeVisualizer.Repositories;

namespace TreeVisualizer.Services
{
    internal class AnswerService
    {
        private readonly AnswerRepository _answerRepo;

        public AnswerService()
        {
            _answerRepo = new AnswerRepository();
        }

        public bool CreateAnswers(List<Answer> answers)
        {
            // Có thể xử lý thêm validate ở đây nếu cần
            return _answerRepo.CreateAnswers(answers);
        }

        public List<AnswerResult> GetAnswers(int attemptId, int quizId)
        {
            return _answerRepo.GetAnswersByAttemptAndQuiz(attemptId, quizId);
        }
    }
}
