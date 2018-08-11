using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinShell
{
    public class ProcessorCommand : ICommand
    {
        public MainWindow MainWindow { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MainWindow.ProcessCommand(parameter as string);
        }

        public event EventHandler CanExecuteChanged;
    }
}
