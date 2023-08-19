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

        public static string GetRelativePath(string basePath, string absolutePath)
        {
#if USE_NORMAL_RELATIVEPATHING
            // Example:
            // string basePath = @"C:\MyProjects\";
            // string absolutePath = @"C:\MyProjects\Subfolder\file.txt";
            // Relative = @"Subfolder\file.txt"
            Uri baseUri = new Uri(basePath);
            Uri absoluteUri = new Uri(absolutePath);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(absoluteUri).ToString());
#else
            Uri baseUri = new Uri(basePath);
            Uri absoluteUri = new Uri(absolutePath);

            // Remove the common base path from the absolute path
            string relativePath = Uri.UnescapeDataString(absoluteUri.AbsolutePath.Replace(baseUri.AbsolutePath, ""));

            // Trim any leading directory separator characters
            char[] directorySeparatorChars = { '\\', '/' };
            relativePath = relativePath.TrimStart(directorySeparatorChars);

            return relativePath;
#endif
        }

        public List<string> GetAllAbsoluteDirectories()
        {
            List<string> directories = new List<string>();
            directories.Add(RootDirectory);
            // https://www.tutorialspoint.com/how-to-get-all-the-directories-and-sub-directories-inside-a-path-in-chash#:~:text=To%20get%20the%20directories%20C%23,directory%2C%20and%20optionally%20searches%20subdirectories.
            directories.AddRange(Directory.GetDirectories(RootDirectory, "*", SearchOption.AllDirectories));

            return directories;
        }

        public List<string> GetAllRelativeDirectories()
        {
            List<string> directories = GetAllAbsoluteDirectories();
            for (int i = 0; i < directories.Count; ++i)
            {
                directories[i] = GetRelativePath(RootDirectory, directories[i]);
            }
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
            List<string> directories = GetAllAbsoluteDirectories();
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

        public static void LockFile(string path)
        {
            File.SetAttributes(path, FileAttributes.ReadOnly);
        }

        public static void UnlockFile(string path)
        {
            File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
        }
    }
}
