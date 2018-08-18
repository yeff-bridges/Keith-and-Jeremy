using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    /// <summary>
    /// Class for creating commands that use a single process.
    /// </summary>
    public class SingleProcessCommand : ProcessorCommand
    {
        private IEnumerable<string> _args;
        private SingleCommandType _commandType;

        public SingleProcessCommand(IEnumerable<string> args, SingleCommandType commandType) : base()
        {
            _args = args;
            _commandType = commandType;
        }

        public override IEnumerable<string> GetArgs()
        {
            return _args;
        }

        public override SingleCommandType GetSingleCommandType()
        {
            return _commandType;
        }

        public override MultiCommandType GetMultiCommandType()
        {
            throw new NotImplementedException();
        }
    }
}
