using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for ShowQuestionControl.xaml
    /// </summary>
    public partial class ShowQuestionControl : UserControl
    {
        public Boolean IsComplete;

        private Question question;
        private int answerCount;

        private MediaPlayer correctMediaPlayer;
        private MediaPlayer answeredMediaPlayer;
        private MediaPlayer wrongMediaPlayer;

        private static BitmapImage twoButtons = new BitmapImage(new Uri(@"assets/LR.png", UriKind.Relative));
        private static BitmapImage fourButtons = new BitmapImage(new Uri(@"assets/ABXYTilt.png", UriKind.Relative));

        private DispatcherTimer timeOutStartTimer;
        private DispatcherTimer timeOutTimer;

        private DispatcherTimer endTimer;
        private const int END_TIMER = 100;
        private const int TICK_SPEED = 100;
        private Boolean timerStarted;

        private int time;
        private int timeLeft;

        private int playersAnswered;
        private Boolean allMustAnswer;

        public ShowQuestionControl(Question question)
        {
            InitializeComponent();

            endTimer = new DispatcherTimer();
            endTimer.Interval = new TimeSpan(0, 0, 0, 0, END_TIMER);
            endTimer.Tick += new EventHandler(timer_End);
            timerStarted = false;

            timeOutStartTimer = new DispatcherTimer();
            timeOutStartTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.TimeOutStartTime);
            timeOutStartTimer.Tick += new EventHandler(timeOutStartTimer_Tick);

            timeOutTimer = new DispatcherTimer();
            timeOutTimer.Interval = new TimeSpan(0, 0, 0, 0, TICK_SPEED);
            timeOutTimer.Tick += new EventHandler(timeOutTimer_Tick);
            TimerProgressBar.Value = 100;

            playersAnswered = 0;

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
            allMustAnswer = question.mustAnswer;

            correctMediaPlayer = new MediaPlayer();
            answeredMediaPlayer = new MediaPlayer();
            wrongMediaPlayer = new MediaPlayer();

            for (int i = 0; i < 5; i++)
            {
                MainWindow.getInstance().GetPlayerControl(i).canAnswer = true;
            }

            if (Settings.QuestionsTimed || question.timed)
            {
                if (question.timed)
                {
                    time = question.timer * 1000;
                }
                else
                {
                    time = Settings.QuestionTime * 1000;
                }
                timeLeft = time;
                TimerProgressBar.Visibility = System.Windows.Visibility.Visible;
                timeOutStartTimer.Start();
            }
            else
            {
                TimerProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }

            setScale(MainWindow.getInstance().getXScale(), MainWindow.getInstance().getYScale());

            IsComplete = false;
        }

        public void update()
        {
            if (!timerStarted)
            {
                for (int i = 0; i < 5; i++)
                {
                    checkAnswer(i);
                }
            }
            else { }
        }

        private void setAllAnswers(int answer)
        {
            for (int i = 0; i < 5; i++)
            {
                checkButton(MainWindow.getInstance().GetPlayerControl(i), answer);
            }
        }

        private void checkAnswer(int index)
        {
            PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(index);
            if (playerControl.canAnswer)
            {
                switch (answerCount)
                {
                    case 2:
                        if (playerControl.IsLDown())
                        {
                            checkButton(playerControl, 0);
                        }
                        else if (playerControl.IsRDown())
                        {
                            checkButton(playerControl, 1);
                        }
                        else { }
                        break;
                    case 4:
                        if (playerControl.IsADown())
                        {
                            checkButton(playerControl, 0);
                        }
                        else if (playerControl.IsBDown())
                        {
                            checkButton(playerControl, 1);
                        }
                        else if (playerControl.IsXDown())
                        {
                            checkButton(playerControl, 2);
                        }
                        else if (playerControl.IsYDown())
                        {
                            checkButton(playerControl, 3);
                        }
                        else { }
                        break;
                }
            }
            else { }
        }

        private void checkButton(PlayerControl playerControl, int index)
        {
            if (playerControl.inGame && !timerStarted)
            {
                playerControl.canAnswer = false;
                playersAnswered++;
                if (index < answerCount && question.correctAnswer == index)
                {
                    playerControl.setAnswered(PlayerControl.AnswerType.correct);
                    if (Settings.ShowAnswers || allMustAnswer)
                    {
                        playerControl.rightRumble();
                        correctMediaPlayer.Stop();
                        correctMediaPlayer.Open(new Uri(@"assets/correct.mp3", UriKind.Relative));
                        correctMediaPlayer.Play();
                    }
                    else
                    {
                        playerControl.answerRumble();
                        answeredMediaPlayer.Stop();
                        answeredMediaPlayer.Open(new Uri(@"assets/answered.mp3", UriKind.Relative));
                        answeredMediaPlayer.Play();
                    }
                    if (!Settings.WaitForAllPlayers)
                    {
                        endTimer.Start();
                        timerStarted = true;
                    }
                    else { }
                }
                else
                {
                    playerControl.setAnswered(PlayerControl.AnswerType.wrong);
                    if (Settings.ShowAnswers || allMustAnswer)
                    {
                        playerControl.wrongRumble();
                        wrongMediaPlayer.Stop();
                        wrongMediaPlayer.Open(new Uri(@"assets/wrong.mp3", UriKind.Relative));
                        wrongMediaPlayer.Play();
                    }
                    else
                    {
                        playerControl.answerRumble();
                        answeredMediaPlayer.Stop();
                        answeredMediaPlayer.Open(new Uri(@"assets/answered.mp3", UriKind.Relative));
                        answeredMediaPlayer.Play();
                    }
                }
                if (playersAnswered == App.getInstance().players)
                {
                    endTimer.Start();
                    timerStarted = true;
                }
                else { }
            }
            else { }
        }

        private void timer_End(object sender, EventArgs e)
        {
            App.getInstance().xKeyboard.OnAShiftPressed = null;
            App.getInstance().xKeyboard.OnBShiftPressed = null;
            App.getInstance().xKeyboard.OnXShiftPressed = null;
            App.getInstance().xKeyboard.OnYShiftPressed = null;
            App.getInstance().xKeyboard.OnLShiftPressed = null;
            App.getInstance().xKeyboard.OnRShiftPressed = null;
            endTimer.Stop();
            IsComplete = true;
        }

        private void timeOutStartTimer_Tick(object sender, EventArgs e)
        {
            timeOutStartTimer.Stop();
            timeOutTimer.Start();
        }

        private void timeOutTimer_Tick(object sender, EventArgs e)
        {
            timeLeft -= TICK_SPEED;
            int progress = (100 * timeLeft) / time;
            TimerProgressBar.Value = progress;
            if (TimerProgressBar.Value <= 0)
            {
                timeOutTimer.Stop();
                TimerProgressBar.Value = 0;
                endTimer.Start();
                timerStarted = true;
                if (allMustAnswer)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        PlayerControl player = MainWindow.getInstance().GetPlayerControl(i);
                        if (player.canAnswer)
                        {
                            player.setAnswered(PlayerControl.AnswerType.wrong);
                            player.wrongRumble();
                        }
                        else { }
                    }
                }
                else { }
            }
            else { }
        }

        private void setAnswers(Question question)
        {
            answerCount = question.answers.Length;
            if (answerCount == 2)
            {
                AnswerAPanel.Visibility = System.Windows.Visibility.Collapsed;
                AnswerBPanel.Visibility = System.Windows.Visibility.Collapsed;
                AnswerXPanel.Visibility = System.Windows.Visibility.Collapsed;
                AnswerYPanel.Visibility = System.Windows.Visibility.Collapsed;
                AnswerLTextBox.Text = question.answers[0];
                AnswerLImage.Source = question.answerImages[0];
                AnswerRTextBox.Text = question.answers[1];
                AnswerRImage.Source = question.answerImages[1];

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

                App.getInstance().xKeyboard.OnLShiftPressed += () => setAllAnswers(0);
                App.getInstance().xKeyboard.OnRShiftPressed += () => setAllAnswers(1);
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
                AnswerLPanel.Visibility = System.Windows.Visibility.Collapsed;
                AnswerRPanel.Visibility = System.Windows.Visibility.Collapsed;

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

                App.getInstance().xKeyboard.OnAShiftPressed += () => setAllAnswers(0);
                App.getInstance().xKeyboard.OnBShiftPressed += () => setAllAnswers(1);
                App.getInstance().xKeyboard.OnXShiftPressed += () => setAllAnswers(2);
                App.getInstance().xKeyboard.OnYShiftPressed += () => setAllAnswers(3);
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
            TimerProgressBar.Height = 20 * yScale;
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
