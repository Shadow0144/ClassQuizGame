using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for Quiz.xaml
    /// </summary>
    public partial class Quiz : UserControl
    {
        private DispatcherTimer timer;
        private const int DELTA_T = 10;

        public String gameFile;
        public int questionCount;
        public Question[] questions;
        public int winner;

        public int currentQuestion;

        public String gameTitle;

        public Boolean[] readyPlayers;

        private TitleSplashControl titleSplashControl;
        private PromptForReadyControl promptForReadyControl;
        private ShowQuestionControl showQuestionControl;
        private ShowAnswerControl showAnswerControl;
        private ShowVictorControl showVictorControl;

        public enum GameState
        {
            waiting_for_quiz,
            splashing_title,
            prompting_for_ready,
            showing_question,
            showing_answer,
            victoring_screen
        };
        public GameState gameState;

        public Quiz()
        {
            InitializeComponent();

            this.gameState = GameState.waiting_for_quiz;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, DELTA_T);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            readyPlayers = new Boolean[5];
            readyPlayers[0] = false;
            readyPlayers[1] = false;
            readyPlayers[2] = false;
            readyPlayers[3] = false;
            readyPlayers[4] = false;
        }

        public Boolean loadQuiz(String gameFile)
        {
            this.gameFile = gameFile;

            close();

            QuestionParser parser = new QuestionParser(gameFile);
            Boolean succeeded = parser.loadQuiz();

            if (succeeded)
            {
                gameTitle = parser.gameTitle;
                questions = parser.questions;
                questionCount = questions.Length;

                currentQuestion = 0;
                titleSplashControl = new TitleSplashControl(gameTitle);
                QuizBorder.Child = titleSplashControl;
                gameState = GameState.splashing_title;
            }
            else
            {
                MainWindow.getInstance().PopupFailedToLoadQuiz();
            }

            return succeeded;
        }

        public void close()
        {
            if (showVictorControl != null)
            {
                showVictorControl.close();
            }
            else { }
            QuizBorder.Child = null;
            unreadyPlayers();
            resetPlayers();
        }

        public void update()
        {
            switch (gameState)
            {
                case GameState.waiting_for_quiz:
                    // Do nothing
                    break;
                case GameState.splashing_title:
                    updateSplashingTitle();
                    break;
                case GameState.prompting_for_ready:
                    updatePromptingForReady();
                    break;
                case GameState.showing_question:
                    updateShowingQuestion();
                    break;
                case GameState.showing_answer:
                    updateShowingAnswer();
                    break;
                case GameState.victoring_screen:
                    updateVictoryingScreen();
                    break;
            }
        }

        // Wait for keyboard input or maybe all players ready to move to the first question
        public void updateSplashingTitle()
        {
            updatePlayers();
            titleSplashControl.update();
            if (titleSplashControl.IsComplete)
            {
                unreadyPlayers();
                gameState = GameState.prompting_for_ready;
                promptForReadyControl = new PromptForReadyControl(questions[currentQuestion]);
                QuizBorder.Child = promptForReadyControl;
            }
            else { }
        }

        // Check that all the players are ready and countdown
        public void updatePromptingForReady()
        {
            updatePlayers();
            promptForReadyControl.update();
            if (promptForReadyControl.IsComplete)
            {
                unreadyPlayers();
                gameState = GameState.showing_question;
                showQuestionControl = new ShowQuestionControl(questions[currentQuestion]);
                QuizBorder.Child = showQuestionControl;
            }
            else { }
        }

        // Check for answers
        public void updateShowingQuestion()
        {
            updatePlayers();
            showQuestionControl.update();
            if (showQuestionControl.IsComplete)
            {
                unreadyPlayers();
                gameState = GameState.showing_answer;
                showAnswerControl = new ShowAnswerControl(questions[currentQuestion]);
                QuizBorder.Child = showAnswerControl;
            }
            else { }
        }

        // Wait for keyboard input
        public void updateShowingAnswer()
        {
            updatePlayers();
            showAnswerControl.update();
            if (showAnswerControl.IsComplete)
            {
                unreadyPlayers();
                currentQuestion++;
                if (currentQuestion < questionCount)
                {
                    gameState = GameState.prompting_for_ready;
                    promptForReadyControl = new PromptForReadyControl(questions[currentQuestion]);
                    QuizBorder.Child = promptForReadyControl;
                }
                else
                {
                    gameState = GameState.victoring_screen;
                    showVictorControl = new ShowVictorControl();
                    QuizBorder.Child = showVictorControl;
                }
            }
            else { }
        }

        public void updateVictoryingScreen()
        {
            updatePlayers();
            showVictorControl.update();
            if (showVictorControl.IsComplete)
            {
                unreadyPlayers();
                gameState = GameState.waiting_for_quiz;
                currentQuestion = 0;
                QuizBorder.Child = null;
            }
            else { }
        }

        private void updatePlayers()
        {
            if (gameState != GameState.showing_question && Settings.RequirePlayers)
            {
                for (int i = 0; i < 5; i++)
                {
                    PlayerControl player = MainWindow.getInstance().GetPlayerControl(i);
                    if (player.IsStartDown() && !player.readied)
                    {
                        player.ready();
                    }
                    else { }
                }
            }
            else { }
        }

        private void unreadyPlayers()
        {
            for (int i = 0; i < 5; i++)
            {
                MainWindow.getInstance().GetPlayerControl(i).unready();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (App.getInstance() != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    XInputController controller = App.getInstance().controllers[i];
                    PlayerControl player = MainWindow.getInstance().GetPlayerControl(i+1);
                    controller.Update();
                    if (player.inGame)
                    {
                        if (controller.connected && !player.connected)
                        {
                            player.animateReconnected();
                        }
                        else if (!controller.connected && player.connected)
                        {
                            player.animateDisconnected();
                        }
                        else { }
                    }
                    else { }
                }

                MainWindow.getInstance().update();

                update();
            }
            else { }
        }

        private void resetPlayers()
        {
            for (int i = 0; i < 5; i++)
            {
                PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(i);
                playerControl.resetFromWin();
                playerControl.resetScore();
            }
        }

        public void jumpToFinalQuestion()
        {
            currentQuestion = questionCount - 1;
            unreadyPlayers();
            gameState = GameState.prompting_for_ready;
            promptForReadyControl = new PromptForReadyControl(questions[currentQuestion]);
            QuizBorder.Child = promptForReadyControl;
        }

        public void setScale(float xScale, float yScale)
        {
            switch (gameState)
            {
                case GameState.waiting_for_quiz:
                    // Do nothing
                    break;
                case GameState.splashing_title:
                    titleSplashControl.setScale(xScale, yScale);
                    break;
                case GameState.prompting_for_ready:
                    promptForReadyControl.setScale(xScale, yScale);
                    break;
                case GameState.showing_question:
                    showQuestionControl.setScale(xScale, yScale);
                    break;
                case GameState.showing_answer:
                    showAnswerControl.setScale(xScale, yScale);
                    break;
                case GameState.victoring_screen:
                    showVictorControl.setScale(xScale, yScale);
                    break;
            }
        }
    }
}
