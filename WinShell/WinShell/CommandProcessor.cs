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
        private string[] builtin_str =
        {
            "cd"
        };

        private string[] builtin_sym =
        {
            "|",
            "<",
            ">"
        };



        /// <summary>
        /// Parses and processes the specified command string.
        /// </summary>
        /// <param name="command">Command string to process.</param>
        /// <param name="window">Window to use for command output.</param>
        /// <returns>A value indicating whether we were able to successfully process the command.</returns>
        public bool ProcessCommand(string command, MainWindow window)
        {
            bool builtin = false;

            window.WriteInfoText($"{window.CurrentWorkingDirectory} ==> {command}");
            window.WriteOutputText($"Processing command: {command}");

            if(!builtin){
                Launch(command, window);
            }

            return true;
        }

        /// <summary>
        /// Creates a process specified by the command string.
        /// <param name="command">Command string to process.</param>
        /// <param name="window">Window to use for command output.</param>
        /// <returns>A value indicating whether we were able to successfully launch a new process.</returns>
        public bool Launch(string command, MainWindow window){
            try
            {
                Process.Start(command);
            }
            catch
            {
                window.WriteInfoText($"Command failed. Could not find: {command}");
                return false;
            }

            return true;
        }
    }
}
