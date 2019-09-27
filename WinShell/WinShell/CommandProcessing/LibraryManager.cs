using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WinShell.CommandProcessing.Commands;
using WinShell.CommandProcessing.Commands.BuiltinCommands;

namespace WinShell
{
    /// <summary>
    /// The class which manages both initialization and interaction with the command library. 
    /// </summary>
    /// <notes>
    /// This class was implemented without much error handling, and a number of specific events
    /// are not properly handled. This mostly pertains to the loading of .dll files, and this
    /// will be fixed after the project is ported successfully to .Net Core
    /// </notes>
    public class LibraryManager
    {
        /// <summary>
        /// Gets and sets the initially empty set of CommandStrings used by the CommandParser to identify valid commands.
        /// </summary>
        public List<string> CommandStrings { get; private set; } = new List<string>();

        /// <summary>
        /// A string used to hold any error messages
        /// </summary>
        public string ErrorString { get; private set; }

        /// <summary>
        /// Gets and sets the CommandProcessor used by the library manager
        /// </summary>
        private CommandProcessor Processor;

        /// <summary>
        /// A dictionary associating elements of CommandStrings with ShellCommandEntries, for use in 
        /// </summary>
        private Dictionary<string, ShellCommandEntry> _commands = new Dictionary<string, ShellCommandEntry>();
        
        /// <summary>
        /// Upon construction, the LibraryManager simply stores the reference of its instantiating processor.
        /// </summary>
        /// <param name="processor"></param>
        public LibraryManager(CommandProcessor processor)
        {
            Processor = processor;
        }

        /// <summary>
        /// A function called by the CommandProcessor to run a valid command. While a check was put in place
        /// to never pass an invalid command, the unlikely case in which this does happen is still handled, and a
        /// unique error message is printed.
        /// </summary>
        /// <param name="argList"> Passed an array of arguments. </param>
        /// <returns> Return 0 if command ran successfullly, and a positive integer if something went wrong. 
        ///           In the case of a NullReferenceException, -1 is returned.                              </returns>
        public int runCommand(string[] args)
        {
            _commands.TryGetValue(args[0], out ShellCommandEntry command);
            try
            {
                return command.Handler.ExecuteCommand(command.Descriptor, args, Processor.Executor);
            }
            catch (NullReferenceException e)
            {
                Processor.Executor.WriteInfoText("Command parsed, but not recognized.");
                return 1;
            }
        }

        // This method may later be called upon construction, but for now is called by the CommandProcessor.
        // Some work needs to be done with this method to check for errors while loading extra commands.
        /// <summary>
        /// A method that calls the initialize functions of the builtin library and of any dlls found.
        /// The "MyDlls" key in the config file is store the file path of the directory full of .dll files.
        /// If "MyDlls" is "null", then only the builtin library is loaded, and no attempt is made to load any others.
        /// </summary>
        /// <returns> Returns an 0 if all libraries succesfully loaded, and 1 otherwise. </returns>
        public int initLibraries()
        {
            var builtin = new BuiltinLibrary(this);
            builtin.InitializeCommands();
            string key = ConfigurationManager.AppSettings.Get("MyDlls");
            if (key != "null")
            {
                try
                {
                    string[] DllNames = Directory.GetFiles(key);
                    Assembly Dll;
                    foreach (string filename in DllNames)
                    {
                        Dll = Assembly.LoadFrom($"{filename}");
                        Type[] types = Dll.GetExportedTypes();
                        foreach (Type t in types)
                        {
                            dynamic d = Activator.CreateInstance(t);
                            try
                            {
                                d.InitializeCommands(Processor);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorString = e.Message;
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Creates a ShellCommandEntry for each command in a given command library, and inserts the command's
        /// name into CommandStrings to make it a valid command according to the parser.
        /// </summary>
        /// <param name="commands"> An enumerable object containing a set of command descriptors. </param>
        /// <param name="handler"> A handler to be associated with the set of command descriptors. </param>
        /// <returns> Returns true on success and false if a command is rejected. </returns>
        public bool registerCommands(IEnumerable<CommandDescriptor> commands, ICommandHandler handler)
        {
            try
            {
                foreach (var command in commands)
                {
                    _commands.Add(
                        command.Name.ToLower(),
                        new ShellCommandEntry
                        {
                            Descriptor = command,
                            Handler = handler
                        }
                    );
                    CommandStrings.Add(command.Name);
                }

                return true;
            }
            catch (Exception e)
            {
                Processor.Executor.WriteInfoText(e.Message);
                return false;
            }
        }

        /// <summary>
        /// A Private class used to store command descriptors with their associated handlers.
        /// A dictionary of these, labled _commands, is used to associate each command with the string
        /// the client inputs to run it.
        /// </summary>
        class ShellCommandEntry
        {
            public CommandDescriptor Descriptor { get; set; }
            public ICommandHandler Handler { get; set; }
        }
    }
}
