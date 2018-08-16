using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    public partial class MultiProcessCommand : ProcessorCommand
    {
        private MultiCommandType _commandType;
        private SingleProcessCommand _cmd1, _cmd2;

        public MultiProcessCommand(List<string> args1, List <string> args2, MultiCommandType multiCommandType, Dictionary<string, SingleProcessCommand> dict)
        {
            _commandType = multiCommandType;
            _cmd1 = new SingleProcessCommand(args1);
            _cmd2 = new SingleProcessCommand(args2);
        }
    }
}
