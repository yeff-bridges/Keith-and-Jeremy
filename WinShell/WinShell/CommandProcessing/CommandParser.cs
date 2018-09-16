using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WinShell
{
    /// <summary>
    /// Class for parsing a string and outputing a new command object.
    /// </summary>
    public class CommandParser
    {
        public CommandProcessor _processor { get; private set; }
        public BuiltinLibrary _lib { get; private set; }

        private bool _hasSymbol; //A boolean used for telling whether a command was split with symbols

        public CommandParser(CommandProcessor processor)
        {
            _processor = processor;
            _lib = processor.Builtins;
        }

        public ProcessorCommand Parse(string command)
        {
            _hasSymbol = false;
            (List<string> args1, List<string> args2, MultiCommandType multiType) = GetArgs(command.ToLowerInvariant()); //this process will be handled differently by a seperate object in the future.

            //Symbols to be handled at a later time. This creates a blank SingleProcessCommand.
            if (!multiType.Equals(MultiCommandType.NULL))
            {
                MultiProcessCommand pCommand = new MultiProcessCommand(args1, args2, multiType, _lib.SingleBuiltins);
                _hasSymbol = false;
                return pCommand;
            }
            else //In the case of an actual SingleProcessCommand, create it using the standard means.
            {
                SingleProcessCommand pCommand = new SingleProcessCommand(args1, _lib.SingleBuiltins[args1.ElementAt(0)]);
                return pCommand;
            }
        }

        /// <summary>
        /// Splits a string into tokens based on certain delimiters.
        /// If the string was split by anything other than white space, _hasSymbol is set.
        /// </summary>
        /// <param name="command">String to split into arguments.</param>
        /// <returns>A list of arguments to be processed.</returns>
        private (List<string>, List<string>, MultiCommandType) GetArgs(string command)
        {
            List<string> args1 = new List<string>();
            List<string> args2 = new List<string>();
            MultiCommandType multiType = MultiCommandType.NULL;
            StringBuilder token = new StringBuilder();
            _hasSymbol = false;
            bool withinQuotes = false;

            foreach (char c in command)
            {
                // Process quoted strings first. The string (with quotes removed) will be
                // treated as a single token.
                // If we're currently processing a quoted string...
                if (withinQuotes)
                {
                    // If we've encountered our closing quote, add the token to our arg list and prepare to start a new token.
                    if (c == '"')
                    {
                        EasyAdd<string>(args1, token.ToString());
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
                if (_lib.MultiBuiltins.TryGetValue(c.ToString(), out MultiCommandType pCommandType))
                {
                    _hasSymbol = true;
                    EasyAdd<string>(args1, token.ToString());
                    EasyAdd<string>(args1, c.ToString());
                    token.Clear();
                }
                else if (c == ' ')
                {
                    EasyAdd<string>(args1, token.ToString());
                    token.Clear();
                }
                else
                {
                    token.Append(c);
                }
            }

            EasyAdd<string>(args1, token.ToString());
            return (args1, args2, multiType);
        }

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
    }
}
