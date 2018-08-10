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

namespace WinShell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentWorkingDirectory { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            PresentCommandPrompt();
        }

        public void PresentCommandPrompt()
        {
            CurrentWorkingDirectory = Directory.GetCurrentDirectory();
        }
    }
}
