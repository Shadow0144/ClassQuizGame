using System;

namespace ClassQuizGame
{
    public static class Settings
    {
        public static Boolean FullScreen { get => fullScreen; set { fullScreen = value; Properties.Settings.Default.fullScreen = fullScreen; Properties.Settings.Default.Save(); } }
        private static Boolean fullScreen = Properties.Settings.Default.fullScreen;
        public static Boolean Mute { get => mute; set { mute = value; Properties.Settings.Default.mute = mute; Properties.Settings.Default.Save(); } }
        private static Boolean mute = Properties.Settings.Default.mute;
        public static Boolean ShuffleQuestions { get => shuffleQuestions; set { shuffleQuestions = value; Properties.Settings.Default.shuffleQuestions = shuffleQuestions; Properties.Settings.Default.Save(); } }
        private static Boolean shuffleQuestions = Properties.Settings.Default.shuffleQuestions;
        public static Boolean ShuffleAnswers { get => shuffleAnswers; set { shuffleAnswers = value; Properties.Settings.Default.shuffleAnswers = shuffleAnswers; Properties.Settings.Default.Save(); } }
        private static Boolean shuffleAnswers = Properties.Settings.Default.shuffleAnswers;
        public static Boolean WrongDeduct { get => wrongDeduct; set { wrongDeduct = value; Properties.Settings.Default.wrongDeduct = wrongDeduct; Properties.Settings.Default.Save(); } }
        private static Boolean wrongDeduct = Properties.Settings.Default.wrongDeduct;
        public static int RightPoints { get => rightPoints; set { rightPoints = value; Properties.Settings.Default.rightPoints = rightPoints; Properties.Settings.Default.Save(); } }
        private static int rightPoints = Properties.Settings.Default.rightPoints;
        public static int WrongPoints { get => wrongPoints; set { wrongPoints = value; Properties.Settings.Default.wrongPoints = wrongPoints; Properties.Settings.Default.Save(); } }
        private static int wrongPoints = Properties.Settings.Default.wrongPoints;
        public static Boolean AlwaysShowPoints { get => alwaysShowPoints; set { alwaysShowPoints = value; Properties.Settings.Default.alwaysShowPoints = alwaysShowPoints; Properties.Settings.Default.Save(); } }
        private static Boolean alwaysShowPoints;
        public static Boolean AlwaysShowPenalties { get => alwaysShowPenalties; set { alwaysShowPenalties = value; Properties.Settings.Default.alwaysShowPenalties = alwaysShowPenalties; Properties.Settings.Default.Save(); } }
        private static Boolean alwaysShowPenalties;
        public static Boolean WaitForAllPlayers { get => waitForAllPlayers; set { waitForAllPlayers = value; Properties.Settings.Default.waitForAllPlayers = waitForAllPlayers; Properties.Settings.Default.Save(); } }
        private static Boolean waitForAllPlayers = Properties.Settings.Default.waitForAllPlayers;
        public static Boolean GradeAnswersImmediately { get => gradeAnswersImmediately; set { gradeAnswersImmediately = value; Properties.Settings.Default.gradeAnswersImmediately = gradeAnswersImmediately; Properties.Settings.Default.Save(); } }
        private static Boolean gradeAnswersImmediately = Properties.Settings.Default.gradeAnswersImmediately;
        public static Boolean ShowAnswers { get => showAnswers; set { showAnswers = value; Properties.Settings.Default.showAnswers = showAnswers; Properties.Settings.Default.Save(); } }
        private static Boolean showAnswers = Properties.Settings.Default.showAnswers;
        public static Boolean QuestionsTimed { get => questionsTimed; set { questionsTimed = value; Properties.Settings.Default.questionsTimed = questionsTimed; Properties.Settings.Default.Save(); } }
        private static Boolean questionsTimed = Properties.Settings.Default.questionsTimed;
        public static int QuestionTime { get => questionTime; set { questionTime = value; Properties.Settings.Default.questionTime = questionTime; Properties.Settings.Default.Save(); } }
        private static int questionTime = Properties.Settings.Default.questionTime;
        public static int TimeOutStartTime { get => timeOutStartTime; set { timeOutStartTime = value; Properties.Settings.Default.timeOutStartTime = timeOutStartTime; Properties.Settings.Default.Save(); } }
        private static int timeOutStartTime = Properties.Settings.Default.timeOutStartTime;
        public static Boolean Rumble { get => rumble; set { rumble = value; Properties.Settings.Default.rumble = rumble; Properties.Settings.Default.Save(); } }
        private static Boolean rumble = Properties.Settings.Default.rumble;
        public static Boolean RequireProctor { get => requireProctor; set { requireProctor = value; Properties.Settings.Default.requireProctor = requireProctor; Properties.Settings.Default.Save(); } }
        private static Boolean requireProctor = Properties.Settings.Default.requireProctor;
        public static Boolean RequirePlayers { get => requirePlayers; set { requirePlayers = value; Properties.Settings.Default.requirePlayers = requirePlayers; Properties.Settings.Default.Save(); } }
        private static Boolean requirePlayers = Properties.Settings.Default.requirePlayers;
    }
}
