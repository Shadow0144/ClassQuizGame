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

            int topScore = int.MinValue;
            for (int index = 0; index < 5; index++)
            {
                PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(index);
                if (playerControl.lastAnswer == PlayerControl.AnswerType.correct)
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
                else if (playerControl.lastAnswer == PlayerControl.AnswerType.wrong)
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
                else {  }
                playerControl.revealAnswer();
                if (playerControl.score > topScore)
                {
                    topScore = playerControl.score;
                }
                playerControl.AnimatePointsStoryboard.Begin();
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
                AnswerAPanel.Visibility = Visibility.Collapsed;
                AnswerBPanel.Visibility = Visibility.Collapsed;
                AnswerXPanel.Visibility = Visibility.Collapsed;
                AnswerYPanel.Visibility = Visibility.Collapsed;
                AnswerLTextBox.Text = question.answers[0];
                AnswerLImage.Source = question.answerImages[0];
                AnswerRTextBox.Text = question.answers[1];
                AnswerRImage.Source = question.answerImages[1];

                switch (question.correctAnswer)
                {
                    case 0:
                        AnswerLRectangle.Fill = correctBrush;
                        AnswerLRectangle.Stroke = correctStroke;
                        AnswerRRectangle.Fill = incorrectBrush;
                        AnswerRRectangle.Stroke = incorrectStroke;
                        break;
                    case 1:
                        AnswerLRectangle.Fill = incorrectBrush;
                        AnswerLRectangle.Stroke = incorrectStroke;
                        AnswerRRectangle.Fill = correctBrush;
                        AnswerRRectangle.Stroke = correctStroke;
                        break;
                }

                if (question.answers[0].Length == 0)
                {
                    AnswerLTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answerImages[0] == null)
                {
                    AnswerLImage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answers[1].Length == 0)
                {
                    AnswerRTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answerImages[1] == null)
                {
                    AnswerRImage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                ButtonsImage.Source = twoButtons;
            }
            else if (answerCount == 4)
            {
                AnswerATextBox.Text = question.answers[0];
                AnswerAImage.Source = question.answerImages[0];
                AnswerBTextBox.Text = question.answers[1];
                AnswerBImage.Source = question.answerImages[1];
                AnswerXTextBox.Text = question.answers[2];
                AnswerXImage.Source = question.answerImages[2];
                AnswerYTextBox.Text = question.answers[3];
                AnswerYImage.Source = question.answerImages[3];
                AnswerLPanel.Visibility = Visibility.Collapsed;
                AnswerRPanel.Visibility = Visibility.Collapsed;

                switch (question.correctAnswer)
                {
                    case 0:
                        AnswerARectangle.Fill = correctBrush;
                        AnswerARectangle.Stroke = correctStroke;
                        AnswerBRectangle.Fill = incorrectBrush;
                        AnswerBRectangle.Stroke = incorrectStroke;
                        AnswerXRectangle.Fill = incorrectBrush;
                        AnswerXRectangle.Stroke = incorrectStroke;
                        AnswerYRectangle.Fill = incorrectBrush;
                        AnswerYRectangle.Stroke = incorrectStroke;
                        break;
                    case 1:
                        AnswerARectangle.Fill = incorrectBrush;
                        AnswerARectangle.Stroke = incorrectStroke;
                        AnswerBRectangle.Fill = correctBrush;
                        AnswerBRectangle.Stroke = correctStroke;
                        AnswerXRectangle.Fill = incorrectBrush;
                        AnswerXRectangle.Stroke = incorrectStroke;
                        AnswerYRectangle.Fill = incorrectBrush;
                        AnswerYRectangle.Stroke = incorrectStroke;
                        break;
                    case 2:
                        AnswerARectangle.Fill = incorrectBrush;
                        AnswerARectangle.Stroke = incorrectStroke;
                        AnswerBRectangle.Fill = incorrectBrush;
                        AnswerBRectangle.Stroke = incorrectStroke;
                        AnswerXRectangle.Fill = correctBrush;
                        AnswerXRectangle.Stroke = correctStroke;
                        AnswerYRectangle.Fill = incorrectBrush;
                        AnswerYRectangle.Stroke = incorrectStroke;
                        break;
                    case 3:
                        AnswerARectangle.Fill = incorrectBrush;
                        AnswerARectangle.Stroke = incorrectStroke;
                        AnswerBRectangle.Fill = incorrectBrush;
                        AnswerBRectangle.Stroke = incorrectStroke;
                        AnswerXRectangle.Fill = incorrectBrush;
                        AnswerXRectangle.Stroke = incorrectStroke;
                        AnswerYRectangle.Fill = correctBrush;
                        AnswerYRectangle.Stroke = correctStroke;
                        break;
                }

                if (question.answers[0].Length == 0)
                {
                    AnswerATextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answerImages[0] == null)
                {
                    AnswerAImage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answers[1].Length == 0)
                {
                    AnswerBTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answerImages[1] == null)
                {
                    AnswerBImage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answers[2].Length == 0)
                {
                    AnswerXTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answerImages[2] == null)
                {
                    AnswerXImage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answers[3].Length == 0)
                {
                    AnswerYTextBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
                if (question.answerImages[3] == null)
                {
                    AnswerYImage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else { }
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
            int answerFontSize = 24;
            AnswerATextBox.FontSize = answerFontSize * yScale;
            AnswerBTextBox.FontSize = answerFontSize * yScale;
            AnswerXTextBox.FontSize = answerFontSize * yScale;
            AnswerYTextBox.FontSize = answerFontSize * yScale;
            AnswerLTextBox.FontSize = answerFontSize * yScale;
            AnswerRTextBox.FontSize = answerFontSize * yScale;
            ButtonsImage.Height = 128 * yScale;
            int imageWidth = 120;
            int imageHeight = 120;
            if (AnswerLImage.Source != null) // L
            {
                AnswerLImage.Width = imageWidth * xScale;
                AnswerLImage.Height = imageHeight * yScale;
            }
            else { }
            if (AnswerRImage.Source != null) // R
            {
                AnswerRImage.Width = imageWidth * xScale;
                AnswerRImage.Height = imageHeight * yScale;
            }
            else { }
            if (AnswerAImage.Source != null) // A
            {
                AnswerAImage.Width = imageWidth * xScale;
                AnswerAImage.Height = imageHeight * yScale;
            }
            else { }
            if (AnswerBImage.Source != null) // B
            {
                AnswerBImage.Width = imageWidth * xScale;
                AnswerBImage.Height = imageHeight * yScale;
            }
            else { }
            if (AnswerXImage.Source != null) // X
            {
                AnswerXImage.Width = imageWidth * xScale;
                AnswerXImage.Height = imageHeight * yScale;
            }
            else { }
            if (AnswerYImage.Source != null) // Y
            {
                AnswerYImage.Width = imageWidth * xScale;
                AnswerYImage.Height = imageHeight * yScale;
            }
            else { }
            PointsTextBlock.FontSize = 24 * yScale;
            PointsTextTranslate.Y = -2 * yScale;
            PenaltyTextBlock.FontSize = 24 * yScale;
            PenaltyTextTranslate.Y = -2 * yScale;
        }
    }
}
