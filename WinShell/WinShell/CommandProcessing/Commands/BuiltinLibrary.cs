using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    /// <summary>
    /// Descriptive enum for classifying single process commands. To be used thoughout the namespace.
    /// </summary>
    public enum SingleCommandType
    {
        //Directory
        ChangeDirectory,
        PrintCurrentDirectory,
        ViewDirectory,

        //File Operations
        CreateFile,
        MoveFile,
        CopyFile,
        DeleteFile,

        //Misc
        Execute,
        Launch,
        ChMod,
        ChOwn,
        Exit,
        Help,

        //Default
        NULL
    };

    /// <summary>
    /// Descriptive enums for commands using multiple processes.
    /// </summary>
    public enum MultiCommandType
    {
        Pipe,
        CondMultiLaunch,
        UncoMultiLaunch,
        Test,

        //Default
        NULL
    };


    /// <summary>
    /// Class for storing and adding logic for builtin commands.
    /// </summary>
    public class BuiltinLibrary
    {
        private CommandExecutor _executor;
        public Dictionary<string, SingleCommandType> SingleBuiltins { get; set; }
        public Dictionary<string, MultiCommandType> MultiBuiltins { get; set; }

        /// <summary>
        /// This function initializes the Dictionary that holds all commands that work with only one process.
        /// </summary>
        /// <returns>Returns the dictionary to be stored in _singleBuiltins.</returns>
        private Dictionary<string, SingleCommandType> InitSingleDict()
        {
            var dict = new Dictionary<string, SingleCommandType> {
                { "cd", SingleCommandType.ChangeDirectory },
                { "pwd", SingleCommandType.PrintCurrentDirectory },
                { "dir", SingleCommandType.ViewDirectory },
                { "creat", SingleCommandType.CreateFile },
                { "mov", SingleCommandType.MoveFile },
                { "cp", SingleCommandType.CopyFile },
                { "del", SingleCommandType.DeleteFile },
                { "exec", SingleCommandType.Execute },
                { "chown", SingleCommandType.ChOwn },
                { "chmod", SingleCommandType.ChMod },
                { "help", SingleCommandType.Help },
                { "exit", SingleCommandType.Exit },
            };

            return dict;
        }

        /// <summary>
        /// This function initializes the Dictionary that holds all commands that work with more than one process.
        /// </summary>
        /// <returns>Returns the dictionary to be stored in _multiBuiltins.</returns>
        private Dictionary<string, MultiCommandType> InitMultiDict()
        {
            var dict = new Dictionary<string, MultiCommandType> {
                { "|", MultiCommandType.Test },
                { "&", MultiCommandType.CondMultiLaunch },
                { "+", MultiCommandType.UncoMultiLaunch },
                { "Test", MultiCommandType.Test },
            };

            return dict;
        }

        public BuiltinLibrary(CommandProcessor processor)
        {
            SingleBuiltins = InitSingleDict();
            MultiBuiltins = InitMultiDict();
            _executor = processor.Executor;
        }


        ///
        /// Begin built-in single process function implementations below
        ///
        

        /// <summary>
        /// Changes the working directory to the path stored in the second slot of args.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        private int CommandCD(IEnumerable<string> args)
        {
            try
            {
                Directory.SetCurrentDirectory(args.ElementAt(1));
            }
            catch (Exception ex)
            {
                _executor.WriteInfoText($"Command failed: {ex.Message}\n");
                //alternate exit status to be determined later
            }

            return 0;
        }

        /// <summary>
        /// Prints a help message for the user.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        private int CommandHelp(IEnumerable<string> args)
        {
            _executor.WriteOutputText("Help is on the way!\n");
            return 0;
        }

        /// <summary>
        /// Exits the shell.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        private int CommandExit(IEnumerable<string> args)
        {
            Environment.Exit(0); //This feels slow. The close API is another option
            return 0;
        }

        /// <summary>
        /// Prints the current working directory.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        private int CommandPrintWorkingDirectory(IEnumerable<string> args)
        {
            _executor.WriteOutputText($"{_executor.GetCurrentWorkingDirectory()}");
            return 0;
        }

        /// <summary>
        /// Displays a directory listing.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>An integer representing the exit status of the operation.</returns>
        private int CommandDirectory(IEnumerable<string> args)
        {
            try
            {
                var path = args.Count() >= 2 ? args.ElementAt(1) : _executor.GetCurrentWorkingDirectory(); 
                IEnumerable<string> directories = Directory.EnumerateDirectories(path); //FLAG
                IEnumerable<string> files = Directory.EnumerateFiles(path); 
                var targetDir = Path.GetFullPath(path);

                // Display the directory banner.
                _executor.WriteOutputText("Directory of ");
                _executor.WriteCommandLink($"{targetDir}\n", $"cd \"{targetDir}\"");

                // Display an output line for each directory in the listing.
                foreach (var directory in directories)
                {
                    var fullPath = Path.GetFullPath(directory);
                    var filespec = Path.GetFileName(fullPath);
                    var dirInfo = new DirectoryInfo(directory);
                    if (dirInfo.Exists)
                    {
                        var lastUpdated = dirInfo.LastWriteTime;
                        _executor.WriteOutputText($"{lastUpdated.ToString("MM/dd/yyyy")}  {lastUpdated.ToString("hh:mm tt")}    ");
                        _executor.WriteCommandLink($"<DIR>", $"dir \"{fullPath}\"");
                        _executor.WriteOutputText("          ");
                        _executor.WriteCommandLink($"{filespec}\n", $"cd \"{fullPath}\"");
                    }
                }

                // Display an output line for each file in the listing.
                foreach (var file in files)
                {
                    var fullPath = Path.GetFullPath(file); 
                    var filespec = Path.GetFileName(fullPath);
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Exists)
                    {
                        var lastUpdated = fileInfo.LastWriteTime;
                        var length = fileInfo.Length;
                        _executor.WriteOutputText($"{lastUpdated.ToString("MM/dd/yyyy")}  {lastUpdated.ToString("hh:mm tt")}    {length.ToString("##,###,###,###").PadLeft(14)} ");
                        _executor.WriteCommandLink($"{filespec}\n", $"exec \"{fullPath}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                _executor.WriteInfoText($"Command failed: {ex.Message}\n");
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Executes the specified command as a new process without checking to see if it's an internal shell command.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>An integer representing the exit status of the operation.</returns>
        private int CommandExecute(IEnumerable<string> args)
        {
            Launch(args.Skip(1));

            return 0;
        }

        /// <summary>
        /// Creates a process specified by the list of arguments passed.
        /// <param name="args">Set of arguments, the first being the program to launch.</param>
        /// <returns>A value indicating whether we were able to successfully launch a new process.</returns>
        private bool Launch(IEnumerable<string> args)
        {
            try
            {
                Process.Start(args.ElementAt(0));
            }
            catch (Exception e)
            {
                _executor.WriteInfoText($"Command failed: {e.Message}\n");
                return false;
            }

            return true;
        }


        ///
        ///End built-in single process function implementations above
        ///


        ///
        ///Begin built-in multi process function implementations below
        ///

        private bool TestFunction(ProcessorCommand command)
        {
            _executor.WriteInfoText("Test command started\n");
            try
            {
                _executor.WriteInfoText("Test command complete\n");
            }catch (Exception e)
            {
                _executor.WriteInfoText($"Test command failed: {e.Message}\n");
            }
            
            return true;
        }

        ///
        ///End built-in multi process function implementations belove
        ///


        ///
        ///Begin support functions below
        ///


        /// <summary>
        /// Support method used to call correct built-in function using command type. 
        /// Only used on SingleProcessCommands.
        /// </summary>
        /// <param name="command">Command input by user to be executed.</param>
        public void runCommand(ProcessorCommand command, bool single)
        {
            if (single)
            {
                switch (command.GetSingleCommandType())
                {
                    case SingleCommandType.ChangeDirectory:
                        CommandCD(command.GetArgs());
                        break;

                    case SingleCommandType.PrintCurrentDirectory:
                        CommandPrintWorkingDirectory(command.GetArgs());
                        break;

                    case SingleCommandType.ViewDirectory:
                        CommandDirectory(command.GetArgs());
                        break;

                    case SingleCommandType.Help:
                        CommandHelp(command.GetArgs());
                        break;

                    case SingleCommandType.Execute:
                        CommandExecute(command.GetArgs());
                        break;

                    case SingleCommandType.Exit:
                        CommandExit(command.GetArgs());
                        break;

                    default:
                        _executor.WriteOutputText($"\"{command.GetArgs().ElementAt(0)}\" command not yet supported.\n");
                        break;

                }
            }
            else
            {
                switch (command.GetMultiCommandType())
                {
                    case MultiCommandType.Test:
                        TestFunction(command);
                        break;
                }
            }
        }


        ///
        ///End Support Functions above
        ///
    }
}
