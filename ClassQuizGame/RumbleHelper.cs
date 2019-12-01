using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ClassQuizGame
{
    public class RumbleHelper
    {
        private XInputController input;

        public enum RumbleType
        {
            wobbleRumble,
            burstRumble,
            shortRumble,
            longRumble
        }
        private RumbleType rumbleType;

        private DispatcherTimer rumbleTimer;
        private int rumbleState;
        private int rumbleCount;

        private const int SHORT_RUMBLE_TIME = 150;
        private const int RUMBLE_TIME = 500;
        private const int LONG_RUMBLE_TIME = 1000;

        private const int RUMBLE_POWER = (int)(short.MaxValue * 0.75f);
        private const int STRONG_RUMBLE_POWER = short.MaxValue;

        private const int WOBBLE_COUNT = 3;
        private const int BURST_COUNT = 2;

        private Vibration noVibration;

        public RumbleHelper(XInputController controller, RumbleType rType)
        {
            input = controller;
            rumbleType = rType;
            rumbleState = 0;
            rumbleCount = 0;
            rumbleTimer = new DispatcherTimer();
            switch (rumbleType)
            {
                case RumbleType.wobbleRumble:
                    rumbleTimer.Interval = new TimeSpan(0, 0, 0, 0, SHORT_RUMBLE_TIME);
                    break;
                case RumbleType.burstRumble:
                    rumbleTimer.Interval = new TimeSpan(0, 0, 0, 0, SHORT_RUMBLE_TIME);
                    break;
                case RumbleType.shortRumble:
                    rumbleTimer.Interval = new TimeSpan(0, 0, 0, 0, RUMBLE_TIME);
                    break;
                case RumbleType.longRumble:
                    rumbleTimer.Interval = new TimeSpan(0, 0, 0, 0, LONG_RUMBLE_TIME);
                    break;
            }
            rumbleTimer.Tick += new EventHandler(rumbleTimer_Tick);
            noVibration = new Vibration();
            noVibration.LeftMotorSpeed = 0;
            noVibration.RightMotorSpeed = 0;
        }

        public void start()
        {
            rumbleTimer_Tick(null, null);
            rumbleTimer.Start();
        }

        private void rumbleTimer_Tick(object sender, EventArgs e)
        {
            switch (rumbleType)
            {
                case RumbleType.wobbleRumble:
                    if (rumbleState == 0)
                    {
                        Vibration leftVibration = new Vibration();
                        leftVibration.LeftMotorSpeed = STRONG_RUMBLE_POWER;
                        leftVibration.RightMotorSpeed = 0;
                        input.setVibration(leftVibration);
                        rumbleState = 1;
                    }
                    else
                    {
                        Vibration rightVibration = new Vibration();
                        rightVibration.LeftMotorSpeed = 0;
                        rightVibration.RightMotorSpeed = STRONG_RUMBLE_POWER;
                        input.setVibration(rightVibration);
                        rumbleCount++;
                        rumbleState = 0;
                    }
                    if (rumbleCount >= WOBBLE_COUNT)
                    {
                        rumbleTimer.Stop();
                        input.setVibration(noVibration);
                    }
                    else { }
                    break;
                case RumbleType.burstRumble:
                    if (rumbleState == 0)
                    {
                        Vibration burstVibration = new Vibration();
                        burstVibration.LeftMotorSpeed = STRONG_RUMBLE_POWER;
                        burstVibration.RightMotorSpeed = STRONG_RUMBLE_POWER;
                        input.setVibration(burstVibration);
                        rumbleState = 1;
                    }
                    else
                    {
                        input.setVibration(noVibration);
                        rumbleCount++;
                        rumbleState = 0;
                    }
                    if (rumbleCount >= BURST_COUNT)
                    {
                        rumbleTimer.Stop();
                        input.setVibration(noVibration);
                    }
                    else { }
                    break;
                case RumbleType.shortRumble:
                case RumbleType.longRumble:
                    if (rumbleState == 0)
                    {
                        Vibration shortVibration = new Vibration();
                        shortVibration.LeftMotorSpeed = RUMBLE_POWER;
                        shortVibration.RightMotorSpeed = RUMBLE_POWER;
                        input.setVibration(shortVibration);
                        rumbleState = 1;
                    }
                    else
                    {
                        rumbleTimer.Stop();
                        input.setVibration(noVibration);
                    }
                    break;
            }
        }
    }
}
