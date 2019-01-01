using CSRegisterHotkey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinShell.UIManagement;

namespace WinShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Event for notifying listeners of property-changed events.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners of property changes.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        /// <remarks>
        /// This method is called by the Set accessor of each property.
        /// The CallerMemberName attribute that is applied to the optional propertyName
        /// parameter causes the property name of the caller to be substituted as an argument.
        /// </remarks>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets or sets the path to the current working directory.
        /// </summary>
        public string CurrentWorkingDirectory
        {
            get
            {
                return _currentWorkingDirectory;
            }

            private set
            {
                if (_currentWorkingDirectory != value)
                {
                    _currentWorkingDirectory = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // The path to the current working directory.
        private string _currentWorkingDirectory;

        private List<string> CommandHistory { get; set; } = new List<string>();

        private int IndexOfLastHistoryDisplayed { get; set; }

        private CommandProcessor Processor { get; set; }

        /// <summary>
        /// Gets the UI manager instance associated with the main window.
        /// </summary>
        private UIManager UIManager { get; }

        /// <summary>
        /// Our registered hotkey for showing/hiding the command line.
        /// </summary>
        private HotKeyRegister _hotKey = null;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            UIManager = new UIManager(this);
            Processor = new CommandProcessor(UIManager);
            UIManager.SetupInitialShellSession(Processor);
            PresentCommandPrompt(UIManager.DefaultShellSession);
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Handles the "Loaded" event for our main window, letting us know when it is "open for business".
        /// </summary>
        /// <param name="sender">The object trigger this event.</param>
        /// <param name="e">Arguments associated with this event.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Register our hotkey. This needs to happen after our main window is loaded.
            RegisterCmdLineHotkey();
        }

        /// <summary>
        /// Initializes the command prompt for use with the next command.
        /// </summary>
        /// <param name="shellSession">The shell session to use with the command prompt.</param>
        public void PresentCommandPrompt(ShellSession shellSession)
        {
            CurrentWorkingDirectory = shellSession.CurrentDirectory;
            IndexOfLastHistoryDisplayed = -1;
            txtCommand.Text = string.Empty;
        }

        /// <summary>
        /// Registers our application hotkey.
        /// </summary>
        private void RegisterCmdLineHotkey()
        {
            try
            {
                // Get the window handle for our main window.
                var hwndMainWindow = new System.Windows.Interop.WindowInteropHelper(this).Handle;

                // Register the hotkey Ctrl-Alt-W.
                _hotKey = new HotKeyRegister(
                    hwndMainWindow,
                    100,
                    KeyModifiers.Alt | KeyModifiers.Control,
                    System.Windows.Forms.Keys.W
                );

                // Listen for "hot key pressed" events.
                _hotKey.HotKeyPressed += new EventHandler(HotKeyPressed);
            }
            catch
            {
                // Ignore the failure to register our hotkey.
            }
        }

        /// <summary>
        /// Handle presses of our hotkey.
        /// </summary>
        /// <param name="sender">The object trigger this event.</param>
        /// <param name="e">Arguments associated with this event.</param>
        private void HotKeyPressed(object sender, EventArgs e)
        {
            // If we're already active...
            if (IsActive)
            {
                // If we're not minimized, minimize us now and return.
                if (WindowState != WindowState.Minimized)
                {
                    WindowState = WindowState.Minimized;
                    return;
                }
            }

            // Restore our window if minimized.
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            // Activate our window.
            this.Activate();
        }

        /// <summary>
        /// Handle the "closed" message for our main window.
        /// </summary>
        /// <param name="e">Arguments associated with this event.</param>
        protected override void OnClosed(EventArgs e)
        {
            // Unregister our hotkey if we previously succeeded in registering one.
            if (_hotKey != null)
            {
                _hotKey.Dispose();
                _hotKey = null;
            }

            base.OnClosed(e);
        }

        /// <summary>
        /// Handles "click" events for the "Current Directory" button.
        /// </summary>
        /// <param name="sender">The object trigger this event.</param>
        /// <param name="e">Arguments associated with this event.</param>
        private void btnOpenCurrentDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", CurrentWorkingDirectory);
        }

        /// <summary>
        /// Cycles through the command history, setting the command line text to the selected history entry.
        /// </summary>
        /// <param name="historyOffset">The offset by which to navigate through the command history. This is usually either 1 or -1, to select
        /// either the previous or next entry.</param>
        private void ShowCommandFromHistory(int historyOffset)
        {
            // If we have any commands in our history...
            if (CommandHistory.Count > 0)
            {
                int historyIndex;

                // If we've not already displayed a history entry while entering the current command...
                if (IndexOfLastHistoryDisplayed == -1)
                {
                    // Use the most recent history entry if we're advancing in a negative direction.
                    if (historyOffset < 0)
                    {
                        historyIndex = CommandHistory.Count() - 1;
                    }
                    else
                    {
                        // Else use the oldest history entry.
                        historyIndex = 0;
                    }
                }
                else
                {
                    // Else advance to another history entry, and then wrap around if necessary.
                    historyIndex = IndexOfLastHistoryDisplayed + historyOffset;
                    if (historyIndex < 0)
                    {
                        historyIndex = CommandHistory.Count() - 1;
                    }
                    else if (historyIndex >= CommandHistory.Count)
                    {
                        historyIndex = 0;
                    }
                }

                // Update the command input control to contain the history entry.
                txtCommand.Text = CommandHistory[historyIndex];
                txtCommand.CaretIndex = txtCommand.Text.Length;
                IndexOfLastHistoryDisplayed = historyIndex;
            }
        }

        /// <summary>
        /// Handles the "preview key down" event for the command line edit control.
        /// </summary>
        /// <param name="sender">The object triggering this event.</param>
        /// <param name="e">Arguments associated with this event.</param>
        private void txtCommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!UIManager.DefaultShellSession.UiRedirectionActive)
            {
                switch (e.Key)
                {
                    // If the Enter key was pressed, attempt to parse and execute the command specified in the command line edit control.
                    case Key.Enter:
                        var command = txtCommand.Text;
                        CommandHistory.Add(command);
                        UIManager.DefaultShellSession.ProcessCommand(command);
                        UIManager.PresentCommandPrompt();
                        e.Handled = true;
                        break;

                    // If the Esc key was pressed, discard the contents of the command line edit control and wait for a new command.
                    case Key.Escape:
                        UIManager.PresentCommandPrompt();
                        e.Handled = true;
                        break;

                    // If the Up arrow was pressed, replace the contents of the command line edit control with the previous entry in the command history.
                    case Key.Up:
                        ShowCommandFromHistory(-1);
                        e.Handled = true;
                        break;

                    // If the Down arrow was pressed, replace the contents of the command line edit control with the next entry in the command history.
                    case Key.Down:
                        ShowCommandFromHistory(1);
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                var kc = new KeyConverter();
                var chars = GetKeyString(e.Key, e.KeyboardDevice.Modifiers);
                if (chars.Length > 0)
                {
                    UIManager.DefaultShellSession.UiTargetStandardIOStream.Write(chars[0]);
                }
                e.Handled = true;
            }
        }
        public static string GetKeyString(Key key, ModifierKeys modifiers)
        {
            string keyStr = string.Empty;
            if (key != Key.None)
            {
                // Setup modifiers
                //if (modifiers.HasFlag(ModifierKeys.Control))
                //    result += "Ctrl + ";
                //if (modifiers.HasFlag(ModifierKeys.Alt))
                //    result += "Alt + ";
                //if (modifiers.HasFlag(ModifierKeys.Shift))
                //    result += "Shift + ";
                // Get string representation
                int keyInt = (int)key;
                switch (key)
                {
                    case Key.Enter:
                        keyStr = "\n";
                        break;

                    case Key.Space:
                        keyStr = " ";
                        break;

                    case Key.Tab:
                        keyStr = "\t";
                        break;

                    default:
                        /*// Numeric keys are returned without the 'D'
                        if (key >= Key.D0 && key <= Key.D9)
                            //keyStr = char.ToString((char)(key - Key.D0 + '0'));
                            keyStr = KeyCodeToUnicode(key);
                        // Char keys are returned directly
                        else if (key >= Key.A && key <= Key.Z)
                            keyStr = KeyCodeToUnicode(key);
                            //keyStr = char.ToString((char)((key - Key.A + 'A') + (modifiers.HasFlag(ModifierKeys.Shift) ? 0 : ('a' - 'A'))));
                        // If the key is a keypad operation (Add, Multiply, ...) or an 'Oem' key, P/Invoke
                        else if ((keyInt >= 84 && keyInt <= 89) || keyInt >= 140)*/
                            keyStr = KeyCodeToUnicode(key);
                        break;
                }
            }

            return keyStr;
        }

        private static string KeyCodeToUnicode(Key key)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint virtualKeyCode = (uint)KeyInterop.VirtualKeyFromKey(key);
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            return result.ToString();
        }

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
    }
}
