using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private string[] _builtin_str =
        {
            "cd",
            "help",
            "exit",
            "pwd",
        };

        private bool _hasSymbol = false;
        private char[] _builtin_sym =
        {
            ' ',
            '|',
            '<',
            '>',
        };

        delegate int CommandMethod(string[] args);

        private CommandMethod[] _builtin_func;

        private MainWindow _outputWindow;

        public CommandProcessor()
        {
            _builtin_func = new CommandMethod[]
            {
                CommandCD,
                CommandHelp,
                CommandHelp,
                CommandPrintWorkingDirectory,
            };
        }

        int CommandCD(string[] args)
        {
            return 0;
        }

        int CommandHelp(string[] args)
        {
            return 0;
        }

        int CommandPrintWorkingDirectory(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            WriteOutputText($"Current directory: {currentDirectory}\n");

            return 0;
        }

        /// <summary>
        /// Parses and processes the specified command string.
        /// </summary>
        /// <param name="command">Command string to process.</param>
        /// <param name="window">Window to use for command output.</param>
        /// <returns>A value indicating whether we were able to successfully process the command.</returns>
        public bool ProcessCommand(string command, MainWindow window)
        {
            bool builtin = false;
            _outputWindow = window;

            var chdirCommand = $"cd {window.CurrentWorkingDirectory}";
            WriteCommandLink(window.CurrentWorkingDirectory, chdirCommand);
            WriteInfoText($" ==> {command}\n");
            WriteOutputText($"Processing command: {command}\n");

            int commandIndex = -1;
            switch (command)
            {
                case "pwd":
                    commandIndex = 3;
                    break;
            }

            builtin = (commandIndex >= 0);

            if (!builtin)
            {
                Launch(command);
            }
            else
            {
                var args = new string[] { "Arg0", "Arg1" };
                _builtin_func[commandIndex](args);
            }

            return true;
        }

        /// <summary>
        /// Creates a process specified by the command string.
        /// <param name="command">Command string to process.</param>
        /// <returns>A value indicating whether we were able to successfully launch a new process.</returns>
        public bool Launch(string command){
            try
            {
                Process.Start(command);
            }
            catch
            {
                WriteInfoText($"Command failed. Could not find: {command}\n");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Splits a string into tokens based on certain delimiters and returns an array to the  caller.
        /// If the string was split by anything other than white space, a boolean is set.
        /// </summary>
        private string[] GetArgs(string command)
        {
            string[] argv = new string[20];
            StringBuilder token = new StringBuilder();
            int argc = 0, count = 0;
            _hasSymbol = false;

            foreach(char c in command)
            {
                foreach(char sym in _builtin_sym)
                {
                    count++;
                    if (sym == c)
                    {
                        if (c == ' ')
                        {
                            argv[argc] = token.ToString();
                        }
                        else
                        {
                            _hasSymbol = true;
                            argv[argc] = c.ToString();
                        }   
                        token.Clear();
                        break;
                    }
                    else if (count == _builtin_sym.Length)
                    {
                        token.Append(c);
                    }
                }
            }

            return argv;
        }

        /// <summary>
        /// Writes a string of informational (non-command output) to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        private void WriteInfoText(string outputText)
        {
            _outputWindow.WriteInfoText(outputText);
        }

        /// <summary>
        /// Writes a string of command output text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        private void WriteOutputText(string outputText)
        {
            _outputWindow.WriteOutputText(outputText);
        }

        /// <summary>
        /// Writes a command hyperlink to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        /// <param name="command">Command line to associate with the hyperlink.</param>
        private void WriteCommandLink(string outputText, string command)
        {
            _outputWindow.WriteCommandLink(outputText, _outputWindow.ProcessorCommand, command);
        }
    }
}
