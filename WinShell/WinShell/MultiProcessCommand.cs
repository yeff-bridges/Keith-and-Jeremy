using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    /// <summary>
    /// Class for creating command objects that use multiple processes.
    /// Made from multiple SingleProcessCommands joined by a single Multi command during execution
    /// </summary>
    public class MultiProcessCommand : ProcessorCommand
    {
        private List<string> _args1, _args2;
        private MultiCommandType _commandType;
        private SingleProcessCommand _cmd1, _cmd2;

        public MultiProcessCommand(List<string> args1, List <string> args2, MultiCommandType multiCommandType, Dictionary<string, SingleCommandType> dict) : base()
        {
            _args1 = args1;
            _args2 = args2;
            _commandType = multiCommandType;
            _cmd1 = new SingleProcessCommand(args1, dict[_args1.ElementAt(0)]);
            _cmd2 = new SingleProcessCommand(args2, dict[_args2.ElementAt(0)]);
        }

        public override IEnumerable<string> GetArgs()
        {
            throw new NotImplementedException();
        }

        public override SingleCommandType GetSingleCommandType()
        {
            throw new NotImplementedException();
        }

        public override MultiCommandType GetMultiCommandType()
        {
            return _commandType;
        }
    }
}
