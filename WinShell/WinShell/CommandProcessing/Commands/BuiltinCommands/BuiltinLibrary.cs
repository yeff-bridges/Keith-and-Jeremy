using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell.CommandProcessing.Commands.BuiltinCommands
{
    /// <summary>
    /// The current library of builtin commands loaded whenever the shell initializes.
    /// These commands are not dependant on the device they run on, and will be the
    /// only set of commands should the "MyDlls" option be "null".
    /// </summary>
    public class BuiltinLibrary : ICommandLibrary
    {
        // May make these fields necessary in the ICommandLibrary interface
        private LibraryManager _libManager;
        private static CommandDescriptor[] _commands = new CommandDescriptor[]
        {
            new CommandDescriptor { Name = "Help", Description = "Prints currently useless help message" },
            new CommandDescriptor { Name = "Cd", Description = "Changes directory. Syntax: cd [absolute/relative path to directory]" },
            new CommandDescriptor { Name = "Dir", Description = "Lists stat call of all directories or files in current directory." },
            new CommandDescriptor { Name = "Pwd", Description = "Prints path to current working directory." },
            new CommandDescriptor { Name = "Echo", Description = "Prints to the console the parsed arguments that follow this command, if any."},
            new CommandDescriptor { Name = "Exec", Description = "Used to execute a file named identically to a command. Syntax: Exec [filename] [args...]" },
            new CommandDescriptor { Name = "Exit", Description = "Properly close current shell session." },
            new CommandDescriptor { Name = "Cls", Description = "Clear shell display of recent input." },
        };

        // Will likely make this contructor part of ICommandLibrary in the future.
        public BuiltinLibrary(LibraryManager libManager)
        {
            _libManager = libManager;
        }

        // Will likely make a default version of this method in the interface
        /// <summary>
        /// Associates all commands in the BuiltinLibrary with the BuiltinHandler instantiated.
        /// </summary>
        /// <returns>True if commands successfully registered, false if otherwise</returns>
        public bool InitializeCommands()
        {
            var handler = new BuiltinHandler();
            return _libManager.registerCommands(_commands, handler);
        }
    }
}
