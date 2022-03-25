using System;
using System.Windows.Input;

namespace ListDemo.ViewModels
{
    /// <summary>
    /// Generische Implementierung des ICommand Interfaces für das Command Binding.
    /// </summary>
    class RelayCommand : ICommand
    {
        private readonly Func<object, bool> canExecute;
        private readonly Action<object> execute;
        /// <summary>
        /// Konstruktor für Funktionen, die einen Paramter von CommandParameter in XAML bekommen.
        /// </summary>
        /// <param name="execute">Funktion, die ausgeführt wird, wenn der Button gedrückt wird.</param>
        /// <param name="canExecute">Funktion, die bestimmt, ob der Button aktiv ist.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        /// <summary>
        /// Konstruktor für Funktionen ohne CommandParameter.
        /// </summary>
        /// <param name="execute">Funktion, die ausgeführt wird, wenn der Button gedrückt wird.</param>
        /// <param name="canExecute">Funktion, die bestimmt, ob der Button aktiv ist.</param>
        public RelayCommand(Action execute, Func<bool> canExecute) : this((param) => execute(), (param) => canExecute())
        { }

        public RelayCommand(Action<object> execute) : this(execute, (param) => true)
        { }

        public RelayCommand(Action execute) : this((param) => execute(), (param) => true)
        { }

        /// <summary>
        /// Damit die Enabled Eigenschaft automatisch aktualisiert wird, wenn CanExecute aufgerufen
        /// wird.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Bestimmt, ob der Button aktiv ist.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// Funktion, die beim Klicken ausgeführt werden soll.
        /// </summary>
        /// <param name="parameter">Wird in XAML über CommandParameter übergeben.</param>
        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }
    }
}
