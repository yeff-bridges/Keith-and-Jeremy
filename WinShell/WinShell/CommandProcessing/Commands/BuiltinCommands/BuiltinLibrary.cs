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
        private static CommandDescriptor[] _commands = new CommandDescriptor[]
        {
            new CommandDescriptor { Name = "Help", Description = "Prints useless help message" },
            new CommandDescriptor { Name = "Cd", Description = "Prints useless help message" },
            new CommandDescriptor { Name = "Dir", Description = "Prints useless help message" },
            new CommandDescriptor { Name = "Pwd", Description = "Prints useless help message" },
            new CommandDescriptor { Name = "Exec", Description = "Prints useless help message" },
            new CommandDescriptor { Name = "Exit", Description = "Prints useless help message" },
            new CommandDescriptor { Name = "Cls", Description = "Prints useless help message" },
        };

        public bool InitializeCommands(CommandProcessor processor)
        {
            var handler = new BuiltinHandler();
            return processor.LibManager.registerCommands(_commands, handler);
        }
    }
}
