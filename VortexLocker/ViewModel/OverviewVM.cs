using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace VortexLocker.ViewModel
{
    public class OverviewVM : ObservableObject
    {
        public RelayCommand TestButtonCommand { get; set; }
        public static ObservableCollection<string> TerminalEntries { get; private set; }

        public OverviewVM()
        {
            TestButtonCommand = new RelayCommand(TestButton);
            TerminalEntries = new();
            LogOnTerminal("TERMINAL LOG");
            LogOnTerminal("============");
            LogOnTerminal("");
        }

        private void TestButton()
        {
            LogOnTerminal("A new log msg");
        }

        public static void LogOnTerminal(string message) 
        {
            TerminalEntries.Add($"{DateTime.Now} | {message}");
        }

        public static void SaveTerminalLog(string filepath)
        {
            StreamWriter writer = new(filepath);

            foreach (var entry in TerminalEntries)
            {
                writer.WriteLine(entry);
            }
        }
    }
}
