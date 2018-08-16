using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinShell
{
    public enum SingleCommandType
    {
        //Directory
        ChangeDirectory,
        PrintCurrentDirectory,
        ListFilesInCurrentDirectory,

        //File Operations
        CreateFile,
        MoveFile,
        CopyFile,
        DeleteFile,

        //Misc
        Launch,
        ChMod,
        ChOwn
    };

    public enum MultiCommandType
    {
        Pipe,
        CondMultiLaunch,
        UncoMultiLaunch,
    };

    class CommandParser
    {
        private Dictionary<string, SingleCommandType> _singleBuiltins;
        private Dictionary<string, MultiCommandType> _multiBuiltins;

        public void GetCommandType()
        {
            var dict = new Dictionary<string, SingleCommandType> {
                { "cd", SingleCommandType.ChangeDirectory },
            };
        }
    }
}
