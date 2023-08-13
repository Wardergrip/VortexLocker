using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using VortexLocker.Utils;

namespace VortexLocker.ViewModel
{
    public class OverviewVM : ObservableObject
    {
        private Grouper _grouper = new();
        public RelayCommand TestButtonCommand { get; set; }
        public static ObservableCollection<string> TerminalEntries { get; private set; }
        public ObservableCollection<string> GroupsDisplay { get; private set; }
        public string SelectedGroupItem { get; set; }

        public OverviewVM()
        {
            TestButtonCommand = new RelayCommand(TestButton);
            TerminalEntries = new();
            LogOnTerminal("TERMINAL LOG");
            LogOnTerminal("============");
            LogOnTerminal("");

            GroupsDisplay = new()
            {
                "RootGroup",
                "   GROUP 0:"
            };
            _grouper.OnGroupsChanged += UpdateGroupsDisplay;
        }

        private void TestButton()
        {
            LogOnTerminal($"{SelectedGroupItem}");
        }

        private void UpdateGroupsDisplay()
        {
            GroupsDisplay.Clear();
            GroupsDisplay.Add("RootGroup");
            for (int i = 0; i < _grouper.Groups.Count; ++i) 
            {
                GroupsDisplay.Add($"    GROUP {i}:");
                foreach (string path in _grouper.Groups[i])
                {
                    GroupsDisplay.Add($"        {path}");
                }
            }
        }

        #region Terminal
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
        #endregion
    }
}
