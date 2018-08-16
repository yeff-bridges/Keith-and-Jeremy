using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{

public partial class SingleProcessCommand : ProcessorCommand
    {
        private List<string> _args;
        private SingleCommandType _commandType;

        public SingleProcessCommand(List<string> args, MainWindow window, Dictionary<string, SingleCommandType> dict)
        {
            //Super();
            _commandType = dict[args.ElementAt(0)];
        }
    }
}
