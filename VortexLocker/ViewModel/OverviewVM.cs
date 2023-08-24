using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using VortexLocker.Utils;
using System.Windows.Controls;
using VortexLocker.View;
using System.Text;

namespace VortexLocker.ViewModel
{
    public class OverviewVM : ObservableObject
    {
        private OverviewPage _page;
        private readonly Grouper _grouper = new();

        #region RelayCommands
        public RelayCommand TestButtonCommand { get; set; }
        public RelayCommand LockUnlockButtonCommand {get; set; }
        public RelayCommand OpenFileExplorerCommand {get; set; }
        public RelayCommand MoveToGroupCommand { get; set; }
        public RelayCommand CommitVortexCommand { get; set; }
        public RelayCommand CommitGitCommand { get; set; }
        #endregion

        public static ObservableCollection<string> TerminalEntries { get; private set; }
        public ObservableCollection<string> GroupsDisplay { get; private set; }
        public string SelectedGroupItem { get; set; }
        public TreeView TreeView { get { return _page?.treeView; } }
        public TreeViewItem SelectedTreeViewItem { get { return TreeView?.SelectedItem as TreeViewItem; } }
        public string SelectedTreeViewItemLockOwnership { get; private set; }
        public string LockUnlockButtonText { get; set; } = "Lock";
        public bool IsLockAvailable { get; set; } = false;

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
            LockUnlockButtonCommand = new RelayCommand(LockUnlockButton);
            OpenFileExplorerCommand = new RelayCommand(OpenFileExplorer);
            MoveToGroupCommand = new RelayCommand(MoveToGroup);
            CommitVortexCommand = new RelayCommand(CommitVortex);
            CommitGitCommand = new RelayCommand(CommitGit);
            TerminalEntries = new();
            LogOnTerminal("TERMINAL LOG");
            LogOnTerminal("============");
            LogOnTerminal("");
            LogOnTerminal($"git username: {MainVM.Instance.GitUsername}");

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

            TreeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        #region Buttons
        private void TestButton()
        {
            LogOnTerminal($"{TreeView.SelectedItem}");
        }
        private void LockUnlockButton()
        {
            var tvi = TreeView.SelectedItem as TreeViewItem;
            LogOnTerminal($"LockUnlockButton called on {tvi.Header}, Assume is folder: {tvi.HasItems}");
            if (tvi.HasItems)
            {
                LogOnTerminal($"LockUnlockButton currently does not support folder locking");
                return;
            }
            if (LockUnlockButtonText == "Lock")
            {
                LockLogger.RegisterFileToLock(FileManager.GetRelativePath(Directory.GetParent(App.FileArg).FullName, GetFullPath(tvi)), MainVM.Instance.GitUsername);
            }
            else
            {
                LockLogger.RegisterFileToUnlock(FileManager.GetRelativePath(Directory.GetParent(App.FileArg).FullName, GetFullPath(tvi)), MainVM.Instance.GitUsername);
            }
        }
        private void OpenFileExplorer()
        {
            var path = GetFullPath(SelectedTreeViewItem);
            path = Directory.GetParent(path).FullName;
            LogOnTerminal($"OpenFileExplorer: {path}");
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        private void MoveToGroup()
        {
            LogOnTerminal($"MoveToGroup called");
        }
        private void CommitVortex()
        {
            LogOnTerminal($"Saving LockLogs to: {App.FileArg}");
            var fileContents = LockLogger.GetFileContents();
            MainVM.Instance.FileManager.LockAll();
            foreach (var kvp in fileContents)
            {
                if (kvp.Key == MainVM.Instance.GitUsername)
                {
                    foreach (var file in kvp.Value)
                    {
                        FileManager.UnlockFile($"{Directory.GetParent(App.FileArg)}/{file}");
                    }
                    break;
                }
            }
            LockLogger.SaveToFile();
        }
        private void CommitGit()
        {
            LogOnTerminal(CmdHelper.Stagechange(App.FileArg));
            var pch = LockLogger.PathsChanged;
            List<string> pathsChanged = new();
            pch.ForEach(x => { pathsChanged.Add($"{(x.Item2 ? 'L' : 'U')}|{x.Item1}"); });
            pch.Clear();
            LogOnTerminal(CmdHelper.LockCommit(pathsChanged));
        }
        #endregion

        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            OnPropertyChanged(nameof(SelectedTreeViewItem));
            UpdateLockOwnerShipText();
            UpdateLockUnlockButtonText();
        }

        #region UpdateView
        private void UpdateLockUnlockButtonText()
        {
            if (SelectedTreeViewItemLockOwnership == "Noone")
            {
                LockUnlockButtonText = "Lock";
                IsLockAvailable = true;
            }
            else if (SelectedTreeViewItemLockOwnership == MainVM.Instance.GitUsername)
            {
                LockUnlockButtonText = "Unlock";
                IsLockAvailable = true;
            }
            else // locked by someone else
            {
                IsLockAvailable = false;
            }

            OnPropertyChanged(nameof(LockUnlockButtonText));
            OnPropertyChanged(nameof(IsLockAvailable));
        }

        private void UpdateLockOwnerShipText()
        {
            string path = GetFullPath(SelectedTreeViewItem);
            path = FileManager.GetRelativePath(Directory.GetParent(MainVM.Instance.FileManager.RootDirectory).FullName, path);
            string username = LockLogger.GetUsernameByPath(path);
            SelectedTreeViewItemLockOwnership = username ?? "Noone"; // ?? takes the first thing only if it isn't null

            OnPropertyChanged(nameof(SelectedTreeViewItemLockOwnership));
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
                var subDirItem = new TreeViewItem
                {
                    Header = new DirectoryInfo(subDir).Name
                };
                parentItem.Items.Add(subDirItem);

                UpdateTreeView(subDirItem, subDir);
            }

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                var fileItem = new TreeViewItem
                {
                    Header = new FileInfo(file).Name
                };
                parentItem.Items.Add(fileItem);
            }
        }
        #endregion

        private string GetFullPath(TreeViewItem treeViewItem)
        {
            // Start from the child path. The rest of the path will be pushed as we go along
            List<string> parsedReversedPath = new()
            {
                treeViewItem.Header as string
            };
            TreeViewItem parent = SelectedTreeViewItem.Parent as TreeViewItem;
            if (parent != null)
            {
                do
                {
                    parsedReversedPath.Add(parent.Header as string);
                    parent = parent.Parent as TreeViewItem;
                } while (parent != null);
            }
            StringBuilder sb = new();
            // Reverse so that the last element is the most specific file or directory
            parsedReversedPath.Reverse();
            // Build the absolute path
            for (int i = 0; i < parsedReversedPath.Count; ++i)
            {
                if (i != 0)
                {
                    sb.Append('\\');
                }
                sb.Append(parsedReversedPath[i]);
            }
            return sb.ToString();
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
