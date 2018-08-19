using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinShell
{
    /// <summary>
    /// A class representing a generic "Command Processor" command.
    /// Concrete command classes must implement the bottom three methods.
    /// </summary>
    public abstract class ProcessorCommand
    {
        abstract public IEnumerable<string> GetArgs();
        abstract public SingleCommandType GetSingleCommandType();
        abstract public MultiCommandType GetMultiCommandType();
    }
}
