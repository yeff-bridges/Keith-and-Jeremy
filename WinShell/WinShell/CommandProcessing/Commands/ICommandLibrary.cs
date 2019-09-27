using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell.CommandProcessing.Commands
{
    /// <summary>
    /// An interface implemented by library object in a dll file of this shell.
    /// Must implement its own initialization method.
    /// Additional required fields and a constructor may be added in the future. See BuiltinLibrary.
    /// </summary>
    public interface ICommandLibrary
    {
        bool InitializeCommands();
    }
}
