using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlyingLogbook.WPFUtilities
{
    /// <summary>
    /// Basic implementation of ICommand which can be assigned a function to execute when
    /// triggered, as well as an optional predicate to control if the command can be executed
    /// </summary>
    public class BasicCommand : ICommand
    {
        private Predicate<object> canExecute;
        private Action<object> execute;

        public BasicCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public BasicCommand(Action<object> execute)
        {
            this.execute = execute;
            this.canExecute = (object o) => true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }
    }
}
