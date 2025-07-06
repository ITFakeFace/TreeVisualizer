using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Models
{
    public class AnswerDTO
    {
        public int QuestionID { get; set; }
        public int Answer {  get; set; }
        public int CorrectAnswer {  get; set; }
        public AnswerDTO(int questionID,int answer, int correctAnswer) 
        {
            this.QuestionID = questionID;
            this.Answer = answer;
            this.CorrectAnswer = correctAnswer;
        }
    }
}
