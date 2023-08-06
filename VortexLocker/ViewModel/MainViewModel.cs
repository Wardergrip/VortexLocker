using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using VortexLocker.Utils;

namespace VortexLocker.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        // This needs to be stored in a file and be configured from within.
        private FileManager _fileManager = new FileManager("E:\\Git\\VortexLocker\\VortexLocker");

        public MainViewModel()
        {

        }

        public string GitUsername 
        { 
            get { return CmdHelper.GetGitUsername(); } 
        }

        public List<string> Directories
        {
            get { return _fileManager.GetAllDirectories(); }
        }
    }
}
