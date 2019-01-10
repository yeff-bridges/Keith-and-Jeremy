using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    /// <summary>
    /// An exeception thrown when the parser encounters an invalid command.
    /// </summary>
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException() : base("An invalid command was parsed") { }
    }
}
