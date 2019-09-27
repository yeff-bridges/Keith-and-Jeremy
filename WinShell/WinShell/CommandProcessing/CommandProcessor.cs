using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinShell.UIManagement;

namespace WinShell
{
    /// <summary>
    /// Class definition for a CommandProcessor object. An instance is used by the UIManager, which
    /// calls the ProcessCommand method here passed a string representation of the user's most recent input.
    /// </summary>
    public class CommandProcessor
    {
        /// <summary>
        /// Gets the UI Manager instance associated with this CommandProcessor.
        /// </summary>
        private UIManager UIManager { get; set; }

        /// <summary>
        /// Gets the output window associated with the current Command being processed.
        /// </summary>
        public ConsoleWindow Window { get; private set; }

        /// <summary>
        /// Gets the CommandExecutor associated with this CommandProcessor.
        /// </summary>
        public CommandExecutor Executor { get; private set; }

        /// <summary>
        /// Gets the CommandParser associated with this CommandProcessor.
        /// </summary>
        public CommandParser Parser { get; private set; }

        /// <summary>
        /// Gets the LibraryManager associated with this CommandProcessor.
        /// </summary>
        public LibraryManager LibManager { get; private set; }


        /// <summary>
        /// Constructor for all CommandProcessors. Instantiates new LibraryManager,
        /// CommandExecutor, and CommandParser objects to be used by this CommandProcessor instance,
        /// calls initLibraries method of the new LibraryManager, and stores the reference to the 
        /// UIManager for which this instance was created.
        /// </summary>
        /// <param name="uiManager">Reference to the UIManager instantiating this CommandProcessor.</param>
        public CommandProcessor(UIManager uiManager)
        {
            UIManager = uiManager;
            LibManager = new LibraryManager(this);
            Executor = new CommandExecutor(this);
            Parser = new CommandParser(this);

            LibManager.initLibraries();
        }

        /// <summary>
        /// Called by CommandWindows to process the command string input by the user. 
        /// If this results in a valid command being successfully ran, return true, otherwise return false.
        /// </summary>
        /// <param name="command">Command string to process.</param>
        /// <param name="shellSession">Shell session to use with this command.</param>
        /// <returns>A value indicating whether the command could be processed.</returns>
        public bool ProcessCommand(string command, ShellSession shellSession)
        {
            bool success = true;
            Window = shellSession.Window;

            var chdirCommand = $"cd \"{shellSession.CurrentDirectory}\"";
            Window.WriteCommandLink(shellSession.CurrentDirectory, Window.RunShellRequestCommand, chdirCommand);
            Window.WriteInfoText($" ==> {command}\n");

            // For command execution, switch to the session's current directory.
            Directory.SetCurrentDirectory(shellSession.CurrentDirectory);

            int lastCommandResult;

            try
            {
                List<string> argList = Parser.Parse(command);
                lastCommandResult = LibManager.runCommand(argList.ToArray());
                if (lastCommandResult != 0)
                {
                    return success = false;
                }
            }
            catch (InvalidCommandException e)
            {
                Executor.WriteInfoText(e.Message);
                success = false;
            }

            // Update the session's current directory based on the result of the command.
            shellSession.CurrentDirectory = Directory.GetCurrentDirectory();

            return success;
        }
    }
}
