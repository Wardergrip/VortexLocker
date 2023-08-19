using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using VortexLocker.Utils;
using System.Windows.Controls;
using VortexLocker.View;
using System.Windows.Shapes;

namespace VortexLocker.ViewModel
{
    public class OverviewVM : ObservableObject
    {
        private Grouper _grouper = new();
        public RelayCommand TestButtonCommand { get; set; }
        public static ObservableCollection<string> TerminalEntries { get; private set; }
        public ObservableCollection<string> GroupsDisplay { get; private set; }
        public string SelectedGroupItem { get; set; }
        public TreeView TreeView { get { return _page.treeView; } }
        private OverviewPage _page;

        public OverviewVM() 
        { 
            // When the page gets initialised, it automatically makes a OverviewVM
            // This does not need to happen, but when constructing it cannot pass arguments.
            // Hence this empty constructor.
        }

        public OverviewVM(OverviewPage overviewPage)
        {
            _page = overviewPage;
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

            TreeView.Items.Clear(); // Clear any previous data


            var path = Directory.GetParent(MainVM.Instance.FileManager.RootDirectory).FullName;
            var rootItem = new TreeViewItem
            {
                Header = path
            };
            TreeView.Items.Add(rootItem);
            UpdateTreeView(rootItem, path);
        }

        private void TestButton()
        {
            LogOnTerminal($"{TreeView.SelectedItem}");
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

        private void UpdateTreeView(TreeViewItem parentItem, string path)
        {
            string[] subDirectories = Directory.GetDirectories(path);
            foreach (string subDir in subDirectories)
            {
                var subDirItem = new TreeViewItem();
                subDirItem.Header = new DirectoryInfo(subDir).Name;
                parentItem.Items.Add(subDirItem);

                UpdateTreeView(subDirItem, subDir);
            }

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                var fileItem = new TreeViewItem();
                fileItem.Header = new FileInfo(file).Name;
                parentItem.Items.Add(fileItem);
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
