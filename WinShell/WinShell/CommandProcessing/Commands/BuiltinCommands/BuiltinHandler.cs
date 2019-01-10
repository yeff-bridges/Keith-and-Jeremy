using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell.CommandProcessing.Commands.BuiltinCommands
{
    /// <summary>
    /// Handler Object for the BuiltinLibrary. The ExecuteCommand method may be called by the
    /// LibraryManager object of the CommandProcessor to run any command in the BuiltinLibrary
    /// via method called by the switch structure.
    /// </summary>
    public class BuiltinHandler : ICommandHandler
    {
        public int ExecuteCommand(CommandDescriptor descriptor, string[] args, CommandProcessor processor){

            int result;
            switch (descriptor.Name)
            {
                case "Help":
                    result = CommandHelp(descriptor, args, processor);
                    break;

                case "Cd":
                    result = CommandCD(descriptor, args, processor);
                    break;

                case "Exit":
                    result = CommandExit(descriptor, args, processor);
                    break;

                case "Pwd":
                    result = CommandPrintWorkingDirectory(descriptor, args, processor);
                    break;

                case "Dir":
                    result = CommandDirectory(descriptor, args, processor);
                    break;

                case "Exec":
                    result = CommandExecute(descriptor, args, processor);
                    break;

                case "Cls":
                    result = CommandClearScreen(descriptor, args, processor);
                    break;

                default:
                    result = 1;
                    break;
            }

            return result;
        }

        private int CommandHelp(CommandDescriptor descriptor, string[] args, CommandProcessor processor)
        {
            processor.Executor.WriteOutputText("We all need help, man.");
            return 0;
        }

        private int CommandCD(CommandDescriptor descriptor, string[] args, CommandProcessor processor)
        {
            try
            {
                Directory.SetCurrentDirectory(args[1]);
            }
            catch (Exception ex)
            {
                processor.Executor.WriteInfoText($"Command failed: {ex.Message}\n");
                //alternate exit status to be determined later
            }

            return 0;
        }

        /// <summary>
        /// Clears the console output window.
        /// </summary>
        /// <param name="args">Arguments for the command.</param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        private int CommandClearScreen(CommandDescriptor descriptor, string[] args, CommandProcessor processor)
        { 
            processor.Executor.ClearOutput();
            return 0;
        }

        /// <summary>
        /// Exits the shell.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        private int CommandExit(CommandDescriptor descriptor, string[] args, CommandProcessor processor)
        {
            Environment.Exit(0); //This feels slow. The close API is another option
            return 0;
        }

        /// <summary>
        /// Prints the current working directory.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Returns an integer representing the exit status of the operation.</returns>
        private int CommandPrintWorkingDirectory(CommandDescriptor descriptor, string[] args, CommandProcessor processor)
        {
            processor.Executor.WriteOutputText($"{processor.Executor.GetCurrentWorkingDirectory()}");
            return 0;
        }

        /// <summary>
        /// Displays a directory listing.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>An integer representing the exit status of the operation.</returns>
        private int CommandDirectory(CommandDescriptor descriptor, string[] args, CommandProcessor processor)
        {
            try
            {
                var path = args.Count() >= 2 ? args.ElementAt(1) : processor.Executor.GetCurrentWorkingDirectory();
                IEnumerable<string> directories = Directory.EnumerateDirectories(path); //FLAG
                IEnumerable<string> files = Directory.EnumerateFiles(path);
                var targetDir = Path.GetFullPath(path);

                // Display the directory banner.
                processor.Executor.WriteOutputText("Directory of ");
                processor.Executor.WriteCommandLink($"{targetDir}\n", $"cd \"{targetDir}\"");

                // Display an output line for each directory in the listing.
                foreach (var directory in directories)
                {
                    var fullPath = Path.GetFullPath(directory);
                    var filespec = Path.GetFileName(fullPath);
                    var dirInfo = new DirectoryInfo(directory);
                    if (dirInfo.Exists)
                    {
                        var lastUpdated = dirInfo.LastWriteTime;
                        processor.Executor.WriteOutputText($"{lastUpdated.ToString("MM/dd/yyyy")}  {lastUpdated.ToString("hh:mm tt")}    ");
                        processor.Executor.WriteCommandLink($"<DIR>", $"dir \"{fullPath}\"");
                        processor.Executor.WriteOutputText("          ");
                        processor.Executor.WriteCommandLink($"{filespec}\n", $"cd \"{fullPath}\"");
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
                        processor.Executor.WriteOutputText($"{lastUpdated.ToString("MM/dd/yyyy")}  {lastUpdated.ToString("hh:mm tt")}    {length.ToString("##,###,###,###").PadLeft(14)} ");
                        processor.Executor.WriteCommandLink($"{filespec}\n", $"exec \"{fullPath}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                processor.Executor.WriteInfoText($"Command failed: {ex.Message}\n");
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Executes the specified command as a new process without checking to see if it's an internal shell command.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>An integer representing the exit status of the operation.</returns>
        private int CommandExecute(CommandDescriptor descriptor, string[] args, CommandProcessor processor)
        {
            processor.Executor.Launch(args.Skip(1).ToArray());

            return 0;
        }
    }
}

