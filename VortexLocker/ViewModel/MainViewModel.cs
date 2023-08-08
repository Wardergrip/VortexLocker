using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Diagnostics;
using VortexLocker.Utils;
using VortexLocker.View;

namespace VortexLocker.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        // This needs to be stored in a file and be configured from within.
        private FileManager _fileManager;

        public RelayCommand OpenGithubReposCommand { get; private set; }
        public OverviewPage OverviewPage { get; private set; } = new OverviewPage();
        public string WindowTitle { get; private set; }

        public MainViewModel()
        {
            WindowTitle = $"VortexLocker v0.1 | User: {GitUsername}";
            OpenGithubReposCommand = new RelayCommand(OpenGithubRepos);
            _fileManager = new FileManager(App.FileArg);
            //LockLogger.Init(App.FileArg);
        }

        public string GitUsername 
        { 
            get { return CmdHelper.GetGitUsername(); } 
        }

        public List<string> Directories
        {
            get { return _fileManager.GetAllAbsoluteDirectories(); }
        }

        private void OpenGithubRepos() => Process.Start(new ProcessStartInfo("https://github.com/Wardergrip/VortexLocker"));
    }
}
