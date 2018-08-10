using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    /// <summary>
    /// Class for parsing and processing commands.
    /// </summary>
    public class CommandProcessor
    {
        /// <summary>
        /// Parses and processes the specified command string.
        /// </summary>
        /// <param name="command">Command string to process.</param>
        /// <param name="window">Window to use for command output.</param>
        /// <returns>A value indicating whether we were able to successfully process the command.</returns>
        public bool ProcessCommand(string command, MainWindow window)
        {
            window.WriteInfoText($"{window.CurrentWorkingDirectory} ==> {command}");
            window.WriteOutputText($"Processing command: {command}");

            try
            {
                Process.Start(command);
            }
            catch
            {
                window.WriteInfoText("Command failed.");
                return false;
            }

            return true;
        }
    }
}
