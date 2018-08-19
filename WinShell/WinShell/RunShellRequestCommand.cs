using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinShell
{
    /// <summary>
    /// A class representing a UI command for running a shell request.
    /// </summary>
    public class RunShellRequestCommand : ICommand
    {
        /// <summary>
        /// The main window associated with this command.
        /// </summary>
        public MainWindow MainWindow { get; set; }

        /// <summary>
        /// Returns a value indicating whether the process is in a state that supports executing this command.
        /// </summary>
        /// <param name="parameter">The parameter object associated with the command request.</param>
        /// <returns>A value indicating whether the UI should make this command available to the user via the UI.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command by passing the parameter as a string argument to the CommandProcessor.
        /// </summary>
        /// <param name="parameter">The parameter object associated with the command request.</param>
        public void Execute(object parameter)
        {
            MainWindow.ProcessCommand(parameter as string);
            MainWindow.PresentCommandPrompt();
        }

        /// <summary>
        /// An event that listeners can use for dynamically determining when "can execute" status changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;
    }
}
