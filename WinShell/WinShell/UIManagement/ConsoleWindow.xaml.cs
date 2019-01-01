using System;
using System.Collections.Generic;
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

namespace WinShell.UIManagement
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : UserControl
    {
        /// <summary>
        /// Gets the RunShellRequestCommand instance associated with this window.
        /// </summary>
        public ICommand RunShellRequestCommand { get; private set; }

        /// <summary>
        /// Gets or sets the output block to use for appending output text.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the shell session used with the command.
        /// </summary>
        private ShellSession ShellSession { get; set; }

        /// <summary>
        /// Default constructor. Mostly intended for "design-time" use in the Visual Studio editor.
        /// </summary>
        public ConsoleWindow() : this(null)
        {
        }

        /// <summary>
        /// Constructs a new ConsoleWindow instance, binding it to a shell session.
        /// </summary>
        public ConsoleWindow(ShellSession shellSession)
        {
            InitializeComponent();

            HyperlinkStyle = (Style)this.FindResource("HyperlinkStyle");
            InfoTextStyle = (Style)this.FindResource("InfoTextStyle");
            OutputBlockStyle = (Style)this.FindResource("OutputBlockStyle");
            OutputTextStyle = (Style)this.FindResource("OutputTextStyle");
            RunShellRequestCommand = new RunShellRequestCommand(shellSession);
            ShellSession = shellSession;
        }

        /// <summary>
        /// Clears the command output area.
        /// </summary>
        public void ClearOutput()
        {
            // Run update on UI thread.
            Dispatcher.Invoke(new Action(() =>
            {
                stackOutputPanel.Children.Clear();
            }));
        }

        /// <summary>
        /// Enables or disables input redirection for the corresponding shell session.
        /// </summary>
        /// <param name="enabled">A value indicating whether the enable input redirection.</param>
        /// <param name="targetStream">The stream to use for writing standard-input characters, if enabled is true.</param>
        /// <returns>The previous input-redirection-enable value.</returns>
        public bool EnableInputRedirection(bool enabled, StreamWriter targetStream)
        {
            var oldValue = ShellSession.UiRedirectionActive;
            ShellSession.UiRedirectionActive = enabled;
            ShellSession.UiTargetStandardIOStream = (enabled) ? targetStream : null;
            return oldValue;
        }

        /// <summary>
        /// Prepares the UI output window for the next set of output.
        /// </summary>
        public void StartNextOutputGrouping()
        {
            // Run update on UI thread.
            Dispatcher.Invoke(new Action(() =>
            {   CurrentOutputBlock = new TextBlock
                {
                    Style = OutputBlockStyle,
                };
                stackOutputPanel.Children.Add(CurrentOutputBlock);
            }));
        }

        /// <summary>
        /// Writes a command hyperlink to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        /// <param name="command">Command to associate with the hyperlink.</param>
        /// <param name="parameters">Object containing the parameters (if any) for the command.</param>
        public void WriteCommandLink(string outputText, ICommand command, object parameters)
        {
            // Run update on UI thread.
            Dispatcher.Invoke(new Action(() =>
            {
                CurrentOutputBlock.Inlines.Add(new Hyperlink(new Run(outputText))
                {
                    Style = HyperlinkStyle,
                    Command = command,
                    CommandParameter = parameters
                });

                ScrollToBottom();
            }));
        }

        /// <summary>
        /// Writes a string of informational (non-command output) text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteInfoText(string outputText)
        {
            // Run update on UI thread.
            Dispatcher.Invoke(new Action(() =>
            {
                CurrentOutputBlock.Inlines.Add(new Run
                {
                    Style = InfoTextStyle,
                    Text = outputText
                });

                ScrollToBottom();
            }));
        }

        /// <summary>
        /// Writes a string of command output text to the command output area.
        /// </summary>
        /// <param name="outputText">String to output.</param>
        public void WriteOutputText(string outputText)
        {
            // Run update on UI thread.
            Dispatcher.Invoke(new Action(() =>
            {
                CurrentOutputBlock.Inlines.Add(new Run
                {
                    Style = OutputTextStyle,
                    Text = outputText
                });

                ScrollToBottom();
            }));
        }

        /// <summary>
        /// Scrolls to the bottom of the command output area.
        /// </summary>
        public void ScrollToBottom()
        {
            // Run update on UI thread.
            Dispatcher.Invoke(new Action(() =>
            {
                viewOutputView.ScrollToBottom();
                viewOutputView.ScrollToLeftEnd();
            }));
        }
    }
}
