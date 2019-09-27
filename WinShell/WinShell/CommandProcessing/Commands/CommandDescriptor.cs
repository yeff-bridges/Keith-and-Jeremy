using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell.CommandProcessing.Commands
{
    /// <summary>
    /// An object storing the Name and Description of a command to be run by the shell.
    /// The Name field is used by the corresponding ICommandHandler to know what command
    /// to run in ExecuteCommand, and the Description is used for help messages involving
    /// the command.
    /// </summary>
    public class CommandDescriptor
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
