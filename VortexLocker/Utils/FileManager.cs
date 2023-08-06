using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VortexLocker.Utils
{
    // Can be optimised with dirty flag.
    public class FileManager
    {
        public string RootDirectory { get; set; }
        public FileManager(string directory)
        {
            RootDirectory = directory;
        }

        public List<string> GetAllDirectories()
        {
            List<string> directories = new List<string>();
            // https://www.tutorialspoint.com/how-to-get-all-the-directories-and-sub-directories-inside-a-path-in-chash#:~:text=To%20get%20the%20directories%20C%23,directory%2C%20and%20optionally%20searches%20subdirectories.
            directories.AddRange(Directory.GetDirectories(RootDirectory, "*", SearchOption.AllDirectories));

            return directories;
        }

        public List<string> GetFiles()
        {
            List<string> files = new List<string>();

            files.AddRange(Directory.GetFiles(RootDirectory));

            return files;
        }

        public List<string> GetAllFiles()
        {
            List<string> directories = GetAllDirectories();
            List<string> files = new List<string>();
            foreach (var dir in directories)
            {
                files.AddRange(Directory.GetFiles(dir));
            }
            return files;
        }

        public void LockAll()
        {
            GetAllFiles().ForEach(path => LockFile(path));
        }

        public void UnlockAll()
        {
            GetAllFiles().ForEach(path => UnlockFile(path));
        }

        public void LockFile(string path)
        {
            File.SetAttributes(path, FileAttributes.ReadOnly);
        }

        public void UnlockFile(string path)
        {
            File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
        }
    }
}
