using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VortexLocker.Utils
{
    public class FileManager
    {
        public string RootDirectory { get; private set; }
        public FileManager(string directory)
        {
            RootDirectory = directory;
            //Directory.GetDirectories(".");
        }
    }
}
