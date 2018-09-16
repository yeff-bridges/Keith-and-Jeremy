using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinShell;

namespace WinShell
{
    /// <summary>
    /// Class for creating command objects that use multiple processes.
    /// Made from multiple SingleProcessCommands joined by a single Multi command during execution
    /// </summary>
    public class MultiProcessCommand : ProcessorCommand
    { 
        public MultiProcessCommand(List<string> args1, List <string> args2, MultiCommandType multiCommandType, Dictionary<string, SingleCommandType> dict) : base()
        {
            mCommandType = multiCommandType;
            Cmd1 = new SingleProcessCommand(args1, dict[args1.ElementAt(0)]);
            Cmd2 = new SingleProcessCommand(args2, dict[args2.ElementAt(0)]);
        }

        public override IEnumerable<string> GetArgs()
        {
            throw new NotImplementedException();
        }

        public override SingleCommandType GetSingleCommandType() //to be removed soon
        {
            throw new NotImplementedException();
        }

        public override MultiCommandType GetMultiCommandType() //ditto
        {
            return mCommandType;
        }
    }
}
