using System;
using System.Windows;
using System.Windows.Input;

namespace Unfocus
{
    public class ShowWindowCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Window window;

        public ShowWindowCommand(Window window)
        {
            this.window = window;
        }

        public bool CanExecute(object parameter)
        {
            return window != null;
        }

        public void Execute(object parameter)
        {
            window.Visibility = Visibility.Visible;
        }
    }
}
