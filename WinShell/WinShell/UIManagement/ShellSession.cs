using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinShell.UIManagement
{
    /// <summary>
    /// A class representing the combination of output window, current directory, etc. that
    /// makes up a UI session with the shell.
    /// </summary>
    public class ShellSession
    {
        /// <summary>
        /// The window used for presenting console I/O.
        /// </summary>
        public ConsoleWindow Window { get; set; }

        /// <summary>
        /// Gets or sets the current directory associated with the shell session.
        /// </summary>
        public string CurrentDirectory { get; set; }

        /// <summary>
        /// Gets the UI Manager associated with this shell session.
        /// </summary>
        public UIManager UIManager { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether keyboard and display I/O is being redirected to an external console application.
        /// </summary>
        public bool UiRedirectionActive { get; set; }

        /// <summary>
        /// Gets or sets the target stream for redirecting standard input.
        /// </summary>
        public StreamWriter UiTargetStandardIOStream { get; set; }

        /// <summary>
        /// Gets the command processor associated with this shell session.
        /// </summary>
        private CommandProcessor CommandProcessor { get; }

        /// <summary>
        /// Constructs a new shell session instance.
        /// </summary>
        /// <param name="uiManager">The UI manager associated with this shell session.</param>
        /// <param name="commandProcessor">The command processor associated with this shell session.</param>
        public ShellSession(UIManager uiManager, CommandProcessor commandProcessor)
        {
            UIManager = uiManager;
            CommandProcessor = commandProcessor;
            UiRedirectionActive = false;
        }

        /// <summary>
        /// Prepares the UI output window for the next command, and then calls the command processor to process the command.
        /// </summary>
        /// <param name="command">User input taken from window to be used as a command.</param>
        public void ProcessCommand(string command)
        {
            //Processor.ProcessCommand(command, this);
            Window.StartNextOutputGrouping();
            CommandProcessor.ProcessCommand(command, this);
        }
    }
}
