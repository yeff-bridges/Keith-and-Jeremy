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

        /// <summary>
        /// Gets the executor associated with this command processor instance.
        /// </summary>
        public CommandExecutor Executor { get; private set; }

        /// <summary>
        /// Gets the parser associated with this command processor instance.
        /// </summary>
        public CommandParser Parser { get; private set; }

        /// <summary>
        /// Gets the built-in commands associated with this command processor instance.
        /// </summary>
        public BuiltinLibrary Builtins { get; private set; }

        public CommandProcessor(MainWindow outputWindow)
        {
            Window = outputWindow;
            Executor = new CommandExecutor(this);
            Builtins = new BuiltinLibrary(this);
            Parser = new CommandParser(this);
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
            Executor.WriteCommandLink(window.CurrentWorkingDirectory, chdirCommand);
            Executor.WriteInfoText($" ==> {command}\n");

            try
            {
                ProcessorCommand pCommand = Parser.Parse(command);
                if (pCommand is SingleProcessCommand)
                {
                    Executor.ExecuteSingleProcessCommand(pCommand);
                }
                else if (pCommand is MultiProcessCommand)
                {
                    Executor.ExecuteMultipleProcessCommand(pCommand);
                }
            }
            catch(KeyNotFoundException e)
            {
                Executor.WriteOutputText("Command not recognized.");
                return false; //return value may be used later, but unlikely
            }

            return true;
        }
    }
}
