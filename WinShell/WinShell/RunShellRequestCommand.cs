using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinShell.UIManagement;

namespace WinShell
{
    /// <summary>
    /// A class representing a UI command for running a shell request.
    /// </summary>
    public class RunShellRequestCommand : ICommand
    {
        /// <summary>
        /// The shell session associated with this command.
        /// </summary>
        public ShellSession ShellSession { get; private set; }

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
            if (ShellSession != null)
            {
                ShellSession.ProcessCommand(parameter as string);
                ShellSession.UIManager.PresentCommandPrompt();
            }
        }

        /// <summary>
        /// An event that listeners can use for dynamically determining when "can execute" status changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Constructs a new RunShellRequestCommand instance associated with a specific shell session.
        /// </summary>
        /// <param name="shellSession">The shell session associated with this RunShellRequestCommand instance.</param>
        public RunShellRequestCommand(ShellSession shellSession)
        {
            ShellSession = shellSession;
        }
    }
}
