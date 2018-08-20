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
        private CommandProcessor _processor;
        private BuiltinLibrary _lib;
        private bool _hasSymbol;

        public CommandParser(CommandProcessor processor)
        {
            _processor = processor;
            _lib = processor.Builtins;
        }

        public ProcessorCommand Parse(string command)
        {
            _hasSymbol = false;
            IEnumerable<string> args = GetArgs(command.ToLowerInvariant()); //this process will be handled differently by a seperate object in the future.

            //Symbols to be handled at a later time. This creates a blank SingleProcessCommand.
            if (_hasSymbol)
            {
                SingleProcessCommand pCommand = new SingleProcessCommand(args, SingleCommandType.NULL);
                _hasSymbol = false;
                return pCommand;
            }
            else //In the case of an actual SingleProcessCommand, create it using the standard means.
            {
                SingleProcessCommand pCommand = new SingleProcessCommand(args, _lib.GetSingleCommandDict()[args.ElementAt(0)]);
                return pCommand;
            }
        }

        /// <summary>
        /// Splits a string into tokens based on certain delimiters.
        /// If the string was split by anything other than white space, _hasSymbol is set.
        /// </summary>
        /// <param name="command">String to split into arguments.</param>
        /// <returns>A list of arguments to be processed.</returns>
        private IEnumerable<string> GetArgs(string command)
        {
            List<string> argv = new List<string>();
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
                        EasyAdd<string>(argv, token.ToString());
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
                if (_lib.GetMultiCommandDict().TryGetValue(c.ToString(), out MultiCommandType pCommandType))
                {
                    _hasSymbol = true;
                    EasyAdd<string>(argv, token.ToString());
                    EasyAdd<string>(argv, c.ToString());
                    token.Clear();
                }
                else if (c == ' ')
                {
                    EasyAdd<string>(argv, token.ToString());
                    token.Clear();
                }
                else
                {
                    token.Append(c);
                }
            }

            EasyAdd<string>(argv, token.ToString());
            return argv;
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
