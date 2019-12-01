using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpDX.XInput;

namespace ClassQuizGame
{
    public class XInputController
    {
        Controller controller;
        public bool connected;
        public int index;

        public Point LThumb;
        public Point RThumb;
        public float LTrigger;
        public float RTrigger;

        public Boolean ADown;
        public Boolean BDown;
        public Boolean XDown;
        public Boolean YDown;
        public Boolean StartDown;
        public Boolean BackDown;
        public Boolean LDown;
        public Boolean RDown;
        public Boolean LStickDown;
        public Boolean RStickDown;

        public Boolean APressed;
        public Boolean BPressed;
        public Boolean XPressed;
        public Boolean YPressed;
        public Boolean StartPressed;
        public Boolean BackPressed;
        public Boolean LPressed;
        public Boolean RPressed;
        public Boolean LStickPressed;
        public Boolean RStickPressed;

        public XInputController(int index)
        {
            this.index = index;
            switch (index)
            {
                case 0:
                    controller = new Controller(UserIndex.One);
                    break;
                case 1:
                    controller = new Controller(UserIndex.Two);
                    break;
                case 2:
                    controller = new Controller(UserIndex.Three);
                    break;
                case 3:
                    controller = new Controller(UserIndex.Four);
                    break;
                default:
                    controller = new Controller(UserIndex.One);
                    break;
            }
            connected = controller.IsConnected;

            LThumb = new Point(0, 0);
            RThumb = new Point(0, 0);
            LTrigger = 0.0f;
            RTrigger = 0.0f;
            ADown = false;
            BDown = false;
            XDown = false;
            YDown = false;
            StartDown = false;
            BackDown = false;
            LDown = false;
            RDown = false;
            LStickDown = false;
            RStickDown = false;

            APressed = false;
            BPressed = false;
            XPressed = false;
            YPressed = false;
            StartPressed = false;
            BackPressed = false;
            LPressed = false;
            RPressed = false;
            LStickPressed = false;
            RStickPressed = false;
        }

        public void Update()
        {
            connected = controller.IsConnected;
            if (connected)
            {
                Gamepad gamepad = controller.GetState().Gamepad;

                LThumb.X = (gamepad.LeftThumbX < -Gamepad.LeftThumbDeadZone || gamepad.LeftThumbX > Gamepad.LeftThumbDeadZone) ? 0.0f : ((float)gamepad.LeftThumbX / short.MaxValue); // * 1.0f);
                LThumb.Y = (gamepad.LeftThumbY < -Gamepad.LeftThumbDeadZone || gamepad.LeftThumbY > Gamepad.LeftThumbDeadZone) ? 0.0f : ((float)gamepad.LeftThumbY / short.MaxValue); // * 1.0f);
                RThumb.Y = (gamepad.RightThumbX < -Gamepad.RightThumbDeadZone || gamepad.RightThumbX > Gamepad.RightThumbDeadZone) ? 0.0f : ((float)gamepad.RightThumbX / short.MaxValue); // * 1.0f);
                RThumb.X = (gamepad.RightThumbY < -Gamepad.RightThumbDeadZone || gamepad.RightThumbY > Gamepad.RightThumbDeadZone) ? 0.0f : ((float)gamepad.RightThumbY / short.MaxValue); // * 1.0f);

                LTrigger = (Math.Abs(gamepad.LeftTrigger) < Gamepad.TriggerThreshold) ? 0.0f : gamepad.LeftTrigger;
                RTrigger = (Math.Abs(gamepad.RightTrigger) < Gamepad.TriggerThreshold) ? 0.0f : gamepad.RightTrigger;

                APressed = !ADown && (gamepad.Buttons == GamepadButtonFlags.A);
                ADown = (gamepad.Buttons == GamepadButtonFlags.A);

                BPressed = !BDown && (gamepad.Buttons == GamepadButtonFlags.B);
                BDown = (gamepad.Buttons == GamepadButtonFlags.B);

                XPressed = !XDown && (gamepad.Buttons == GamepadButtonFlags.X);
                XDown = (gamepad.Buttons == GamepadButtonFlags.X);

                YPressed = !YDown && (gamepad.Buttons == GamepadButtonFlags.Y);
                YDown = (gamepad.Buttons == GamepadButtonFlags.Y);

                StartPressed = !StartDown && (gamepad.Buttons == GamepadButtonFlags.Start);
                StartDown = (gamepad.Buttons == GamepadButtonFlags.Start);

                BackPressed = !BackDown && (gamepad.Buttons == GamepadButtonFlags.Back);
                BackDown = (gamepad.Buttons == GamepadButtonFlags.Back);

                LPressed = !LDown && (gamepad.Buttons == GamepadButtonFlags.LeftShoulder);
                LDown = (gamepad.Buttons == GamepadButtonFlags.LeftShoulder);

                RPressed = !RDown && (gamepad.Buttons == GamepadButtonFlags.RightShoulder);
                RDown = (gamepad.Buttons == GamepadButtonFlags.RightShoulder);

                LStickPressed = !LStickDown && (gamepad.Buttons == GamepadButtonFlags.LeftThumb);
                LStickDown = (gamepad.Buttons == GamepadButtonFlags.LeftThumb);

                RStickPressed = !RStickDown && (gamepad.Buttons == GamepadButtonFlags.RightThumb);
                RStickDown = (gamepad.Buttons == GamepadButtonFlags.RightThumb);
            }
            else { }
        }

        public void setVibration(Vibration vibration)
        {
            controller.SetVibration(vibration);
        }
    }
}
