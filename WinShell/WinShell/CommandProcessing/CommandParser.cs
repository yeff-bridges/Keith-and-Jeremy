using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WinShell
{   
    /// <summary>
    /// Object class for parsing a string and outputing a new command object.
    /// </summary>
    public class CommandParser
    {
        private List<string> _commandStrings;
        private CommandProcessor _processor;

        /// <summary>
        /// Constructor which stores the instantiating CommandProcessor's reference and
        /// a List<string> of valid command names determined _processor's corresponding LibraryManager.
        /// </summary>
        /// <param name="processor"></param>
        public CommandParser(CommandProcessor processor)
        {
            _processor = processor;
            _commandStrings = _processor.LibManager.CommandStrings;
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
            bool withinQuotes = false, validCommand = false;

            foreach (char c in command)
            {
                if (validCommand)
                {
                    // Process quoted strings first. The string (with quotes removed) will be
                    // treated as a single token.
                    // If we're currently processing a quoted string...
                    if (withinQuotes)
                    {
                        // If we've encountered our closing quote, add the token to our arg list and prepare to start a new token.
                        if (c == '"')
                        {
                            EasyAdd(args, token.ToString());
                            token.Clear();
                            withinQuotes = false;
                            break;
                        }
                        else
                        {
                            // Else add the character to the token.
                            token.Append(c);
                            continue;
                        }
                    }
                    else if (c == '"')
                    {
                        // If we've encountered an opening quote, note it and skip to the next character.
                        withinQuotes = true;
                        continue;
                    }

                    //If we're currently processing a non-quoted string...
                    if (c == ' ')
                    { 
                        EasyAdd(args, token.ToString());
                        token.Clear();
                    }
                    else
                    {
                        token.Append(c);
                    }
                }
                //We have not confirmed that the command we've encountered is valid
                //No argument in quotes will be valid
                else
                {
                    if (c == ' ')
                    {
                        validCommand = ValidateCommand(token.ToString());
                        args.Add(token.ToString());
                        token.Clear();
                    }
                    else
                    {
                        token.Append(c);
                    }
                }
            }
            EasyAdd(args, token.ToString());

            if (!validCommand)
            {
                ValidateCommand(args.ElementAt(0));
            }

            return args;
        }   

        /// <summary>
        /// Helper function to assist storing tokens in parse. Written to be general for
        /// the sake of progressing science ;)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"> A list to have an item inserted. </param>
        /// <param name="item"> An item to be inserted in the above list. </param>
        private void EasyAdd<T>(List<T> list, T item)
        {
            if (item != null)
            {
                if (!(item.GetType() == typeof(string)) || !String.IsNullOrWhiteSpace(item as string))
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Helper function used by parse to check if the first token of the command string
        /// (first token referenced by t) shows up in the list of possible commands.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool ValidateCommand(string t)
        {
            bool valid = false;
            foreach (string s in _commandStrings)
            {
                if (t.ToLowerInvariant() == s.ToLowerInvariant())
                {
                    valid = true;
                    return valid;
                }
            }
            //If the token matches none of the known commands, then no command can be made
            if (!valid)
            {
                throw new InvalidCommandException();
            }

            return false;
        }
    }
}
