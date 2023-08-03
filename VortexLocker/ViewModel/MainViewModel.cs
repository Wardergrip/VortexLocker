using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace VortexLocker.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {

        }

        public string GitUsername 
        { 
            get { return Utils.CmdHelper.GetGitUsername(); } 
        }
    }
}
