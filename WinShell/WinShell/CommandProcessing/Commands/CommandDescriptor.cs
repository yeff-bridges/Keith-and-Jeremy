using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell.CommandProcessing.Commands
{
    /// <summary>
    /// An object storying the Name and Description of a command a shell command.
    /// This object is also used by the handler to decide how to execute a command 
    /// with the given descriptor, and by the Library Manager to associate it with
    /// the proper handler class.
    /// </summary>
    public class CommandDescriptor
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
