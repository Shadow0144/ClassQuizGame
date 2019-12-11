using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for TitleSplashControl.xaml
    /// </summary>
    public partial class TitleSplashControl : UserControl
    {
        public Boolean IsComplete;

        private MediaPlayer readySound;

        public TitleSplashControl(String title)
        {
            InitializeComponent();

            TitleTextBlock.Text = title;

            App.getInstance().xKeyboard.OnSpacePressed += () => finish();

            setScale(MainWindow.getInstance().getXScale(), MainWindow.getInstance().getYScale());

            readySound = new MediaPlayer();
            readySound.ScrubbingEnabled = false;
            readySound.Open(new Uri(@"assets/ready_prompt.mp3", UriKind.Relative));
            readySound.Stop();

            IsComplete = false;
        }

        public void update()
        {
            if (Settings.RequirePlayers && !Settings.RequireProctor)
            {
                for (int i = 0; i < 5; i++)
                {
                    PlayerControl playerControl = MainWindow.getInstance().GetPlayerControl(i);
                    if (playerControl.inGame && playerControl.readied)
                    {
                        finish();
                    }
                    else { }
                }
            }
            else { }
        }

        private void finish()
        {
            if (!Settings.Mute)
            {
                readySound.Play();
            }
            else { }
            IsComplete = true;
            App.getInstance().xKeyboard.OnSpacePressed = null;
        }

        public void setScale(float xScale, float yScale)
        {
            TitleTextBlock.FontSize = 72 * xScale;
            LeftSpotlightTranslate.X = -50 * xScale;
            LeftSpotlightTranslate.Y = 175 * yScale;
            RightSpotlightTranslate.X = 50 * xScale;
            RightSpotlightTranslate.Y = 175 * yScale;
        }
    }
}
