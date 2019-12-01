using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ClassQuizGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public XInputController[] controllers;
        public XKeyboard xKeyboard;

        public int players;

        private static App instance;

        public App()
        {
            instance = this;

            players = 0;
            controllers = new XInputController[4];
            controllers[0] = new XInputController(0);
            controllers[1] = new XInputController(1);
            controllers[2] = new XInputController(2);
            controllers[3] = new XInputController(3);

            xKeyboard = new XKeyboard();
        }

        public static App getInstance()
        {
            return instance;
        }
    }
}
