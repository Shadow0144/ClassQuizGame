using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for PromptForReadyControl.xaml
    /// </summary>
    public partial class PromptForReadyControl : UserControl
    {
        public Boolean IsComplete;

        private Boolean startTimer;
        private DispatcherTimer timer;

        private const int PROGRESS = 10;
        private const int MILLI = 100;

        private Boolean playersReady;

        private static BitmapImage twoButtons = new BitmapImage(new Uri(@"assets/LR.png", UriKind.Relative));
        private static BitmapImage fourButtons = new BitmapImage(new Uri(@"assets/ABXY.png", UriKind.Relative));

        public PromptForReadyControl(Question question)
        {
            InitializeComponent();

            IsComplete = false;
            startTimer = false;
            playersReady = false;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, MILLI);
            timer.Tick += new EventHandler(timer_Tick);

            setScale(MainWindow.getInstance().getXScale(), MainWindow.getInstance().getYScale());

            switch (question.answerCount)
            {
                case 2:
                    TwoButtonsImage.Visibility = Visibility.Visible;
                    FourButtonsImage.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    TwoButtonsImage.Visibility = Visibility.Collapsed;
                    FourButtonsImage.Visibility = Visibility.Visible;
                    break;
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
            if (question.mustAnswer)
            {
                AllMustAnswerTextBlock.Visibility = Visibility.Visible;
            }
            else { }
            if (question.timed)
            {
                TimerStackPanel.Visibility = Visibility.Visible;
                TimerTextBlock.Text = "Timer: " + question.timer + " Seconds";
            }
            else { }

            App.getInstance().xKeyboard.OnSpacePressed += () => finish();
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
                if (!startTimer)
                {
                    startTimer = true;
                    timer.Start();
                }
                else { }
            }
            else { }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            StartBar.Value = Math.Min(100, StartBar.Value + PROGRESS);
            if (StartBar.Value == 100)
            {
                App.getInstance().xKeyboard.OnSpacePressed = null;
                IsComplete = true;
                timer.Stop();
            }
            else { }
        }

        public void setScale(float xScale, float yScale)
        {
            ReadyTextBlock.FontSize = 48 * xScale;
            StartBar.Height = 15 * yScale;
            TwoButtonsImage.Width = 128 * xScale;
            FourButtonsImage.Width = 64 * xScale;
            PointsTextBlock.FontSize = 24 * yScale;
            PointsTextTranslate.Y = -2 * yScale;
            PenaltyTextBlock.FontSize = 24 * yScale;
            PenaltyTextTranslate.Y = -2 * yScale;
            AllMustAnswerTextBlock.FontSize = 28 * yScale;
            TimerTextBlock.FontSize = 24 * yScale;
            LeftClockImage.Width = 20 * xScale;
            RightClockImage.Width = 20 * xScale;
        }
    }
}
