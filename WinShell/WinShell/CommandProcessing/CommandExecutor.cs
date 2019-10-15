using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinShell.UIManagement;
using System.Diagnostics;


// Continue comments here
namespace WinShell
{
    /// <summary>
    /// Object Class that is used for executing valid commands. Handles interactions with the window for other 
    /// backend objects such as the CommandParser, LibraryManager, and libraries.
    /// </summary>
    public class CommandExecutor
    {
        private CommandProcessor _processor;
        private ConsoleWindow _outputWindow;

        /// <summary>
        /// Constructor for the CommandExecutor which simply stores the reference to the
        /// processor it belongs to. The _outputWindow field is left uninitialized to force
        /// methods to always update it first. When multiple windows are finally supported,
        /// the method in which the CommandExecutor figures out which window to use may
        /// be more complicated. 
        /// </summary>
        public CommandExecutor(CommandProcessor processor)
        {
            _processor = processor;
        }

        /// <summary>
        /// Clears the command output area.
        /// </summary>
        public void ClearOutput()
        {
            _outputWindow = _processor.Window;
            _outputWindow.ClearOutput();
        }

        /// <summary>
        /// Gets the path to the current working directory.
        /// </summary>
        public string GetCurrentWorkingDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Creates a process specified by the list of arguments passed. 
        /// If possible, process output is redirected to _outputWindow
        /// Exceptions must be handled by the caller.
        /// <param name="args">Set of arguments, the first being the program to launch.</param>
        /// <returns>A value indicating whether we were able to successfully launch a new process.</returns>
        public bool Launch(string[] args)
        {
            StringBuilder argsString = new StringBuilder();
            args.Skip(1).ToList().ForEach(s => argsString.Append($"{s} "));

            var startInfo = new ProcessStartInfo
            {
                FileName = args.ElementAt(0),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                Arguments = argsString.ToString().TrimEnd(' ')
            };

            var process = Process.Start(startInfo);

            // Handle redirected standard input and output I/O on via a background task.
            Task.Run(() =>
            {
                var wasInputRedirected = EnableInputRedirection(true, process.StandardInput);

                var output = new char[1024];
                int stdinCharCount = 0;
                do
                {
                    stdinCharCount = process.StandardOutput.Read(output, 0, output.Length);
                    if (stdinCharCount > 0)
                    {
                        WriteOutputText(new string(output, 0, stdinCharCount));
                    }
                } while (stdinCharCount > 0);

                EnableInputRedirection(wasInputRedirected, null);
            });

            // Handle redirected standard-error I/O on via a background task.
            Task.Run(() =>
            {
                var output = new char[1024];
                int stdinErrorCount;
                do
                {
                    stdinErrorCount = process.StandardError.Read(output, 0, output.Length);
                    if (stdinErrorCount > 0)
                    {
                        WriteOutputText(new string(output, 0, stdinErrorCount));
                    }
                } while (stdinErrorCount > 0);
            });

            return true;
        }

        /// <summary>
        /// Method called by LibraryManager to try launching the first element of args
        /// in the case that it doesn't match a command name or alias. Libraries are welcome
        /// to use this, though there will likely be no reason too.
        /// </summary>
        public int TryLaunch(string[] args) 
        {
            try
            {
                Launch(args);
                return 0;
            }
            catch (Exception e)
            {
                if (e.Message.Equals("The system cannot find the file specified"))
                {
                    WriteInfoText($"The term '{args[0]}' did not match any registered commands or valid executable paths\n");
                }
                else
                {
                    WriteInfoText($"Command failed: {e.Message}\n");
                }
            }
            
            return 1;
        }

        /// <summary>
        /// Enables or disables input redirection for the corresponding shell session.
        /// </summary>
        /// <param name="enabled">A value indicating whether the enable input redirection.</param>
        /// <param name="targetStream">The stream to use for writing standard-input characters, if enabled is true.</param>
        /// <returns>The previous input-redirection-enable value.</returns>
        public bool EnableInputRedirection(bool enabled, StreamWriter targetStream)
        {
            return _processor.Window.EnableInputRedirection(enabled, targetStream);
        }

        /// <summary>
        /// Writes a string of informational (non-command output) to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteInfoText(string outputText)
        {
            _outputWindow = _processor.Window;
            _outputWindow.WriteInfoText(outputText);
        }

        /// <summary>
        /// Writes a string of command output text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteOutputText(string outputText)
        {
            _outputWindow = _processor.Window;
            _outputWindow.WriteOutputText(outputText);
        }

        /// <summary>
        /// Writes a command hyperlink to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        /// <param name="command">Command line to associate with the hyperlink.</param>
        public void WriteCommandLink(string outputText, string command)
        {
            _outputWindow = _processor.Window;
            _outputWindow.WriteCommandLink(outputText, _outputWindow.RunShellRequestCommand, command);
        }
    }
}
