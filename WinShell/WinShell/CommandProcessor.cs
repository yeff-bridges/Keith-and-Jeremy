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
        /// <summary>
        /// Gets the output window associated with this command.
        /// </summary>
        public MainWindow Window { get; private set; }

        private CommandExecutor _executor;
        private CommandParser _parser;
        private BuiltinLibrary _lib;

        public CommandProcessor(MainWindow outputWindow)
        {
            Window = outputWindow;
            _executor = new CommandExecutor(this);
            _lib = new BuiltinLibrary(this);
            _parser = new CommandParser(this);
        }

        /// <summary>
        /// Parses and processes the specified command string.
        /// </summary>
        /// <param name="command">Command string to process.</param>
        /// <param name="window">Window to use for command output.</param>
        /// <returns>A value indicating whether we were able to successfully process the command.</returns>
        public bool ProcessCommand(string command, MainWindow window)
        {
            Window = window;

            var chdirCommand = $"cd \"{window.CurrentWorkingDirectory}\"";
            _executor.WriteCommandLink(window.CurrentWorkingDirectory, chdirCommand);
            _executor.WriteInfoText($" ==> {command}\n");

            try
            {
                ProcessorCommand pCommand = _parser.Parse(command);
                if (pCommand is SingleProcessCommand)
                {
                    _executor.ExecuteSingleProcessCommand(pCommand);
                }
                else if (pCommand is MultiProcessCommand)
                {
                    _executor.ExecuteMultipleProcessCommand(pCommand);
                }
            }
            catch(KeyNotFoundException e)
            {
                _executor.WriteOutputText("Command not recognized.");
                return false; //return value may be used later, but unlikely
            }

            return true;
        }


        //
        // Getter functions below.
        //

        public CommandExecutor GetExecutor()
        {
            return _executor;
        }

        public BuiltinLibrary GetLib()
        {
            return _lib;
        }
    }
}
