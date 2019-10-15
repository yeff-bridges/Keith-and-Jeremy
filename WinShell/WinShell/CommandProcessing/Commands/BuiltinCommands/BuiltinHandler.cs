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
    /// via method called by the switch structure. Each method returns an exit status, with 0 
    /// on success.
    /// </summary>
    public class BuiltinHandler : ICommandHandler
    {
        public int ExecuteCommand(CommandDescriptor descriptor, string[] args, CommandExecutor executor){

            int result;
            switch (descriptor.Name)
            {
                case "Help":
                    result = CommandHelp(descriptor, args, executor);
                    break;

                case "Cd":
                    result = CommandCD(descriptor, args, executor);
                    break;

                case "Exit":
                    result = CommandExit(descriptor, args, executor);
                    break;

                case "Pwd":
                    result = CommandPrintWorkingDirectory(descriptor, args, executor);
                    break;

                case "Dir":
                    result = CommandDirectory(descriptor, args, executor);
                    break;

                case "Exec":
                    result = CommandExecute(descriptor, args, executor);
                    break;

                case "Cls":
                    result = CommandClearScreen(descriptor, args, executor);
                    break;

                case "Echo":
                    result = CommandEcho(descriptor, args, executor);
                    break;

                default:
                    result = 1;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Prints this very useful help message to the window
        /// </summary>
        private int CommandHelp(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        {
            executor.WriteOutputText("We all need help, man.");
            return 0;
        }

        /// <summary>
        /// Changes the working directory (may wish to tweak if multiple windows are controlled by the same processor)
        /// The shortcut ~ -> %HOMEDRIVE%%HOMEPATH% is applied if the first part of the path is "~" or "~/".
        /// Does not support any optional arguments.
        /// </summary>
        private int CommandCD(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        {
            try
            {
                if (args.Length > 2) 
                {
                    executor.WriteInfoText($"cd: No additional arguments are supported for cd.");
                }
                else if (args.Length == 1)
                {
                    executor.WriteInfoText("cd: Must include a path as argument to cd.");
                }
                
                if (args[1].Length >= 1 && args[1][0] == '~') 
                {
                    if ((args[1].Length > 1 && (args[1][1] == '\\' || args[1][1] == '/') )|| args[1].Length==1)
                    {
                        string home = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                        Directory.SetCurrentDirectory(home);
                        if (args[1].Length > 1) 
                        {
                            args[1] = args[1].Substring(2);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                if (args[1].Length > 0) 
                {
                    Directory.SetCurrentDirectory(args[1]);
                }
                
            }
            catch (Exception ex)
            {
                executor.WriteInfoText($"Command failed: {ex.Message}\n");
                //alternate exit status to be determined later
            }

            return 0;
        }

        /// <summary>
        /// Clears the console output window.
        /// </summary>
        private int CommandClearScreen(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        { 
            executor.ClearOutput();
            return 0;
        }

        private int CommandEcho(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        {
            if (args.Length > 1) 
            {
                StringBuilder toEcho = new StringBuilder(args[1]);
                for (int i = 2; i < args.Length; i++)
                {
                    toEcho.Append(" ");
                    toEcho.Append(args[i]);
                }

                executor.WriteOutputText(toEcho.ToString());
            }
            else 
            {
                executor.WriteOutputText("");
            }

            return 0;
        }

        /// <summary>
        /// Exits the shell.
        /// </summary>
        private int CommandExit(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        {
            Environment.Exit(0); //This feels slow. The close API is another option
            return 0;
        }

        /// <summary>
        /// Prints the current working directory.
        /// </summary>
        private int CommandPrintWorkingDirectory(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        {
            executor.WriteOutputText($"{executor.GetCurrentWorkingDirectory()}");
            return 0;
        }

        /// <summary>
        /// Displays a directory listing.
        /// </summary>
        private int CommandDirectory(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        {
            try
            {
                var path = args.Count() >= 2 ? args.ElementAt(1) : executor.GetCurrentWorkingDirectory();
                IEnumerable<string> directories = Directory.EnumerateDirectories(path); //FLAG
                IEnumerable<string> files = Directory.EnumerateFiles(path);
                var targetDir = Path.GetFullPath(path);

                // Display the directory banner.
                executor.WriteOutputText("Directory of ");
                executor.WriteCommandLink($"{targetDir}\n", $"cd \"{targetDir}\"");

                // Display an output line for each directory in the listing.
                foreach (var directory in directories)
                {
                    var fullPath = Path.GetFullPath(directory);
                    var filespec = Path.GetFileName(fullPath);
                    var dirInfo = new DirectoryInfo(directory);
                    if (dirInfo.Exists)
                    {
                        var lastUpdated = dirInfo.LastWriteTime;
                        executor.WriteOutputText($"{lastUpdated.ToString("MM/dd/yyyy")}  {lastUpdated.ToString("hh:mm tt")}    ");
                        executor.WriteCommandLink($"<DIR>", $"dir \"{fullPath}\"");
                        executor.WriteOutputText("          ");
                        executor.WriteCommandLink($"{filespec}\n", $"cd \"{fullPath}\"");
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
                        executor.WriteOutputText($"{lastUpdated.ToString("MM/dd/yyyy")}  {lastUpdated.ToString("hh:mm tt")}    {length.ToString("##,###,###,###").PadLeft(14)} ");
                        executor.WriteCommandLink($"{filespec}\n", $"exec \"{fullPath}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                executor.WriteInfoText($"Command failed: {ex.Message}\n");
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Executes the specified command as a new process without checking to see if it's an internal shell command.
        /// </summary>
        private int CommandExecute(CommandDescriptor descriptor, string[] args, CommandExecutor executor)
        {
            try 
            {
                executor.Launch(args.Skip(1).ToArray());
                return 0;
            } 
            catch (Exception e)
            {
                executor.WriteInfoText($"Exec failed: {e.Message}\n");
            }

            return 1;
        }
    }
}

