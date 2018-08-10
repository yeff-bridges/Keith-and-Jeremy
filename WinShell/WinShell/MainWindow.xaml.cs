using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace WinShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentWorkingDirectory { get; set; }

        public List<string> CommandHistory { get; set; } = new List<string>();

        public int IndexOfLastHistoryDisplayed { get; set; }

        public CommandProcessor Processor { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            PresentCommandPrompt();
            Processor = new CommandProcessor();
        }

        /// <summary>
        /// Writes a string of informational (non-command output) to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteInfoText(string outputText)
        {
            stackOutputPanel.Children.Add(new TextBlock
            {
                Background = Brushes.Black,
                Foreground = Brushes.LightGreen,
                Text = outputText
            });
        }

        /// <summary>
        /// Writes a string of command output text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteOutputText(string outputText)
        {
            stackOutputPanel.Children.Add(new TextBlock
            {
                Background = Brushes.Black,
                Foreground = Brushes.White,
                Text = outputText
            });
        }

        private void PresentCommandPrompt()
        {
            CurrentWorkingDirectory = Directory.GetCurrentDirectory();
            IndexOfLastHistoryDisplayed = -1;
        }

        private void btnOpenCurrentDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", CurrentWorkingDirectory);
        }

        private void ShowCommandFromHistory(int historyOffset)
        {
            // If we have any commands in our history...
            if (CommandHistory.Count > 0)
            {
                int historyIndex;

                // If we've not already displayed a history entry while entering the current command...
                if (IndexOfLastHistoryDisplayed == -1)
                {
                    // Use the most recent history entry if we're advancing in a positive direction.
                    if (historyOffset > 0)
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

        private void txtCommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    var command = txtCommand.Text;

                    CommandHistory.Add(command);
                    //stackOutputPanel
                    Processor.ProcessCommand(command, this);
                    txtCommand.Text = string.Empty;
                    e.Handled = true;
                    break;

                case Key.Up:
                    ShowCommandFromHistory(1);
                    e.Handled = true;
                    break;

                case Key.Down:
                    ShowCommandFromHistory(-1);
                    e.Handled = true;
                    break;
            }
        }
    }
}
