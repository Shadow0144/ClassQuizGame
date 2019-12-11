using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for ShowVictorControl.xaml
    /// </summary>
    public partial class ShowVictorControl : UserControl
    {
        public Boolean IsComplete;
        private Boolean readyToComplete;

        public Boolean[] previouslyVisible;

        private ParticleGenerator particleGenerator;

        private DispatcherTimer initialTimer;
        private const int INITIAL_WAIT = 3;

        private Boolean playersReady;

        private MediaPlayer victorySound;

        private Random shineRandom;
        private int[] shineTimers;
        private const int MIN_SHINE_TIME = 1000;
        private const int MAX_SHINE_TIME = 4000;
        private const int SHINE_DURATION = 10000;
        private long lastNow;

        private Storyboard[] shineStoryboards;
        private DoubleAnimation[] shineAnimations;

        public ShowVictorControl()
        {
            InitializeComponent();

            readyToComplete = false;
            IsComplete = false;
            NameScope.SetNameScope(this, new NameScope());
            for (int i = 0; i < 5; i++)
            {
                RegisterName(getShineTransformName(i), getShineTransform(i));
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            previouslyVisible = new Boolean[5];

            shineRandom = new Random();
            shineTimers = new int[5];
            shineAnimations = new DoubleAnimation[5];
            shineStoryboards = new Storyboard[5];

            int winners = 0;
            int topScore = int.MinValue;
            for (int index = 0; index < 5; index++)
            {
                PlayerControl playerControl = getController(index);
                if (playerControl.inGame && playerControl.score > topScore)
                {
                    topScore = playerControl.score;
                }
                else { }
            }

            for (int i = 0; i < 5; i++)
            {
                PlayerControl control = getController(i);
                if (control.score == topScore)
                {
                    winners++;
                    control.showCrown();
                    getGrid(i).Width = 180 * MainWindow.getInstance().getXScale();
                }
                else { }
                shineTimers[i] = (shineRandom.Next() % MAX_SHINE_TIME) + MIN_SHINE_TIME;
                shineAnimations[i] = new DoubleAnimation();
                shineAnimations[i].Duration = new Duration(new TimeSpan(0, 0, 0, SHINE_DURATION));
                shineAnimations[i].From = -100.0f;
                shineAnimations[i].To = 100.0f;
                shineStoryboards[i] = new Storyboard();
                Storyboard.SetTarget(shineAnimations[i], getShineTransform(i));
                Storyboard.SetTargetProperty(shineAnimations[i], new PropertyPath(TranslateTransform.XProperty));
                shineStoryboards[i].Children.Add(shineAnimations[i]);
            }

            for (int i = 0; i < 5; i++)
            {
                PlayerControl control = getController(i);
                if (control.inGame && control.score == topScore)
                {
                    Point controlPoint = MainWindow.getInstance().TranslatePoint(new Point(0.0f, 0.0f), control);
                    Point borderPoint = MainWindow.getInstance().TranslatePoint(new Point(0.0f, 0.0f), getGrid(i));
                    Point point = new Point(-borderPoint.X + controlPoint.X, -borderPoint.Y + controlPoint.Y);
                    displayWinner(i);
                }
                else { }
            }

            if (winners == 1)
            {
                WinnerTextBlock.Text = "Winner:";
            }
            else
            {
                WinnerTextBlock.Text = "Winners:";
            }

            particleGenerator = new ParticleGenerator(MainWindow.getInstance().ParticleCanvas);
            particleGenerator.active = true;

            playersReady = false;

            victorySound = new MediaPlayer();
            victorySound.ScrubbingEnabled = false;
            if (!Settings.Mute)
            {
                victorySound.Open(new Uri(@"assets/victory.mp3", UriKind.Relative));
                victorySound.Play();
            }
            else { }

            initialTimer = new DispatcherTimer();
            initialTimer.Interval = new TimeSpan(0, 0, INITIAL_WAIT);
            initialTimer.Tick += InitialTimer_Tick;
            initialTimer.Start();

            lastNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            setScale(MainWindow.getInstance().getXScale(), MainWindow.getInstance().getYScale());

            App.getInstance().xKeyboard.OnSpacePressed += () => finish();
        }

        private void InitialTimer_Tick(object sender, EventArgs e)
        {
            PressToCloseTextBlock.Visibility = Visibility.Visible;
            readyToComplete = true;
            initialTimer.Stop();
        }
        private void displayWinner(int i)
        {
            switch (i)
            {
                case 0:
                    TeamKWinGrid.Visibility = Visibility.Visible;
                    TeamKName.Text = getController(i).NameTextBox.Text;
                    break;
                case 1:
                    Team1WinGrid.Visibility = Visibility.Visible;
                    Team1Name.Text = getController(i).NameTextBox.Text;
                    break;
                case 2:
                    Team2WinGrid.Visibility = Visibility.Visible;
                    Team2Name.Text = getController(i).NameTextBox.Text;
                    break;
                case 3:
                    Team3WinGrid.Visibility = Visibility.Visible;
                    Team3Name.Text = getController(i).NameTextBox.Text;
                    break;
                case 4:
                    Team4WinGrid.Visibility = Visibility.Visible;
                    Team4Name.Text = getController(i).NameTextBox.Text;
                    break;
            }
        }

        private PlayerControl getController(int i)
        {
            return MainWindow.getInstance().GetPlayerControl(i);
        }

        private Grid getGrid(int i)
        {
            Grid r = null;
            switch (i)
            {
                case 0:
                    r = TeamKWinGrid;
                    break;
                case 1:
                    r = Team1WinGrid;
                    break;
                case 2:
                    r = Team2WinGrid;
                    break;
                case 3:
                    r = Team3WinGrid;
                    break;
                case 4:
                    r = Team4WinGrid;
                    break;
            }
            return r;
        }

        private TranslateTransform getShineTransform(int i)
        {
            TranslateTransform r = null;
            switch (i)
            {
                case 0:
                    r = TeamKShineTX;
                    break;
                case 1:
                    r = Team1ShineTX;
                    break;
                case 2:
                    r = Team2ShineTX;
                    break;
                case 3:
                    r = Team3ShineTX;
                    break;
                case 4:
                    r = Team4ShineTX;
                    break;
            }
            return r;
        }

        private String getShineTransformName(int i)
        {
            String r = null;
            switch (i)
            {
                case 0:
                    r = "TeamKShineTX";
                    break;
                case 1:
                    r = "Team1ShineTX";
                    break;
                case 2:
                    r = "Team2ShineTX";
                    break;
                case 3:
                    r = "Team3ShineTX";
                    break;
                case 4:
                    r = "Team4ShineTX";
                    break;
            }
            return r;
        }

        public void update()
        {
            particleGenerator.update();

            long deltaTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastNow;
            for (int i = 0; i < 5; i++)
            {
                shineTimers[i] -= (int)deltaTime;
                if (shineTimers[i] <= 0)
                {
                    shineStoryboards[i].Begin();
                    shineTimers[i] = (shineRandom.Next() % MAX_SHINE_TIME) + MIN_SHINE_TIME;
                }
                else { }
            }
            lastNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();

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
            if (readyToComplete)
            {
                if (!Settings.RequirePlayers || playersReady)
                {
                    MainWindow.getInstance().closeQuiz();
                    App.getInstance().xKeyboard.OnSpacePressed = null;
                    IsComplete = true;
                }
                else { }
            }
            else { }
        }

        public void close()
        {
            for (int i = 0; i < 5; i++)
            {
                PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(i);
                playerControl.resetFromWin();
                playerControl.resetScore();
            }
            MainWindow.getInstance().ParticleCanvas.Children.Clear();
        }

        public void setScale(float xScale, float yScale)
        {
            WinnerTextBlock.FontSize = 48 * yScale;
            Team1Name.FontSize = 32 * yScale;
            Team2Name.FontSize = 32 * yScale;
            Team3Name.FontSize = 32 * yScale;
            Team4Name.FontSize = 32 * yScale;
            particleGenerator.rescale();
            PressToCloseTextBlock.FontSize = 12 * yScale;
            PressToCloseTextBlock.Margin = new Thickness(12 * yScale);
        }
    }
}
