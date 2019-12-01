using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClassQuizGame
{
    public class Question
    {
        public String question;
        public BitmapImage questionImage;
        public int answerCount;
        public String[] answers;
        public BitmapImage[] answerImages;
        public int correctAnswer;
        public Boolean usingCustomPoints;
        public int customPoints;
        public Boolean usingCustomPenalty;
        public int customPenalty;
        public Boolean timed;
        public int timer;
        public Boolean mustAnswer;

        public Question(String question, BitmapImage questionImage,
            String[] answers, BitmapImage[] answerImages, int correctAnswer)
        {
            this.question = question;
            this.questionImage = questionImage;
            this.answerCount = answers.Length;
            this.answers = answers;
            this.answerImages = answerImages;
            this.correctAnswer = correctAnswer;
            this.usingCustomPoints = false;
            this.usingCustomPenalty = false;
            this.timed = false;
            this.timer = 0;
            this.mustAnswer = false;
        }
    }
}
