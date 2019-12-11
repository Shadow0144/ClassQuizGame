using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for ShowAnswerControl.xaml
    /// </summary>
    public partial class ShowAnswerControl : UserControl
    {
        public Boolean IsComplete;

        private Question question;

        private static SolidColorBrush correctBrush = new SolidColorBrush(Color.FromArgb(125, 0, 255, 0));
        private static SolidColorBrush correctStroke = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        private static SolidColorBrush incorrectBrush = new SolidColorBrush(Color.FromArgb(125, 255, 0, 0));
        private static SolidColorBrush incorrectStroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        private static BitmapImage twoButtons = new BitmapImage(new Uri(@"assets/LR.png", UriKind.Relative));
        private static BitmapImage fourButtons = new BitmapImage(new Uri(@"assets/ABXYTilt.png", UriKind.Relative));

        private int answerCount;

        private Boolean playersReady;

        public ShowAnswerControl(Question question)
        {
            InitializeComponent();

            this.question = question;
            QuestionImage.Source = question.questionImage;
            QuestionTextBlock.Text = question.question;
            if (question.question.Length == 0)
            {
                QuestionTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            else { }
            if (question.questionImage == null)
            {
                QuestionImage.Visibility = System.Windows.Visibility.Collapsed;
            }
            else { }

            setAnswers(question);

            if (question.timed)
            {
                TimerProgressBar.Visibility = Visibility.Hidden;
            }
            else
            {
                TimerProgressBar.Visibility = Visibility.Collapsed;
            }

            if (question.usingCustomPoints)
            {
                PointsTextBlock.Text = "+" + question.customPoints;
                PointsGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                if (Settings.AlwaysShowPoints)
                {
                    PointsTextBlock.Text = "+" + Settings.RightPoints;
                    PointsGrid.Visibility = System.Windows.Visibility.Visible;
                }
                else { }
            }
            if (question.usingCustomPenalty)
            {
                if (question.customPenalty > 0)
                {
                    PenaltyTextBlock.Text = "+" + question.customPenalty;
                }
                else
                {
                    PenaltyTextBlock.Text = "" + question.customPenalty;
                }
                PenaltyGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                if (Settings.AlwaysShowPenalties)
                {
                    if (Settings.WrongPoints > 0)
                    {
                        PenaltyTextBlock.Text = "+" + Settings.WrongPoints;
                    }
                    else
                    {
                        PenaltyTextBlock.Text = "" + Settings.WrongPoints;
                    }
                    PenaltyGrid.Visibility = System.Windows.Visibility.Visible;
                }
                else { }
            }

            if (Settings.ShowAnswers)
            {
                AnswerABox.showAnswers();
                AnswerBBox.showAnswers();
                AnswerXBox.showAnswers();
                AnswerYBox.showAnswers();
                AnswerLBox.showAnswers();
                AnswerRBox.showAnswers();
            }
            else { }

            int topScore = int.MinValue;
            for (int index = 0; index < 5; index++)
            {
                PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(index);
                if (playerControl.inGame)
                {
                    if (playerControl.getLastAnswerResult() == PlayerControl.AnswerResult.correct)
                    {
                        if (question.usingCustomPoints)
                        {
                            playerControl.addScore(question.customPoints, true);
                        }
                        else
                        {
                            playerControl.addScore(Settings.RightPoints, true);
                        }
                    }
                    else if (playerControl.getLastAnswerResult() == PlayerControl.AnswerResult.wrong)
                    {
                        if (question.usingCustomPenalty)
                        {
                            playerControl.addScore(question.customPenalty, false);
                        }
                        else
                        {
                            if (Settings.WrongDeduct)
                            {
                                playerControl.addScore(Settings.WrongPoints, false);
                            }
                            else { }
                        }
                    }
                    else { }
                    playerControl.revealAnswer();
                    if (Settings.ShowAnswers)
                    {
                        switch (playerControl.lastAnswer)
                        {
                            case AnswerBox.Answer.None:
                                break;
                            case AnswerBox.Answer.A:
                                AnswerABox.setAnswered(index);
                                break;
                            case AnswerBox.Answer.B:
                                AnswerBBox.setAnswered(index);
                                break;
                            case AnswerBox.Answer.X:
                                AnswerXBox.setAnswered(index);
                                break;
                            case AnswerBox.Answer.Y:
                                AnswerYBox.setAnswered(index);
                                break;
                            case AnswerBox.Answer.L:
                                AnswerLBox.setAnswered(index);
                                break;
                            case AnswerBox.Answer.R:
                                AnswerRBox.setAnswered(index);
                                break;
                        }
                    }
                    else { }
                    if (playerControl.score > topScore)
                    {
                        topScore = playerControl.score;
                    }
                    else { }
                    playerControl.AnimatePointsStoryboard.Begin();
                }
                else { }
            }
            for (int index = 0; index < 5; index++)
            {
                PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(index);
                if (playerControl.score == topScore)
                {
                    playerControl.showSilverCrown();
                }
                else
                {
                    playerControl.hideCrown();
                }
            }

            playersReady = false;

            App.getInstance().xKeyboard.OnSpacePressed += () => finish();

            setScale(MainWindow.getInstance().getXScale(), MainWindow.getInstance().getYScale());

            IsComplete = false;
        }

        public void update()
        {
            if (!playersReady)
            {
                int readiedPlayers = 0;
                for (int i = 0; i < 5; i++)
                {
                    PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(i);
                    if (playerControl.inGame && playerControl.readied)
                    {
                        readiedPlayers++;
                    }
                    else { }
                }
                if (readiedPlayers == App.getInstance().players)
                {
                    playersReady = true;
                    if (!Settings.RequireProctor)
                    {
                        finish();
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        private void finish()
        {
            if (!Settings.RequirePlayers || playersReady)
            {
                for (int index = 0; index < 5; index++)
                {
                    PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(index);
                    playerControl.resetScoreDisplay();
                }
                App.getInstance().xKeyboard.OnSpacePressed = null;
                IsComplete = true;
            }
            else { }
        }

        private void setAnswers(Question question)
        {
            answerCount = question.answers.Length;
            if (answerCount == 2)
            {
                AnswerABox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerBBox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerXBox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerYBox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerLBox.setAnswer(AnswerBox.Answer.L);
                AnswerRBox.setAnswer(AnswerBox.Answer.R);
                AnswerLBox.setAnswerContent(question.answers[0], question.answerImages[0]);
                AnswerRBox.setAnswerContent(question.answers[1], question.answerImages[1]);

                switch (question.correctAnswer)
                {
                    case 0:
                        AnswerLBox.setCorrect();
                        AnswerRBox.setWrong();
                        break;
                    case 1:
                        AnswerLBox.setWrong();
                        AnswerRBox.setCorrect();
                        break;
                }

                ButtonsImage.Source = twoButtons;
            }
            else if (answerCount == 4)
            {
                AnswerABox.setAnswer(AnswerBox.Answer.A);
                AnswerBBox.setAnswer(AnswerBox.Answer.B);
                AnswerXBox.setAnswer(AnswerBox.Answer.X);
                AnswerYBox.setAnswer(AnswerBox.Answer.Y);
                AnswerABox.setAnswerContent(question.answers[0], question.answerImages[0]);
                AnswerBBox.setAnswerContent(question.answers[1], question.answerImages[1]);
                AnswerXBox.setAnswerContent(question.answers[2], question.answerImages[2]);
                AnswerYBox.setAnswerContent(question.answers[3], question.answerImages[3]);
                AnswerLBox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerRBox.Visibility = System.Windows.Visibility.Collapsed;

                switch (question.correctAnswer)
                {
                    case 0:
                        AnswerABox.setCorrect();
                        AnswerBBox.setWrong();
                        AnswerXBox.setWrong();
                        AnswerYBox.setWrong();
                        break;
                    case 1:
                        AnswerABox.setWrong();
                        AnswerBBox.setCorrect();
                        AnswerXBox.setWrong();
                        AnswerYBox.setWrong();
                        break;
                    case 2:
                        AnswerABox.setWrong();
                        AnswerBBox.setWrong();
                        AnswerXBox.setCorrect();
                        AnswerYBox.setWrong();
                        break;
                    case 3:
                        AnswerABox.setWrong();
                        AnswerBBox.setWrong();
                        AnswerXBox.setWrong();
                        AnswerYBox.setCorrect();
                        break;
                }

                ButtonsImage.Source = fourButtons;
            }
            else { }
        }

        public void setScale(float xScale, float yScale)
        {
            if (QuestionImage.Source != null)
            {
                if (question.question.Length != 0)
                {
                    QuestionImage.Width = 512 * xScale;
                    QuestionImage.Height = 96 * yScale;
                }
                else
                {
                    QuestionImage.Width = 512 * xScale;
                    QuestionImage.Height = 120 * yScale;
                }
            }
            else { }
            QuestionTextBlock.FontSize = 32 * yScale;

            AnswerABox.setScale(xScale, yScale);
            AnswerBBox.setScale(xScale, yScale);
            AnswerXBox.setScale(xScale, yScale);
            AnswerYBox.setScale(xScale, yScale);
            AnswerLBox.setScale(xScale, yScale);
            AnswerRBox.setScale(xScale, yScale);

            ButtonsImage.Height = 128 * yScale;
            
            PointsTextBlock.FontSize = 24 * yScale;
            PointsTextTranslate.Y = -2 * yScale;
            PenaltyTextBlock.FontSize = 24 * yScale;
            PenaltyTextTranslate.Y = -2 * yScale;
        }
    }
}
