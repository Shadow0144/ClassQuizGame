using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow instance;

        private float xScale;
        private float yScale;

        public MainWindow()
        {
            InitializeComponent();

            instance = this;

            xScale = 1.0f;
            yScale = 1.0f;

            QuestionTimerTextBox.setEnableDecimal(false);
            CorrectPointsTextBox.setEnableDecimal(false);
            WrongPointsTextBox.setEnableDecimal(false);
        }

        public static MainWindow getInstance()
        {
            return instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the settings
            FullScreenMenuItem.IsChecked = Settings.FullScreen;
            MuteMenuItem.IsChecked = Settings.Mute;
            ShuffleQuestionsMenuItem.IsChecked = Settings.ShuffleQuestions;
            ShuffleAnswersMenuItem.IsChecked = Settings.ShuffleAnswers;
            WrongAnswersDeductMenuItem.IsChecked = Settings.WrongDeduct;
            WaitForAllPlayersMenuItem.IsChecked = Settings.WaitForAllPlayers;
            GradesAnswersImmediatelyMenuItem.IsChecked = Settings.GradeAnswersImmediately;
            ShowPlayerAnswersMenuItem.IsChecked = Settings.ShowAnswers;
            QuestionsTimedMenuItem.IsChecked = Settings.QuestionsTimed;
            RumbleMenuItem.IsChecked = Settings.Rumble;
            RequireProctorMenuItem.IsChecked = Settings.RequireProctor;
            RequirePlayersMenuItem.IsChecked = Settings.RequirePlayers;

            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    GetPlayerControl(i).setPlayer(i, App.getInstance().xKeyboard);
                }
                else
                {
                    GetPlayerControl(i).setPlayer(i, App.getInstance().controllers[i-1]);
                }
                setupPlayer(i);
            }

            toggleWindowFullScreen();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void toggleWindowFullScreen()
        {
            if (Settings.FullScreen)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void setupPlayer(int index)
        {
            PlayerControl player = GetPlayerControl(index);
            if (player.connected && (player.controlSource != PlayerControl.ControlSource.keyboard))
            {
                addPlayer(index);
            }
            else
            {
                player.Visibility = Visibility.Collapsed;
                togglePlayerAdd(index, true);
            }
        }

        private void togglePlayerAdd(int index, Boolean add)
        {
            switch (index)
            {
                case 0:
                    AddPlayerKMenuItem.IsEnabled = add;
                    RemovePlayerKMenuItem.IsEnabled = !add;
                    break;
                case 1:
                    AddPlayer1MenuItem.IsEnabled = add;
                    RemovePlayer1MenuItem.IsEnabled = !add;
                    break;
                case 2:
                    AddPlayer2MenuItem.IsEnabled = add;
                    RemovePlayer2MenuItem.IsEnabled = !add;
                    break;
                case 3:
                    AddPlayer3MenuItem.IsEnabled = add;
                    RemovePlayer3MenuItem.IsEnabled = !add;
                    break;
                case 4:
                    AddPlayer4MenuItem.IsEnabled = add;
                    RemovePlayer4MenuItem.IsEnabled = !add;
                    break;
            }
        }

        public PlayerControl GetPlayerControl(int index)
        {
            PlayerControl returnControl = null;

            switch (index)
            {
                case 0:
                    returnControl = TeamKPlayerControl;
                    break;
                case 1:
                    returnControl = Team1PlayerControl;
                    break;
                case 2:
                    returnControl = Team2PlayerControl;
                    break;
                case 3:
                    returnControl = Team3PlayerControl;
                    break;
                case 4:
                    returnControl = Team4PlayerControl;
                    break;
            }

            return returnControl;
        }

        public void closeQuiz()
        {
            QuizControl.close();
            CloseQuizMenuItem.IsEnabled = false;
            JumpToFinalQuestionMenuItem.IsEnabled = false;
        }

        private void Open_Quiz_Clicked(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            String path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            dialog.InitialDirectory = path + "\\quizzes\\";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                if (QuizControl.loadQuiz(dialog.FileName))
                {
                    CloseQuizMenuItem.IsEnabled = true;
                    JumpToFinalQuestionMenuItem.IsEnabled = true;
                }
                else { }
            }
            else { }
        }

        private void Jump_Clicked(object sender, EventArgs e)
        {
            JumpToFinalQuestionPopup.IsOpen = true;
        }

        private void Jump_Popup_Ok_Clicked(object sender, EventArgs e)
        {
            JumpToFinalQuestionPopup.IsOpen = false;
            QuizControl.jumpToFinalQuestion();
        }

        private void Jump_Popup_Cancel_Clicked(object sender, EventArgs e)
        {
            JumpToFinalQuestionPopup.IsOpen = false;
        }

        private void Close_Quiz_Clicked(object sender, EventArgs e)
        {
            CloseQuizPopup.IsOpen = true;
        }

        private void Close_Quiz_Popup_Ok_Clicked(object sender, EventArgs e)
        {
            CloseQuizPopup.IsOpen = false;
            closeQuiz();
        }

        private void Close_Quiz_Popup_Cancel_Clicked(object sender, EventArgs e)
        {
            CloseQuizPopup.IsOpen = false;
        }

        private void Quit_Clicked(object sender, EventArgs e)
        {
            QuitPopup.IsOpen = true;
        }

        private void Quit_Popup_Ok_Clicked(object sender, EventArgs e)
        {
            QuitPopup.IsOpen = false;
            Close();
        }

        private void Quit_Popup_Cancel_Clicked(object sender, EventArgs e)
        {
            QuitPopup.IsOpen = false;
        }

        private void addPlayer(int index)
        {
            GetPlayerControl(index).Visibility = Visibility.Visible;
            GetPlayerControl(index).animateIn();
            GetPlayerControl(index).inGame = true;
            GetPlayerControl(index).checkConnected();
            App.getInstance().players++;
            togglePlayerAdd(index, false);
        }

        private void removePlayer(int index)
        {
            GetPlayerControl(index).animateOut();
            GetPlayerControl(index).inGame = false;
            App.getInstance().players--;
            togglePlayerAdd(index, true);
        }

        private void Add_Player_Keyboard_Clicked(object sender, EventArgs e)
        {
            addPlayer(0);
        }

        private void Remove_Player_Keyboard_Clicked(object sender, EventArgs e)
        {
            removePlayer(0);
        }

        private void Add_Player_1_Clicked(object sender, EventArgs e)
        {
            addPlayer(1);
        }

        private void Remove_Player_1_Clicked(object sender, EventArgs e)
        {
            removePlayer(1);
        }

        private void Add_Player_2_Clicked(object sender, EventArgs e)
        {
            addPlayer(2);
        }

        private void Remove_Player_2_Clicked(object sender, EventArgs e)
        {
            removePlayer(2);
        }

        private void Add_Player_3_Clicked(object sender, EventArgs e)
        {
            addPlayer(3);
        }

        private void Remove_Player_3_Clicked(object sender, EventArgs e)
        {
            removePlayer(3);
        }

        private void Add_Player_4_Clicked(object sender, EventArgs e)
        {
            addPlayer(4);
        }

        private void Remove_Player_4_Clicked(object sender, EventArgs e)
        {
            removePlayer(4);
        }

        private void Full_Screen_Checked(object sender, EventArgs e)
        {
            Settings.FullScreen = true;
            toggleWindowFullScreen();
        }

        private void Full_Screen_Unchecked(object sender, EventArgs e)
        {
            Settings.FullScreen = false;
            toggleWindowFullScreen();
        }

        private void Mute_Checked(object sender, EventArgs e)
        {
            Settings.Mute = true;
        }

        private void Mute_Unchecked(object sender, EventArgs e)
        {
            Settings.Mute = false;
        }

        private void Shuffle_Questions_Checked(object sender, EventArgs e)
        {
            Settings.ShuffleQuestions = true;
        }

        private void Shuffle_Questions_Unchecked(object sender, EventArgs e)
        {
            Settings.ShuffleQuestions = false;
        }

        private void Shuffle_Answers_Checked(object sender, EventArgs e)
        {
            Settings.ShuffleAnswers = true;
        }

        private void Shuffle_Answers_Unchecked(object sender, EventArgs e)
        {
            Settings.ShuffleAnswers = false;
        }

        private void Correct_Answer_Points_Clicked(object sender, EventArgs e)
        {
            CorrectAnswerPointsPopup.IsOpen = true;
            CorrectPointsTextBox.Text = "" + Settings.RightPoints;
        }

        private void Correct_Answer_Popup_Ok_Clicked(object sender, EventArgs e)
        {
            Settings.RightPoints = int.Parse(CorrectPointsTextBox.Text);
            CorrectAnswerPointsPopup.IsOpen = false;
        }

        private void Correct_Answer_Popup_Cancel_Clicked(object sender, EventArgs e)
        {
            CorrectAnswerPointsPopup.IsOpen = false;
        }

        private void Wrong_Answers_Deduct_Checked(object sender, EventArgs e)
        {
            Settings.WrongDeduct = true;
            WrongAnswerPointsMenuItem.IsEnabled = true;
        }

        private void Wrong_Answers_Deduct_Unchecked(object sender, EventArgs e)
        {
            Settings.WrongDeduct = false;
            WrongAnswerPointsMenuItem.IsEnabled = false;
        }

        private void Wrong_Answer_Points_Clicked(object sender, EventArgs e)
        {
            WrongAnswerPointsPopup.IsOpen = true;
            WrongPointsTextBox.Text = "" + Settings.WrongPoints;
        }

        private void Wrong_Answer_Popup_Ok_Clicked(object sender, EventArgs e)
        {
            Settings.WrongPoints = int.Parse(WrongPointsTextBox.Text);
            WrongAnswerPointsPopup.IsOpen = false;
        }

        private void Wrong_Answer_Popup_Cancel_Clicked(object sender, EventArgs e)
        {
            WrongAnswerPointsPopup.IsOpen = false;
        }

        private void Always_Show_Points_Checked(object sender, EventArgs e)
        {
            Settings.AlwaysShowPoints = true;
        }

        private void Always_Show_Points_Unchecked(object sender, EventArgs e)
        {
            Settings.AlwaysShowPoints = false;
        }

        private void Always_Show_Penalties_Checked(object sender, EventArgs e)
        {
            Settings.AlwaysShowPenalties = true;
        }

        private void Always_Show_Penalties_Unchecked(object sender, EventArgs e)
        {
            Settings.AlwaysShowPenalties = false;
        }

        private void Wait_For_All_Players_Checked(object sender, EventArgs e)
        {
            Settings.WaitForAllPlayers = true;
        }

        private void Wait_For_All_Players_Unchecked(object sender, EventArgs e)
        {
            Settings.WaitForAllPlayers = false;
        }

        private void Grade_Answers_Immediately_Checked(object sender, EventArgs e)
        {
            Settings.GradeAnswersImmediately = true;
        }

        private void Grade_Answers_Immediately_Unchecked(object sender, EventArgs e)
        {
            Settings.GradeAnswersImmediately = false;
        }

        private void Show_Player_Answers_Checked(object sender, EventArgs e)
        {
            Settings.ShowAnswers = true;
        }

        private void Show_Player_Answers_Unchecked(object sender, EventArgs e)
        {
            Settings.ShowAnswers = false;
        }

        private void Questions_Timed_Checked(object sender, EventArgs e)
        {
            Settings.QuestionsTimed = true;
        }

        private void Questions_Timed_Unchecked(object sender, EventArgs e)
        {
            Settings.QuestionsTimed = false;
        }

        private void Set_Question_Timer_Clicked(object sender, EventArgs e)
        {
            QuestionTimerTextBox.Text = ""+Settings.QuestionTime;
            QuestionTimerPopup.IsOpen = true;
        }

        private void Set_Question_Timer_Ok_Clicked(object sender, EventArgs e)
        {
            try
            {
                Settings.QuestionTime = int.Parse(QuestionTimerTextBox.Text);
            }
            catch (Exception)
            {
                System.Console.WriteLine("Failed to parse as number: " + QuestionTimerTextBox.Text);
            }
            QuestionTimerPopup.IsOpen = false;
        }

        private void Set_Question_Timer_Cancel_Clicked(object sender, EventArgs e)
        {
            QuestionTimerPopup.IsOpen = false;
        }

        private void Rumble_Checked(object sender, EventArgs e)
        {
            Settings.Rumble = true;
        }

        private void Rumble_Unchecked(object sender, EventArgs e)
        {
            Settings.Rumble = false;
        }

        private void Require_Proctor_Checked(object sender, EventArgs e)
        {
            Settings.RequireProctor = true;
        }

        private void Require_Proctor_Unchecked(object sender, EventArgs e)
        {
            Settings.RequireProctor = false;
        }

        private void Require_Players_Checked(object sender, EventArgs e)
        {
            Settings.RequirePlayers = true;
        }

        private void Require_Players_Unchecked(object sender, EventArgs e)
        {
            Settings.RequirePlayers = false;
        }

        private void Controls_Clicked(object sender, EventArgs e)
        {
            ControlsPopup.IsOpen = true;
        }

        private void Controls_Ok_Clicked(object sender, EventArgs e)
        {
            ControlsPopup.IsOpen = false;
        }

        private void About_Clicked(object sender, EventArgs e)
        {
            AboutPopup.IsOpen = true;
        }

        private void About_Ok_Clicked(object sender, EventArgs e)
        {
            AboutPopup.IsOpen = false;
        }

        public void PopupFailedToLoadQuiz()
        {
            FailedReasonTextBlock.Text = QuestionParser.ErrorMessage;
            FailedToLoadQuizPopup.IsOpen = true;
        }

        private void Failed_Ok_Clicked(object sender, EventArgs e)
        {
            FailedToLoadQuizPopup.IsOpen = false;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            App.getInstance().xKeyboard.Update();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            App.getInstance().xKeyboard.Update();
        }

        public void update()
        {

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            xScale = ((float)e.NewSize.Width) / 800.0f;
            yScale = ((float)e.NewSize.Height) / 650.0f;

            JumpToFinalQuestionTextBlock.FontSize = 12 * yScale;
            JumpToFinalQuestionOkButton.FontSize = 12 * yScale;
            JumpToFinalQuestionCancelButton.FontSize = 12 * yScale;

            CloseQuizTextBlock.FontSize = 12 * yScale;
            CloseQuizOkButton.FontSize = 12 * yScale;
            CloseQuizCancelButton.FontSize = 12 * yScale;

            QuitTextBlock.FontSize = 12 * yScale;
            QuitOkButton.FontSize = 12 * yScale;
            QuitCancelButton.FontSize = 12 * yScale;

            CorrectAnswerPointsTextBlock.FontSize = 12 * yScale;
            CorrectPointsTextBox.FontSize = 12 * yScale;
            CorrectPointsOkButton.FontSize = 12 * yScale;
            CorrectPointsCancelButton.FontSize = 12 * yScale;

            WrongPointsTextBlock.FontSize = 12 * yScale;
            WrongPointsTextBox.FontSize = 12 * yScale;
            WrongPointsOkButton.FontSize = 12 * yScale;
            WrongPointsCancelButton.FontSize = 12 * yScale;

            QuestionTimerTextBlock.FontSize = 12 * yScale;
            QuestionTimerTextBox.FontSize = 12 * yScale;
            QuestionTimerOkButton.FontSize = 12 * yScale;
            QuestionTimerCancelButton.FontSize = 12 * yScale;

            ControlsTextBlock.FontSize = 12 * yScale;
            ControlsExplainTextBlock.FontSize = 12 * yScale;
            ControlsOkButton.FontSize = 12 * yScale;

            AboutTextBlock.FontSize = 12 * yScale;
            AboutOkButton.FontSize = 12 * yScale;

            FailedTextBlock.FontSize = 16 * yScale;
            FailedReasonTextBlock.FontSize = 12 * yScale;
            FailedOkButton.FontSize = 12 * yScale;

            ParticleGenerator.setScale(xScale, yScale);
            QuizControl.setScale(xScale, yScale);
            TeamKPlayerControl.resize(xScale, yScale);
            Team1PlayerControl.resize(xScale, yScale);
            Team2PlayerControl.resize(xScale, yScale);
            Team3PlayerControl.resize(xScale, yScale);
            Team4PlayerControl.resize(xScale, yScale);

            InvalidateArrange();
        }

        public float getXScale()
        {
            return xScale;
        }

        public float getYScale()
        {
            return yScale;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(QuizControl);
        }

        public void LoseKeyboardFocus()
        {
            App.getInstance().xKeyboard.Enabled = false;
        }

        public void GainKeyboardFocus()
        {
            App.getInstance().xKeyboard.Enabled = true;
        }
    }
}
