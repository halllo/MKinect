using System;
using System.Windows.Input;

namespace GestureRun
{
    class ActionCommand : ICommand
    {
        Action _toExe;
        public ActionCommand(Action toExe)
        {
            _toExe = toExe;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _toExe();
        }
    }
}
