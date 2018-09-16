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
        public SingleProcessCommand(IEnumerable<string> args, SingleCommandType commandType) : base()
        {
            Args = args;
            sCommandType = commandType;
        }

        public override IEnumerable<string> GetArgs() //obsolete and to be deleted soon
        {
            return Args;
        }

        public override SingleCommandType GetSingleCommandType() //obsolete and to be deleted soon
        {
            return sCommandType;
        }

        public override MultiCommandType GetMultiCommandType()
        {
            throw new NotImplementedException();
        }
    }
}
