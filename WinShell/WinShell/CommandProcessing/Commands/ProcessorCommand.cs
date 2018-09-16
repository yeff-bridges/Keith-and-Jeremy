using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinShell.UIManagement;

namespace WinShell
{
    /// <summary>
    /// A class representing a generic "Command Processor" command.
    /// Concrete command classes must implement the bottom three methods.
    /// </summary>
    public abstract class ProcessorCommand
    {

        public IEnumerable<string> Args { get; protected set; }
        public SingleCommandType sCommandType { get; protected set; }
        public MultiCommandType mCommandType { get; protected set; }
        public SingleProcessCommand Cmd1 { get; protected set; }
        public SingleProcessCommand Cmd2 { get; protected set; }

        abstract public IEnumerable<string> GetArgs();
        abstract public SingleCommandType GetSingleCommandType();
        abstract public MultiCommandType GetMultiCommandType();

        /// <summary>
        /// Gets or sets the console window to use for user input and output.
        /// </summary>
        public ConsoleWindow ConsoleWindow { get; set; }
    }
}
