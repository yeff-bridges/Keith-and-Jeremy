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

        delegate int CommandMethod(IEnumerable<string> args);

        private CommandMethod[] _builtin_func;

        private MainWindow _outputWindow;

        public CommandProcessor()
        {
            _builtin_func = new CommandMethod[]
            {
                CommandCD,
                CommandHelp,
                CommandExit,
                CommandPrintWorkingDirectory,
            };
        }

        /// <summary>
        /// Changes the working directory to the path stored in the second slot of args.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        int CommandCD(IEnumerable<string> args)
        {
            try
            {
                Directory.SetCurrentDirectory(args.ElementAt(1));
            }
            catch
            {
                WriteOutputText("There was a problem with the file path entered\n");
                //alternate exit status to be determined later
            }

            return 0;
        }

        /// <summary>
        /// Prints a help message for the user.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        int CommandHelp(IEnumerable<string> args)
        {
            WriteOutputText("Help is on the way!\n");
            return 0;
        }

        /// <summary>
        /// Exits the shell.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        int CommandExit(IEnumerable<string> args)
        {
            WriteOutputText("There is no exit!\n");
            return 0;
        }

        /// <summary>
        /// Prints the current working directory.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        int CommandPrintWorkingDirectory(IEnumerable<string> args)
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
            _outputWindow = window;

            var chdirCommand = $"cd {window.CurrentWorkingDirectory}";
            WriteCommandLink(window.CurrentWorkingDirectory, chdirCommand);
            WriteInfoText($" ==> {command}\n");
            WriteOutputText($"Processing command: {command}\n");

            int commandIndex = 0;

            var args = GetArgs(command);

            //this process will be handled differently by a seperate object in the future.
            if (_hasSymbol)
            {
                HandleMultiprocessCommand(args);
                _hasSymbol = false;
                return true;
            }

            foreach(string str in _builtin_str)
            {
                if (str.Equals(args.ElementAt(0)))
                {
                    _builtin_func[commandIndex](args);
                    return true;
                }
                commandIndex++;
            }

            //Should the command be unrecognized by this point, attempt to launch the first argument.
            Launch(args);
            
            return true;
        }

        /// <summary>
        /// Creates a process specified by the list of arguments passed.
        /// <param name="args">Set of arguments, the first being the program to launch.</param>
        /// <returns>A value indicating whether we were able to successfully launch a new process.</returns>
        public bool Launch(IEnumerable<string> args){
            try
            {
                Process.Start(args.ElementAt(0));
            }
            catch
            {
                WriteInfoText($"Command failed. Could not find: {args.ElementAt(0)}\n");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Splits a string into tokens based on certain delimiters.
        /// If the string was split by anything other than white space, _hasSymbol is set.
        /// </summary>
        /// <param name="command">String to split into arguments.</param>
        /// <returns>A list of arguments to be processed.</returns>
        private IEnumerable<string> GetArgs(string command)
        {
            List<string> argv = new List<string>();
            StringBuilder token = new StringBuilder();
            int checkedSymbolCount = 0;
            _hasSymbol = false;

            foreach(char c in command)
            {
                foreach(char sym in _builtin_sym)
                {
                    checkedSymbolCount++;
                    if (sym == c)
                    {
                        if (c == ' ')
                        {
                            EasyAdd<string>(argv, token.ToString());
                        }
                        else
                        {
                            _hasSymbol = true;
                            EasyAdd<string>(argv, token.ToString());
                            EasyAdd<string>(argv, c.ToString());
                        }
                        token.Clear();
                        break;
                    }
                    else if (checkedSymbolCount == _builtin_sym.Length)
                    {
                        token.Append(c);
                    }
                }
                checkedSymbolCount = 0;
            }

            EasyAdd<string>(argv, token.ToString());
            return argv;
        }

        /// <summary>
        /// Starts the work of handling commands involving multiple processes.
        /// </summary>
        /// <param name="args"></param>
        private void HandleMultiprocessCommand(IEnumerable<string> args)
        {
            WriteOutputText("To be added soon!\n");
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

        private void EasyAdd<T>(List<T> list, T item)
        {
            if (item != null)
            {
                if (!(item.GetType() == typeof(string)) || !String.IsNullOrWhiteSpace(item as string))
                {
                    list.Add(item);
                }
            }
        }
    }
}
