using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Diagnostics;
using VortexLocker.Utils;
using VortexLocker.View;

namespace VortexLocker.ViewModel
{
    public class MainVM : ObservableObject
    {
        public static MainVM Instance { get; private set; }

        private FileManager _fileManager;
        public FileManager FileManager { get { return _fileManager; } }

        public RelayCommand OpenGithubReposCommand { get; private set; }
        public OverviewPage OverviewPage { get; private set; }
        public string WindowTitle { get; private set; }

        public MainVM()
        {
            WindowTitle = $"VortexLocker v0.1 | User: {GitUsername}";
            OpenGithubReposCommand = new RelayCommand(OpenGithubRepos);

            if (App.FileArg != null)
            {
                Init(App.FileArg);
            }

            if (Instance != null)
            {
                throw new System.Exception("Multiple MainVMs");
            }
            else
            {
                Instance = this;
            }

            OverviewPage = new();
        }
            public void Init(string filePath, bool initLockLogger = true)
        {
            _fileManager = new FileManager(filePath);
            if (!initLockLogger) return;
            LockLogger.Init(filePath);
        }
        
        public string GitUsername 
        { 
            get { return CmdHelper.GetGitUsername(); } 
        }

        public List<string> Directories
        {
            get 
            {
                if (_fileManager == null) return null;
                return _fileManager.GetAllAbsoluteDirectories(); 
            }
        }

        private void OpenGithubRepos() => Process.Start(new ProcessStartInfo("https://github.com/Wardergrip/VortexLocker"));
    }
}
