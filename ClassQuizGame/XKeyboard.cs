﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClassQuizGame
{
    public class XKeyboard
    {
        public Boolean SpaceDown;
        public Boolean SpacePressed;

        public Boolean EnterDown;
        public Boolean EnterPressed;

        public Boolean ADown;
        public Boolean APressed;

        public Boolean AShiftDown;
        public Boolean AShiftPressed;

        public Boolean BDown;
        public Boolean BPressed;

        public Boolean BShiftDown;
        public Boolean BShiftPressed;

        public Boolean XDown;
        public Boolean XPressed;

        public Boolean XShiftDown;
        public Boolean XShiftPressed;

        public Boolean YDown;
        public Boolean YPressed;

        public Boolean YShiftDown;
        public Boolean YShiftPressed;

        public Boolean LDown;
        public Boolean LPressed;

        public Boolean LShiftDown;
        public Boolean LShiftPressed;

        public Boolean RDown;
        public Boolean RPressed;

        public Boolean RShiftDown;
        public Boolean RShiftPressed;

        public Action OnSpacePressed;
        public Action OnEnterPressed;
        public Action OnAPressed;
        public Action OnBPressed;
        public Action OnXPressed;
        public Action OnYPressed;
        public Action OnLPressed;
        public Action OnRPressed;
        public Action OnAShiftPressed;
        public Action OnBShiftPressed;
        public Action OnXShiftPressed;
        public Action OnYShiftPressed;
        public Action OnLShiftPressed;
        public Action OnRShiftPressed;

        public XKeyboard()
        {
            SpaceDown = false;
            SpacePressed = false;

            EnterDown = false;
            EnterPressed = false;

            ADown = false;
            APressed = false;

            AShiftDown = false;
            AShiftPressed = false;

            BDown = false;
            BPressed = false;

            BShiftDown = false;
            BShiftPressed = false;

            XDown = false;
            XPressed = false;

            XShiftDown = false;
            XShiftPressed = false;

            YDown = false;
            YPressed = false;

            YShiftDown = false;
            YShiftPressed = false;

            LShiftDown = false;
            LShiftPressed = false;

            LShiftDown = false;
            LShiftPressed = false;

            RDown = false;
            RPressed = false;

            RShiftDown = false;
            RShiftPressed = false;

            // TODO - Check for focus and add other handlers
        }

        public void Update()
        {
            SpacePressed = !SpaceDown && (Keyboard.IsKeyDown(Key.Space));
            SpaceDown = Keyboard.IsKeyDown(Key.Space);
            if (OnSpacePressed != null && SpacePressed)
            {
                OnSpacePressed();
            }
            else { }

            EnterPressed = !EnterDown && (Keyboard.IsKeyDown(Key.Enter));
            EnterDown = Keyboard.IsKeyDown(Key.Enter);
            if (OnEnterPressed != null && EnterPressed)
            {
                OnEnterPressed();
            }
            else { }

            APressed = !ADown && (Keyboard.IsKeyDown(Key.A)
                && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            ADown = Keyboard.IsKeyDown(Key.A) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnAPressed != null && APressed)
            {
                OnAPressed();
            }
            else { }

            AShiftPressed = !AShiftDown && (Keyboard.IsKeyDown(Key.A) 
                && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            AShiftDown = Keyboard.IsKeyDown(Key.A) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnAShiftPressed != null && AShiftPressed)
            {
                OnAShiftPressed();
            }
            else { }

            BPressed = !BDown && (Keyboard.IsKeyDown(Key.B)
                && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            BDown = Keyboard.IsKeyDown(Key.B) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnBPressed != null && BPressed)
            {
                OnBPressed();
            }
            else { }

            BShiftPressed = !BShiftDown && (Keyboard.IsKeyDown(Key.B)
                && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            BShiftDown = Keyboard.IsKeyDown(Key.B) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnBShiftPressed != null && BShiftPressed)
            {
                OnBShiftPressed();
            }
            else { }

            XPressed = !XDown && (Keyboard.IsKeyDown(Key.X)
                && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            XDown = Keyboard.IsKeyDown(Key.X) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnXPressed != null && XPressed)
            {
                OnXPressed();
            }
            else { }

            XShiftPressed = !XShiftDown && (Keyboard.IsKeyDown(Key.X)
                && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            XShiftDown = Keyboard.IsKeyDown(Key.X) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnXShiftPressed != null && XShiftPressed)
            {
                OnXShiftPressed();
            }
            else { }

            YPressed = !YDown && (Keyboard.IsKeyDown(Key.Y)
                && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            YDown = Keyboard.IsKeyDown(Key.Y) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnYPressed != null && YPressed)
            {
                OnYPressed();
            }
            else { }

            YShiftPressed = !YShiftDown && (Keyboard.IsKeyDown(Key.Y)
                && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            YShiftPressed = Keyboard.IsKeyDown(Key.Y) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnYShiftPressed != null && YShiftPressed)
            {
                OnYShiftPressed();
            }
            else { }

            LPressed = !LDown && (Keyboard.IsKeyDown(Key.L)
                && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            LDown = Keyboard.IsKeyDown(Key.L) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnLPressed != null && LPressed)
            {
                OnLPressed();
            }
            else { }

            LShiftPressed = !LShiftDown && (Keyboard.IsKeyDown(Key.L)
                && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            LShiftDown = Keyboard.IsKeyDown(Key.L) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnLShiftPressed != null && LShiftPressed)
            {
                OnLShiftPressed();
            }
            else { }

            RPressed = !RDown && (Keyboard.IsKeyDown(Key.R)
                && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            RDown = Keyboard.IsKeyDown(Key.R) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnRPressed != null && RPressed)
            {
                OnRPressed();
            }
            else { }

            RShiftPressed = !RShiftDown && (Keyboard.IsKeyDown(Key.R)
                && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
            RShiftPressed = Keyboard.IsKeyDown(Key.R) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            if (OnRShiftPressed != null && RShiftPressed)
            {
                OnRShiftPressed();
            }
            else { }
        }
    }
}
