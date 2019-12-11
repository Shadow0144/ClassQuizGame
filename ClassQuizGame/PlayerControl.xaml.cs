using SharpDX.XInput;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        private const float WIDTH = 200.0f;
        private const float HEIGHT = 136.0f;
        public static float ControlWidth = WIDTH;

        private static Color playerKColor = Color.FromArgb(255, 255, 255, 255);
        private static Color player1Color = Color.FromArgb(255, 255, 0, 0);
        private static Color player2Color = Color.FromArgb(255, 0, 0, 255);
        private static Color player3Color = Color.FromArgb(255, 0, 255, 0);
        private static Color player4Color = Color.FromArgb(255, 255, 255, 0);

        private static Color disPlayerKColor = Color.FromArgb(255, 125, 125, 125);
        private static Color disPlayer1Color = Color.FromArgb(255, 125, 0, 0);
        private static Color disPlayer2Color = Color.FromArgb(255, 0, 0, 125);
        private static Color disPlayer3Color = Color.FromArgb(255, 0, 125, 0);
        private static Color disPlayer4Color = Color.FromArgb(255, 125, 125, 0);

        private static ImageSource noImageSource = null;
        private static ImageSource wrongImageSource = new BitmapImage(new Uri(@"assets/RedXMark.png", UriKind.Relative));
        private static ImageSource answeredImageSource = new BitmapImage(new Uri(@"assets/YellowCircleMark.png", UriKind.Relative));
        private static ImageSource correctImageSource = new BitmapImage(new Uri(@"assets/GreenCheckMark.png", UriKind.Relative));

        private static ImageSource silverCrownImageSource = new BitmapImage(new Uri(@"assets/CrownGrey.png", UriKind.Relative));
        private static ImageSource goldCrownImageSource = new BitmapImage(new Uri(@"assets/CrownGold.png", UriKind.Relative));

        private int player;
        public String name;
        public int score;
        public Boolean canAnswer;

        public Boolean inGame;
        public Boolean connected;

        private MediaPlayer readiedSound;
        public Boolean readied;

        public Boolean showAnswers;

        private RumbleHelper rumbleHelper;

        public enum ControlSource
        {
            keyboard,
            controller
        }
        public ControlSource controlSource;

        public XInputController controller;
        public XKeyboard keyboard;

        public AnswerBox.Answer lastAnswer;

        public enum AnswerResult
        {
            no_answer,
            wrong,
            correct
        }
        private AnswerResult lastAnswerResult;

        public PlayerControl()
        {
            InitializeComponent();

            InControlWidth.To = WIDTH;
            OutControlWidth.From = WIDTH;
            showAnswers = true;
        }
        
        public void checkConnected()
        {
            if (!connected)
            {
                animateDisconnected();
            }
            else { }
        }

        public void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            name = NameTextBox.Text;
        }

        public void ScoreTextBox_TextChanged(object sender, EventArgs e)
        {
            score = Int32.Parse(ScoreTextBox.Text);
        }

        public void setPlayer(int i, XKeyboard keyboard)
        {
            inGame = false;
            readied = false;
            this.keyboard = keyboard;
            controlSource = ControlSource.keyboard;
            connected = true;
            name = NameTextBox.Text;
            score = 0;
            canAnswer = false;
            lastAnswerResult = AnswerResult.no_answer;
            player = i;
            setColor();
            readiedSound = new MediaPlayer();
        }

        public void setPlayer(int i, XInputController controller)
        {
            inGame = false;
            readied = false;
            this.controller = controller;
            controlSource = ControlSource.controller;
            connected = controller.connected;
            name = NameTextBox.Text;
            score = 0;
            canAnswer = false;
            lastAnswerResult = AnswerResult.no_answer;
            player = i;
            setColor();
            readiedSound = new MediaPlayer();
        }

        private void setColor()
        {
            switch (player)
            {
                case 0:
                    PlayerBrushStopStart.Color = playerKColor;
                    break;
                case 1:
                    PlayerBrushStopStart.Color = player1Color;
                    break;
                case 2:
                    PlayerBrushStopStart.Color = player2Color;
                    break;
                case 3:
                    PlayerBrushStopStart.Color = player3Color;
                    break;
                case 4:
                    PlayerBrushStopStart.Color = player4Color;
                    break;
            }
        }

        public void setAnswered(AnswerBox.Answer answer, AnswerResult answerType)
        {
            lastAnswer = answer;
            lastAnswerResult = answerType;

            switch (answerType)
            {
                case AnswerResult.no_answer:
                    AnsweredImage.Source = noImageSource;
                    break;
                case AnswerResult.wrong:
                    if (showAnswers)
                    {
                        AnsweredImage.Source = wrongImageSource;
                    }
                    else
                    {
                        AnsweredImage.Source = answeredImageSource;
                    }
                    break;
                case AnswerResult.correct:
                    if (showAnswers)
                    {
                        AnsweredImage.Source = correctImageSource;
                    }
                    else
                    {
                        AnsweredImage.Source = answeredImageSource;
                    }
                    break;
            }
        }

        public AnswerResult getLastAnswerResult()
        {
            return lastAnswerResult;
        }

        public void revealAnswer()
        {
            switch (lastAnswerResult)
            {
                case AnswerResult.no_answer:
                    AnsweredImage.Source = noImageSource;
                    break;
                case AnswerResult.wrong:
                    AnsweredImage.Source = wrongImageSource;
                    break;
                case AnswerResult.correct:
                    AnsweredImage.Source = correctImageSource;
                    break;
            }
        }

        public void setEnabled(Boolean enabled)
        {
            if (enabled)
            {
                switch (player)
                {
                    case 0:
                        PlayerBrushStopStart.Color = playerKColor;
                        break;
                    case 1:
                        PlayerBrushStopStart.Color = player1Color;
                        break;
                    case 2:
                        PlayerBrushStopStart.Color = player2Color;
                        break;
                    case 3:
                        PlayerBrushStopStart.Color = player3Color;
                        break;
                    case 4:
                        PlayerBrushStopStart.Color = player4Color;
                        break;
                }
            }
            else
            {
                switch (player)
                {
                    case 0:
                        PlayerBrushStopStart.Color = disPlayerKColor;
                        break;
                    case 1:
                        PlayerBrushStopStart.Color = disPlayer1Color;
                        break;
                    case 2:
                        PlayerBrushStopStart.Color = disPlayer2Color;
                        break;
                    case 3:
                        PlayerBrushStopStart.Color = disPlayer3Color;
                        break;
                    case 4:
                        PlayerBrushStopStart.Color = disPlayer4Color;
                        break;
                }
            }
        }

        public void addScore(int score, Boolean positive)
        {
            if (score > 0)
            {
                ScorePlusTextBlock.Text = "+" + score;
            }
            else
            {
                ScorePlusTextBlock.Text = "" + score;
            }
            if (positive)
            {
                ScorePlusTextBlock.Fill = Brushes.Green;
                ScorePlusTextBlock.Stroke = Brushes.White;
            }
            else
            {
                ScorePlusTextBlock.Fill = Brushes.Red;
                ScorePlusTextBlock.Stroke = Brushes.White;
            }
            this.score += score;
            ScoreTextBox.Text = ""+this.score;
        }

        public void resetScoreDisplay()
        {
            ScorePlusTextBlock.Text = "";
            setAnswered(AnswerBox.Answer.None, AnswerResult.no_answer);
            ScoreTextBox.Text = "" + this.score;
        }

        public void resetScore()
        {
            this.score = 0;
            resetScoreDisplay();
            hideCrown();
        }

        public void showCrown()
        {
            VictoryImage.Source = goldCrownImageSource;
        }

        public void showSilverCrown()
        {
            VictoryImage.Source = silverCrownImageSource;
        }

        public void hideCrown()
        {
            VictoryImage.Source = null;
        }

        public void ready()
        {
            ReadyImage.Visibility = Visibility.Visible;
            if (!Settings.Mute)
            {
                readiedSound.Open(new Uri(@"assets/ready.mp3", UriKind.Relative));
                readiedSound.Play();
            }
            else { }
            readied = true;
        }

        public void rightRumble()
        {
            if (Settings.Rumble && connected && controlSource == ControlSource.controller)
            {
                rumbleHelper = new RumbleHelper(controller, RumbleHelper.RumbleType.burstRumble);
                rumbleHelper.start();
            }
            else { }
        }

        public void wrongRumble()
        {
            if (Settings.Rumble && connected && controlSource == ControlSource.controller)
            {
                rumbleHelper = new RumbleHelper(controller, RumbleHelper.RumbleType.wobbleRumble);
                rumbleHelper.start();
            }
            else { }
        }

        public void answerRumble()
        {
            if (Settings.Rumble && connected && controlSource == ControlSource.controller)
            {
                rumbleHelper = new RumbleHelper(controller, RumbleHelper.RumbleType.longRumble);
                rumbleHelper.start();
            }
            else { }
        }

        public void unready()
        {
            setColor();
            ReadyImage.Visibility = Visibility.Hidden;
            readied = false;
        }

        public void animateIn()
        {
            AnimateInStoryboard.Begin();
        }

        public void animateOut()
        {
            unready();
            AnimateOutStoryboard.Begin();
        }

        public void animateDisconnected()
        {
            connected = false;
            unready();
            DisconnectedStoryboard.Begin();
        }

        public void animateReconnected()
        {
            connected = true;
            ConnectedStoryboard.Begin();
        }

        public void resetFromWin()
        {
            setColor();
        }

        public Boolean IsStartDown()
        {
            Boolean StartDown = false;
            switch (controlSource)
            {
                case ControlSource.keyboard:
                    StartDown = keyboard.EnterDown;
                    break;
                case ControlSource.controller:
                    StartDown = controller.StartDown;
                    break;
            }
            return StartDown;
        }

        public Boolean IsADown()
        {
            Boolean ADown = false;
            switch (controlSource)
            {
                case ControlSource.keyboard:
                    ADown = keyboard.ADown;
                    break;
                case ControlSource.controller:
                    ADown = controller.ADown;
                    break;
            }
            return ADown;
        }

        public Boolean IsBDown()
        {
            Boolean BDown = false;
            switch (controlSource)
            {
                case ControlSource.keyboard:
                    BDown = keyboard.BDown;
                    break;
                case ControlSource.controller:
                    BDown = controller.BDown;
                    break;
            }
            return BDown;
        }

        public Boolean IsXDown()
        {
            Boolean XDown = false;
            switch (controlSource)
            {
                case ControlSource.keyboard:
                    XDown = keyboard.XDown;
                    break;
                case ControlSource.controller:
                    XDown = controller.XDown;
                    break;
            }
            return XDown;
        }

        public Boolean IsYDown()
        {
            Boolean YDown = false;
            switch (controlSource)
            {
                case ControlSource.keyboard:
                    YDown = keyboard.YDown;
                    break;
                case ControlSource.controller:
                    YDown = controller.YDown;
                    break;
            }
            return YDown;
        }

        public Boolean IsLDown()
        {
            Boolean LDown = false;
            switch (controlSource)
            {
                case ControlSource.keyboard:
                    LDown = keyboard.LDown;
                    break;
                case ControlSource.controller:
                    LDown = controller.LDown;
                    break;
            }
            return LDown;
        }

        public Boolean IsRDown()
        {
            Boolean RDown = false;
            switch (controlSource)
            {
                case ControlSource.keyboard:
                    RDown = keyboard.RDown;
                    break;
                case ControlSource.controller:
                    RDown = controller.RDown;
                    break;
            }
            return RDown;
        }

        public void resize(float xScale, float yScale)
        {
            float scale = Math.Min(xScale, yScale);

            ControlWidth = WIDTH * scale;

            Width = WIDTH * scale;
            Height = HEIGHT * scale;

            Control.Width = WIDTH * scale;
            Control.Height = HEIGHT * scale;

            PlayerGrid.Width = WIDTH * scale;
            PlayerGrid.Height = HEIGHT * scale;

            DisconnectImage.Width = 100 * scale;
            DisconnectImage.Height = 100 * scale;

            AnsweredImage.Width = 42 * scale;
            AnsweredImage.Height = 42 * scale;

            ScorePlusTextBlock.Width = 160 * scale;
            ScorePlusTextBlock.FontSize = 32 * scale;
            ((TranslateTransform)ScorePlusTextBlock.RenderTransform).X = 6 * scale;
            ((TranslateTransform)ScorePlusTextBlock.RenderTransform).Y = -52 * scale;
            AnimatePointsTranslation.To = -50 * scale;

            VictoryImage.Width = 64 * scale;
            VictoryImage.Height = 64 * scale;
            ((TranslateTransform)VictoryImage.RenderTransform).Y = -70 * scale;

            ScoreTextBox.Width = 140 * scale;
            ScoreTextBox.FontSize = 42 * scale;
            ((TranslateTransform)ScoreTextBox.RenderTransform).Y = -32 * scale;

            NameTextBox.Width = 160 * scale;
            NameTextBox.FontSize = 24 * scale;
            ((TranslateTransform)NameTextBox.RenderTransform).Y = 34 * scale;

            InControlWidth.To = WIDTH * scale;
            OutControlWidth.From = WIDTH * scale;
        }

        private void OnScoreTextChanged(object sender, TextChangedEventArgs e)
        {
            int newScore = Int32.Parse(ScoreTextBox.Text);
            if (newScore != score)
            {
                score = newScore;
            }
            else { }
        }

        private void OnTeamNameTextChanged(object sender, TextChangedEventArgs e)
        {
            if (name == null || !name.Equals(NameTextBox.Text))
            {
                name = NameTextBox.Text;
            }
            else { }
        }

        private void OnGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            MainWindow.getInstance().LoseKeyboardFocus();
        }

        private void OnLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            MainWindow.getInstance().GainKeyboardFocus();
        }
    }
}
