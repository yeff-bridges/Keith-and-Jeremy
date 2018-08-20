using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    /// <summary>
    /// Class for taking a valid command object and executing it. Handles interactions between window and library.
    /// </summary>
    public class CommandExecutor {
        private CommandProcessor _processor;
        private MainWindow _outputWindow;
        private string _currentWorkingDirectory;

        public CommandExecutor(CommandProcessor processor)
        {
            _processor = processor;
            _outputWindow = processor.Window;
        }

        /// <summary>
        /// Used to call the built-in method that handles the command passed to this method.
        /// </summary>
        /// <param name="command">The command from the user to be executed.</param>
        public void ExecuteSingleProcessCommand(ProcessorCommand command)
        {
            _processor.GetLib().runCommand(command);
            return;
        }

        /// <summary>
        /// Will be used in the future to execute a multiple process command.
        /// </summary>
        /// <param name="command">The command from the user to be executed.</param>
        public void ExecuteMultipleProcessCommand(ProcessorCommand command)
        {
            return;
        }

        /// <summary>
        /// Gets or sets the path to the current working directory.
        /// </summary>
        public string GetCurrentWorkingDirectory()
        {
            return _outputWindow.CurrentWorkingDirectory;
        }

        /// <summary>
        /// Writes a string of informational (non-command output) to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteInfoText(string outputText)
        {
            _outputWindow.WriteInfoText(outputText);
        }

        /// <summary>
        /// Writes a string of command output text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteOutputText(string outputText)
        {
            _outputWindow.WriteOutputText(outputText);
        }

        /// <summary>
        /// Writes a command hyperlink to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        /// <param name="command">Command line to associate with the hyperlink.</param>
        public void WriteCommandLink(string outputText, string command)
        {
            _outputWindow.WriteCommandLink(outputText, _outputWindow.RunShellRequestCommand, command);
        }
    }
}
