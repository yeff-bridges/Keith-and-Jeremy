using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WinShell
{   
    /// <summary>
    /// Object class for parsing a string and outputing a new command object. All lower level logic for how the 
    /// Shell will actually read, fill, and break apart the strings that it is given will be handled here.
    /// </summary>
    public class CommandParser
    {
        private CommandExecutor _executor;

        /// <summary>
        /// Constructor which stores the instantiating CommandProcessor's reference and
        /// a List<string> of valid command names determined _processor's corresponding LibraryManager.
        /// </summary>
        /// <param name="processor"></param>
        public CommandParser(CommandExecutor executor)
        {
            _executor = executor;
        }

        //  TO BE OVERHAULED SOON (see specs.txt on Git)
        /// <summary>
        /// Parses the user's string and returns a list with the command followed by
        /// the user's arguments for it. If the command string did not represent a valid
        /// command, an InvalidCommandException is thrown instead. Case-sensitive and supports quotes.
        /// </summary>
        /// <param name="command"> The string input into the window by the user. </param>
        /// <returns> Returns a list containing a command string followed by
        ///           any remaining arguments.                              </returns>
        public List<string> Parse(string command)
        { 
            List<string> args = new List<string>();
            StringBuilder token = new StringBuilder();
            bool withinDoubQuotes = false, withinSingQuotes = false;

            foreach (char c in command)
            {
                switch (c)
                {
                    case '\"' when withinDoubQuotes:
                        withinDoubQuotes = false;
                        break;
                    case '\"' when !withinSingQuotes:
                        withinDoubQuotes = true;
                        break;
                    case '\'' when withinSingQuotes:
                        withinSingQuotes = false;
                        break;
                    case '\'' when !withinDoubQuotes:
                        withinSingQuotes = true;
                        break;
                    case ' ' when !withinDoubQuotes && !withinSingQuotes:
                        EasyAdd(args, token.ToString());
                        token.Clear();
                        break;
                    default:
                        token.Append(c);
                        break;
                }
            }

            if (withinDoubQuotes || withinSingQuotes)
            {
                string quote = withinSingQuotes ? "single" : "double";
                _executor.WriteInfoText($"The input entered had an unclosed {quote} quote. Command Aborted.\n");
                return new List<string>();
            }

            EasyAdd(args, token.ToString());

            return args;
        }   

        /// <summary>
        /// Helper function to assist storing tokens in parse without storing empty strings.
        /// </summary>
        private void EasyAdd(List<string> list, string str)
        { 
            if (!string.IsNullOrWhiteSpace(str))
            {
                list.Add(str);
            }
        }
    }
}
