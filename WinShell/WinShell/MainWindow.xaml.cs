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
        public string CurrentWorkingDirectory { get; private set; }

        public ICommand ProcessorCommand { get; private set; }

        private List<string> CommandHistory { get; set; } = new List<string>();

        private int IndexOfLastHistoryDisplayed { get; set; }

        private CommandProcessor Processor { get; set; }

        private TextBlock CurrentOutputBlock { get; set; }

        /// <summary>
        /// Gets or sets the style used for hyperlinked text.
        /// </summary>
        private Style HyperlinkStyle { get; set; }

        /// <summary>
        /// Gets or sets the style used for informational text.
        /// </summary>
        private Style InfoTextStyle { get; set; }

        /// <summary>
        /// Gets or sets the style used for output text block.
        /// </summary>
        private Style OutputBlockStyle { get; set; }

        /// <summary>
        /// Gets or sets the style used for command output text.
        /// </summary>
        private Style OutputTextStyle { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            PresentCommandPrompt();
            Processor = new CommandProcessor();
            ProcessorCommand = new ProcessorCommand() { MainWindow = this };
            HyperlinkStyle = (Style)this.FindResource("HyperlinkStyle");
            InfoTextStyle = (Style)this.FindResource("InfoTextStyle");
            OutputBlockStyle = (Style)this.FindResource("OutputBlockStyle");
            OutputTextStyle = (Style)this.FindResource("OutputTextStyle");
        }

        /// <summary>
        /// Writes a command hyperlink to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        /// <param name="command">Command to associate with the hyperlink.</param>
        /// <param name="parameters">Object containing the parameters (if any) for the command.</param>
        public void WriteCommandLink(string outputText, ICommand command, object parameters)
        {
            CurrentOutputBlock.Inlines.Add(new Hyperlink(new Run(outputText))
            {
                Style = HyperlinkStyle,
                Command = command,
                CommandParameter = parameters
            });

            ScrollToBottom();
        }

        /// <summary>
        /// Writes a string of informational (non-command output) text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteInfoText(string outputText)
        {
            CurrentOutputBlock.Inlines.Add(new Run
            {
                Style = InfoTextStyle,
                Text = outputText
            });

            ScrollToBottom();
        }

        /// <summary>
        /// Writes a string of command output text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteOutputText(string outputText)
        {
            CurrentOutputBlock.Inlines.Add(new Run
            {
                Style = OutputTextStyle,
                Text = outputText
            });

            ScrollToBottom();
        }

        /// <summary>
        /// Scrolls to the bottom of the command output area.
        /// </summary>
        public void ScrollToBottom()
        {
            viewOutputView.ScrollToBottom();
            viewOutputView.ScrollToLeftEnd();
        }

        private void PresentCommandPrompt()
        {
            CurrentWorkingDirectory = Directory.GetCurrentDirectory();
            IndexOfLastHistoryDisplayed = -1;
            txtCommand.Text = string.Empty;
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

        private void txtCommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    var command = txtCommand.Text;
                    CommandHistory.Add(command);
                    ProcessCommand(command);
                    PresentCommandPrompt();
                    e.Handled = true;
                    break;

                case Key.Escape:
                    PresentCommandPrompt();
                    e.Handled = true;
                    break;

                case Key.Up:
                    ShowCommandFromHistory(-1);
                    e.Handled = true;
                    break;

                case Key.Down:
                    ShowCommandFromHistory(1);
                    e.Handled = true;
                    break;
            }
        }

        public void ProcessCommand(string command)
        {
            CurrentOutputBlock = new TextBlock
            {
                Style = OutputBlockStyle,
            };
            stackOutputPanel.Children.Add(CurrentOutputBlock);

            Processor.ProcessCommand(command, this);
        }
    }
}
