using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell.CommandProcessing.Commands
{
    /// <summary>
    /// An interface used by the handler object of a shell command library. 
    /// Must implement the ExecuteCommand method, which typically uses the given
    /// CommandDescriptor to decide on a method to call.
    /// </summary>
    public interface ICommandHandler
    {
        int ExecuteCommand(CommandDescriptor descriptor, string[] args, CommandProcessor processor);
    }
}
