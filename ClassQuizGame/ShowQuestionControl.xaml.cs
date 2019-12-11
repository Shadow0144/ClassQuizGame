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

        private void setAllAnswers(int answerIndex)
        {
            AnswerBox.Answer answer = AnswerBox.Answer.None;
            switch (question.answerCount)
            {
                case 2:
                    switch (answerIndex)
                    {
                        case 0:
                            answer = AnswerBox.Answer.L;
                            break;
                        case 1:
                            answer = AnswerBox.Answer.R;
                            break;
                    }
                    break;
                case 4:
                    switch (answerIndex)
                    {
                        case 0:
                            answer = AnswerBox.Answer.A;
                            break;
                        case 1:
                            answer = AnswerBox.Answer.B;
                            break;
                        case 2:
                            answer = AnswerBox.Answer.X;
                            break;
                        case 3:
                            answer = AnswerBox.Answer.Y;
                            break;
                    }
                    break;
            }
            for (int i = 0; i < 5; i++)
            {
                checkButton(MainWindow.getInstance().GetPlayerControl(i), answerIndex, answer);
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
                            checkButton(playerControl, 0, AnswerBox.Answer.L);
                        }
                        else if (playerControl.IsRDown())
                        {
                            checkButton(playerControl, 1, AnswerBox.Answer.R);
                        }
                        else { }
                        break;
                    case 4:
                        if (playerControl.IsADown())
                        {
                            checkButton(playerControl, 0, AnswerBox.Answer.A);
                        }
                        else if (playerControl.IsBDown())
                        {
                            checkButton(playerControl, 1, AnswerBox.Answer.B);
                        }
                        else if (playerControl.IsXDown())
                        {
                            checkButton(playerControl, 2, AnswerBox.Answer.X);
                        }
                        else if (playerControl.IsYDown())
                        {
                            checkButton(playerControl, 3, AnswerBox.Answer.Y);
                        }
                        else { }
                        break;
                }
            }
            else { }
        }

        private void checkButton(PlayerControl playerControl, int index, AnswerBox.Answer answer)
        {
            if (playerControl.inGame && !timerStarted)
            {
                playerControl.showAnswers = (Settings.GradeAnswersImmediately && !allMustAnswer);
                playerControl.canAnswer = false;
                playersAnswered++;
                if (index < answerCount && question.correctAnswer == index)
                {
                    playerControl.setAnswered(answer, PlayerControl.AnswerResult.correct);
                    if (Settings.GradeAnswersImmediately && !allMustAnswer)
                    {
                        playerControl.rightRumble();
                        if (!Settings.Mute)
                        {
                            correctMediaPlayer.Stop();
                            correctMediaPlayer.Open(new Uri(@"assets/correct.mp3", UriKind.Relative));
                            correctMediaPlayer.Play();
                        }
                        else { }
                    }
                    else
                    {
                        playerControl.answerRumble();
                        if (!Settings.Mute)
                        {
                            answeredMediaPlayer.Stop();
                            answeredMediaPlayer.Open(new Uri(@"assets/answered.mp3", UriKind.Relative));
                            answeredMediaPlayer.Play();
                        }
                        else { }
                    }
                    if (!Settings.WaitForAllPlayers && !allMustAnswer)
                    {
                        endTimer.Start();
                        timerStarted = true;
                    }
                    else { }
                }
                else
                {
                    playerControl.setAnswered(answer, PlayerControl.AnswerResult.wrong);
                    if (Settings.GradeAnswersImmediately && !allMustAnswer)
                    {
                        playerControl.wrongRumble();
                        if (!Settings.Mute)
                        {
                            wrongMediaPlayer.Stop();
                            wrongMediaPlayer.Open(new Uri(@"assets/wrong.mp3", UriKind.Relative));
                            wrongMediaPlayer.Play();
                        }
                        else { }
                    }
                    else
                    {
                        playerControl.answerRumble();
                        if (!Settings.Mute)
                        {
                            answeredMediaPlayer.Stop();
                            answeredMediaPlayer.Open(new Uri(@"assets/answered.mp3", UriKind.Relative));
                            answeredMediaPlayer.Play();
                        }
                        else { }
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
                        if (player.canAnswer && player.inGame)
                        {
                            player.setAnswered(AnswerBox.Answer.None, PlayerControl.AnswerResult.wrong);
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
                AnswerABox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerBBox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerXBox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerYBox.Visibility = System.Windows.Visibility.Collapsed;
                AnswerLBox.setAnswer(AnswerBox.Answer.L);
                AnswerRBox.setAnswer(AnswerBox.Answer.R);
                AnswerLBox.setAnswerContent(question.answers[0], question.answerImages[0]);
                AnswerRBox.setAnswerContent(question.answers[1], question.answerImages[1]);

                ButtonsImage.Source = twoButtons;

                App.getInstance().xKeyboard.OnLShiftPressed += () => setAllAnswers(0);
                App.getInstance().xKeyboard.OnRShiftPressed += () => setAllAnswers(1);
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

            AnswerABox.setScale(xScale, yScale);
            AnswerBBox.setScale(xScale, yScale);
            AnswerXBox.setScale(xScale, yScale);
            AnswerYBox.setScale(xScale, yScale);
            AnswerLBox.setScale(xScale, yScale);
            AnswerRBox.setScale(xScale, yScale);

            ButtonsImage.Height = 128 * yScale;
            TimerProgressBar.Height = 20 * yScale;

            PointsTextBlock.FontSize = 24 * yScale;
            PointsTextTranslate.Y = -2 * yScale;
            PenaltyTextBlock.FontSize = 24 * yScale;
            PenaltyTextTranslate.Y = -2 * yScale;
        }
    }
}
