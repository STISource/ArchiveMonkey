using System;
using System.Windows.Input;

namespace ArchiveMonkey.Settings.Helpers
{
    public class SimpleCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public SimpleCommand(Func<bool> canExecute, Action execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }        

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {   
            this.CanExecuteChanged?.Invoke(this, new EventArgs());           
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            this.execute.Invoke();
        }
    }
}
