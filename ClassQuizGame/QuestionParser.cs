using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace ClassQuizGame
{
    public class QuestionParser
    {
        private String gameFile;
        public String gameTitle;
        public int answerCount;
        public Question[] questions;
        public int questionCount;
        private String folder;

        public static String ErrorMessage;

        public QuestionParser(String gameFile)
        {
            this.gameFile = gameFile;
        }

        public Boolean loadQuiz()
        {
            Boolean succeeded = false;
            int location = -3;
            try
            {
                XmlDocument quiz = new XmlDocument();
                quiz.Load(gameFile);
                location++;

                XmlNode quizNode = quiz.GetElementsByTagName("Quiz")[0];
                if (quizNode.Attributes["folder"] != null)
                {
                    folder = quizNode.Attributes["folder"].Value;
                    if (!folder.EndsWith("/"))
                    {
                        folder += "/";
                    }
                    else { }
                }
                else
                {
                    folder = "";
                }
                location++;

                XmlNode titleNode = quiz.GetElementsByTagName("Title")[0];
                gameTitle = titleNode.InnerText;
                location++;

                XmlNodeList questionNodes = quiz.GetElementsByTagName("Question");
                questions = new Question[questionNodes.Count];
                int i = 0;
                foreach (XmlNode questionNode in questionNodes)
                {
                    BitmapImage questionImage;
                    questionImage = getImageFromNode(questionNode);
                    String questionString = questionNode.FirstChild.InnerText.Trim();
                    String[] answers = null;
                    BitmapImage[] answerImages = null;
                    int correctAnswer = -1;
                    int choices = int.Parse(questionNode.Attributes["choices"].Value);
                    switch (choices)
                    {
                        case 2:
                            answers = new String[2];
                            answerImages = new BitmapImage[2];
                            answers[0] = questionNode.SelectNodes("L")[0].InnerText;
                            answerImages[0] = getImageFromNode(questionNode.SelectNodes("L")[0]);
                            answers[1] = questionNode.SelectNodes("R")[0].InnerText;
                            answerImages[1] = getImageFromNode(questionNode.SelectNodes("R")[0]);
                            if (questionNode.Attributes["answer"].Value.Equals("L"))
                            {
                                correctAnswer = 0;
                            }
                            else // if (questionNode.Attributes["answer"].Value.Equals("L"))
                            {
                                correctAnswer = 1;
                            }
                            break;
                        case 4:
                            answers = new String[4];
                            answerImages = new BitmapImage[4];
                            answers[0] = questionNode.SelectNodes("A")[0].InnerText;
                            answerImages[0] = getImageFromNode(questionNode.SelectNodes("A")[0]);
                            answers[1] = questionNode.SelectNodes("B")[0].InnerText;
                            answerImages[1] = getImageFromNode(questionNode.SelectNodes("B")[0]);
                            answers[2] = questionNode.SelectNodes("X")[0].InnerText;
                            answerImages[2] = getImageFromNode(questionNode.SelectNodes("X")[0]);
                            answers[3] = questionNode.SelectNodes("Y")[0].InnerText;
                            answerImages[3] = getImageFromNode(questionNode.SelectNodes("Y")[0]);
                            if (questionNode.Attributes["answer"].Value.Equals("A"))
                            {
                                correctAnswer = 0;
                            }
                            else if (questionNode.Attributes["answer"].Value.Equals("B"))
                            {
                                correctAnswer = 1;
                            }
                            else if (questionNode.Attributes["answer"].Value.Equals("X"))
                            {
                                correctAnswer = 2;
                            }
                            else // if (questionNode.Attributes["answer"].Value.Equals("Y"))
                            {
                                correctAnswer = 3;
                            }
                            break;
                        default:
                            answers = new string[0];
                            correctAnswer = -1;
                            break;
                    }
                    Question question = new Question(questionString, questionImage, answers, answerImages, correctAnswer);
                    if (questionNode.Attributes["points"] != null)
                    {
                        question.usingCustomPoints = true;
                        question.customPoints = int.Parse(questionNode.Attributes["points"].Value);
                    }
                    else { }
                    if (questionNode.Attributes["penalty"] != null)
                    {
                        question.usingCustomPenalty = true;
                        question.customPenalty = int.Parse(questionNode.Attributes["penalty"].Value);
                    }
                    else { }
                    if (questionNode.Attributes["timer"] != null)
                    {
                        question.timed = true;
                        question.timer = int.Parse(questionNode.Attributes["timer"].Value);
                    }
                    else { }
                    if (questionNode.Attributes["must_answer"] != null)
                    {
                        question.mustAnswer = Boolean.Parse(questionNode.Attributes["must_answer"].Value);
                    }
                    else { }
                    if (questionNode.Attributes["can_shuffle_question"] != null)
                    {
                        question.canShuffleQuestion = Boolean.Parse(questionNode.Attributes["can_shuffle_question"].Value);
                    }
                    else { }
                    if (questionNode.Attributes["can_shuffle_answers"] != null)
                    {
                        question.canShuffleAnswers = Boolean.Parse(questionNode.Attributes["can_shuffle_answers"].Value);
                    }
                    else { }
                    questions[i++] = question;
                    location++;
                }
                if (Settings.ShuffleQuestions)
                {
                    questions = shuffleQuestions(questions);
                }
                else { }
                if (Settings.ShuffleAnswers)
                {
                    questions = shuffleAnswers(questions);
                }
                else { }
                succeeded = true;
            }
            catch (Exception)
            {
                if (location == -3)
                {
                    ErrorMessage = "Failed to open file. A tag might not be properly closed.";
                }
                else if (location == -2)
                {
                    ErrorMessage = "File malformed. Quiz tag is malformed.";
                }
                else if (location == -1)
                {
                    ErrorMessage = "File malformed. Title tag is malformed.";
                }
                else
                {
                    ErrorMessage = "File malformed. Question #" + (location+1) + " is malformed.";
                }
                Console.WriteLine(ErrorMessage);
            }
            return succeeded;
        }

        private BitmapImage getImageFromNode(XmlNode node)
        {
            BitmapImage r;
            if (node.Attributes["image"] != null)
            {
                r = new BitmapImage(new Uri(System.IO.Path.GetFullPath(folder + node.Attributes["image"].Value), UriKind.Absolute));
            }
            else
            {
                r = null;
            }
            return r;
        }

        private Question[] shuffleQuestions(Question[] questions)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            Question[] shuffledQuestions = new Question[questions.Length];
            int[] shuffledQuestionIndices = Enumerable.Range(0, questions.Length).ToArray();
            shuffledQuestionIndices = shuffledQuestionIndices.OrderBy(x => random.Next()).ToArray();

            for (int i = 0; i < questions.Length; i++)
            {
                shuffledQuestions[i] = questions[shuffledQuestionIndices[i]];
            }

            for (int i = 0; i < questions.Length; i++)
            {
                if (!questions[i].canShuffleQuestion)
                {
                    int index = -1;
                    int j = 0;
                    for (; j < questions.Length; j++)
                    {
                        if (shuffledQuestionIndices[j] == i)
                        {
                            index = j;
                            break;
                        }
                        else { }
                    }
                    shuffledQuestions[index] = shuffledQuestions[i];
                    shuffledQuestions[i] = questions[i];
                }
                else { }
            }

            return shuffledQuestions;
        }

        private Question[] shuffleAnswers(Question[] questions)
        {
            Random random = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < questions.Length; i++)
            {
                if (questions[i].canShuffleAnswers)
                {
                    String[] shuffledAnswers = new String[questions[i].answerCount];
                    BitmapImage[] shuffledAnswerImages = new BitmapImage[questions[i].answerCount];
                    int[] shuffledAnswerIndices = Enumerable.Range(0, questions[i].answerCount).ToArray();
                    shuffledAnswerIndices = shuffledAnswerIndices.OrderBy(x => random.Next()).ToArray();
                    int shuffledCorrectAnswer = -1;
                    for (int j = 0; j < questions[i].answerCount; j++)
                    {
                        shuffledAnswers[j] = questions[i].answers[shuffledAnswerIndices[j]];
                        shuffledAnswerImages[j] = questions[i].answerImages[shuffledAnswerIndices[j]];
                        if (shuffledAnswerIndices[j] == questions[i].correctAnswer)
                        {
                            shuffledCorrectAnswer = j;
                        }
                        else { }
                    }
                    questions[i].answers = shuffledAnswers;
                    questions[i].answerImages = shuffledAnswerImages;
                    questions[i].correctAnswer = shuffledCorrectAnswer;
                }
            }

            return questions;
        }
    }
}
