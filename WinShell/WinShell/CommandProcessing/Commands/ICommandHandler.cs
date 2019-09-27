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
        /// <summary>
        /// Given any associated CommandDescriptor, a list of args, and an associated CommandProcessor,
        /// this method should take the proper action in executing the command. This may include calling a 
        /// method designated to handle the command and/or throwing an InvalidArgumentsException if the args
        /// provided lead to a problem. When passed a CommandDescriptor not associated with this Handler, an
        /// error code of 1 should be returned. This, however, should never happen with the current LibraryManager.
        /// May use the CommandExecutor of processor to accomplish tasks involving the window.
        /// </summary>
        /// <param name="descriptor">CommandDescriptor for the command being executed</param>
        /// <param name="args">List of args for the command being executed</param>
        /// <param name="processor">The processor associated with this ICommandHandler's LibraryManager</param>
        /// <returns></returns>
        int ExecuteCommand(CommandDescriptor descriptor, string[] args, CommandExecutor executor);
    }
}
