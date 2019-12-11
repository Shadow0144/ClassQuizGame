using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for AnswerBox.xaml
    /// </summary>
    public partial class AnswerBox : UserControl
    {
        public enum Answer
        {
            None,
            A,
            B,
            X,
            Y,
            L,
            R
        };
        private Answer answer;
        
        private Color colorA = Color.FromArgb(255, 225, 255, 225);
        private Color colorB = Color.FromArgb(255, 255, 225, 225);
        private Color colorX = Color.FromArgb(255, 225, 225, 255);
        private Color colorY = Color.FromArgb(255, 255, 255, 225);
        private Color colorL = Color.FromArgb(255, 225, 255, 225);
        private Color colorR = Color.FromArgb(255, 255, 225, 225);

        private Color colorCorrect = Color.FromArgb(255, 75, 255, 75);
        private Color colorWrong = Color.FromArgb(255, 255, 75, 75);

        private Boolean onLeftSide;

        public AnswerBox()
        {
            InitializeComponent();
        }

        public void setAnswer(Answer answer)
        {
            this.answer = answer;

            SolidColorBrush fillBrush = new SolidColorBrush();
            switch (answer)
            {
                case Answer.A:
                    onLeftSide = false;
                    AnswerGrid.HorizontalAlignment = HorizontalAlignment.Left;
                    fillBrush.Color = colorA;
                    AnswerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case Answer.B:
                    onLeftSide = false;
                    AnswerGrid.HorizontalAlignment = HorizontalAlignment.Left;
                    fillBrush.Color = colorB;
                    AnswerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case Answer.X:
                    onLeftSide = true;
                    AnswerGrid.HorizontalAlignment = HorizontalAlignment.Right;
                    fillBrush.Color = colorX;
                    AnswerTextBox.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case Answer.Y:
                    onLeftSide = true;
                    AnswerGrid.HorizontalAlignment = HorizontalAlignment.Right;
                    fillBrush.Color = colorY;
                    AnswerTextBox.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case Answer.L:
                    onLeftSide = true;
                    AnswerGrid.HorizontalAlignment = HorizontalAlignment.Right;
                    fillBrush.Color = colorL;
                    AnswerTextBox.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case Answer.R:
                    onLeftSide = false;
                    AnswerGrid.HorizontalAlignment = HorizontalAlignment.Left;
                    fillBrush.Color = colorR;
                    AnswerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
            }
            AnswerRectangle.Fill = fillBrush;
        }

        public void setAnswerContent(String text, ImageSource source)
        {
            setText(text);
            setImageSource(source);
        }

        public void setText(String text)
        {
            AnswerTextBox.Text = text;
            if (text.Length == 0)
            {
                AnswerTextBox.Visibility = System.Windows.Visibility.Collapsed;
            }
            else { }
        }

        public void setImageSource(ImageSource source)
        {
            switch (answer)
            {
                case Answer.A:
                case Answer.B:
                case Answer.R:
                    LeftAnswerImage.Visibility = System.Windows.Visibility.Collapsed;
                    RightAnswerImage.Source = source;
                    break;
                case Answer.X:
                case Answer.Y:
                case Answer.L:
                    RightAnswerImage.Visibility = System.Windows.Visibility.Collapsed;
                    LeftAnswerImage.Source = source;
                    break;
            }
            if (source == null)
            {
                LeftAnswerImage.Visibility = System.Windows.Visibility.Collapsed;
                RightAnswerImage.Visibility = System.Windows.Visibility.Collapsed;
            }
            else { }
        }

        public void setCorrect()
        {
            SolidColorBrush fillBrush = new SolidColorBrush();
            fillBrush.Color = colorCorrect;
            AnswerRectangle.Fill = fillBrush;
        }

        public void setWrong()
        {
            SolidColorBrush fillBrush = new SolidColorBrush();
            fillBrush.Color = colorWrong;
            AnswerRectangle.Fill = fillBrush;
        }

        public void showAnswers()
        {
            if (onLeftSide)
            {
                LeftAnswerRect.Visibility = Visibility.Visible;
                LeftAnswerStack.Visibility = Visibility.Visible;
                for (int i = 0; i < 5; i++)
                {
                    if (MainWindow.getInstance().GetPlayerControl(i).inGame)
                    {
                        switch (i)
                        {
                            case 0:
                                PlayerKLRect.Visibility = Visibility.Hidden;
                                break;
                            case 1:
                                Player1LRect.Visibility = Visibility.Hidden;
                                break;
                            case 2:
                                Player2LRect.Visibility = Visibility.Hidden;
                                break;
                            case 3:
                                Player3LRect.Visibility = Visibility.Hidden;
                                break;
                            case 4:
                                Player4LRect.Visibility = Visibility.Hidden;
                                break;
                        }
                    }
                    else { }
                }
            }
            else
            {
                RightAnswerRect.Visibility = Visibility.Visible;
                RightAnswerStack.Visibility = Visibility.Visible;
                for (int i = 0; i < 5; i++)
                {
                    if (MainWindow.getInstance().GetPlayerControl(i).inGame)
                    {
                        switch (i)
                        {
                            case 0:
                                PlayerKRRect.Visibility = Visibility.Hidden;
                                break;
                            case 1:
                                Player1RRect.Visibility = Visibility.Hidden;
                                break;
                            case 2:
                                Player2RRect.Visibility = Visibility.Hidden;
                                break;
                            case 3:
                                Player3RRect.Visibility = Visibility.Hidden;
                                break;
                            case 4:
                                Player4RRect.Visibility = Visibility.Hidden;
                                break;
                        }
                    }
                    else { }
                }
            }
        }

        public void setAnswered(int player)
        {
            if (onLeftSide)
            {
                switch (player)
                {
                    case 0:
                        PlayerKLRect.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        Player1LRect.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        Player2LRect.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        Player3LRect.Visibility = Visibility.Visible;
                        break;
                    case 4:
                        Player4LRect.Visibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                switch (player)
                {
                    case 0:
                        PlayerKRRect.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        Player1RRect.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        Player2RRect.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        Player3RRect.Visibility = Visibility.Visible;
                        break;
                    case 4:
                        Player4RRect.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        public void setScale(float xScale, float yScale)
        {
            AnswerTextBox.FontSize = 24 * yScale;
            if (LeftAnswerImage.Source != null)
            {
                LeftAnswerImage.Width = 120 * xScale;
                LeftAnswerImage.Height = 120 * yScale;
            }
            else { }
            if (RightAnswerImage.Source != null)
            {
                RightAnswerImage.Width = 120 * xScale;
                RightAnswerImage.Height = 120 * yScale;
            }
            else { }

            PlayerKLRect.Width = 20 * xScale;
            PlayerKLRect.Height = 10 * yScale;
            PlayerKRRect.Width = 20 * xScale;
            PlayerKRRect.Height = 10 * yScale;

            Player1LRect.Width = 20 * xScale;
            Player1LRect.Height = 10 * yScale;
            Player1RRect.Width = 20 * xScale;
            Player1RRect.Height = 10 * yScale;

            Player2LRect.Width = 20 * xScale;
            Player2LRect.Height = 10 * yScale;
            Player2RRect.Width = 20 * xScale;
            Player2RRect.Height = 10 * yScale;

            Player3LRect.Width = 20 * xScale;
            Player3LRect.Height = 10 * yScale;
            Player3RRect.Width = 20 * xScale;
            Player3RRect.Height = 10 * yScale;

            Player4LRect.Width = 20 * xScale;
            Player4LRect.Height = 10 * yScale;
            Player4RRect.Width = 20 * xScale;
            Player4RRect.Height = 10 * yScale;
        }
    }
}
