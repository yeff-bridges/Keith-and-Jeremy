using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell.UIManagement
{
    /// <summary>
    /// A class for managing the UI windows and input/output streams.
    /// </summary>
    public class UIManager
    {
        /// <summary>
        /// Gets the default shell session to use for command processing.
        /// </summary>
        public ShellSession DefaultShellSession { get; set; }

        /// <summary>
        /// Gets or sets the list of shell sessions.
        /// </summary>
        private List<ShellSession> ShellSessions { get; set; }

        /// <summary>
        /// Gets the associated main application window instance.
        /// </summary>
        private MainWindow MainWindow { get; }

        /// <summary>
        /// Constructs a new UI manager instance.
        /// </summary>
        /// <param name="mainWindow">The main application window associated with the UI manager instance.</param>
        public UIManager(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            ShellSessions = new List<ShellSession>();
        }

        /// <summary>
        /// Initializes the UI by creating and hooking up the initial shell session.
        /// </summary>
        /// <param name="commandProcessor">The command processor instance to associate with the shell session.</param>
        public void SetupInitialShellSession(CommandProcessor commandProcessor)
        {
            // Create a new shell session collection and add an initial session to it.
            DefaultShellSession = new ShellSession(this, commandProcessor);
            ShellSessions = new List<ShellSession> { DefaultShellSession };

            // Replace the main window's "design-time" ConsoleWindow instance with a new one
            // that is properly hooked up to the shell session.
            DefaultShellSession.Window = new ConsoleWindow(DefaultShellSession);
            MainWindow.viewCommandOutput.Children.Clear();
            MainWindow.viewCommandOutput.Children.Add(DefaultShellSession.Window);

            // The our processes current directory as the initial current directory for the initial shell session.
            DefaultShellSession.CurrentDirectory = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Presents the command prompt as appropriate for the default shell session.
        /// </summary>
        public void PresentCommandPrompt()
        {
            MainWindow.PresentCommandPrompt(DefaultShellSession);
        }
    }
}
