using System;
using System.Collections.Generic;
using System.IO;

namespace VortexLocker.Utils
{
    public static class LockLogger
    {
        private static string FilePath { get; set; }

        private static Dictionary<string, List<string>> FileContents { get; set; }
        public static List<Tuple<string, bool>> PathsChanged { get; private set; } = new();

        public static void Init(string filepath)
        {
            if (filepath.EndsWith(App.FileExtension) == false)
            {
                throw new Exception("File extension not supported.");
            }
            FilePath = filepath;
        }

        // Returns false if the path is already present, so if it is already locked.
        public static bool RegisterFileToLock(string pathToLock, string username)
        {
            var fileContents = GetFileContents();
            foreach (var kvp in fileContents)
            {
                if (kvp.Value.Contains(pathToLock))
                {
                    return false;
                }
            }
            // If this username is not in the registered yet, register it.
            if (fileContents.ContainsKey(username) == false)
            {
                fileContents.Add(username, new List<string>());
            }

            PathsChanged.Add(new(pathToLock, true));
            fileContents[username].Add(pathToLock);
            return true;
        }

        // Returns false if username is not registered or if path is not locked by the username
        public static bool RegisterFileToUnlock(string pathToUnlock, string username)
        {
            var fileContents = GetFileContents();
            if (fileContents.ContainsKey(username) == false)
            {
                return false;
            }
            PathsChanged.Add(new(pathToUnlock, false));
            return fileContents[username].Remove(pathToUnlock);
        }

        public static Dictionary<string, List<string>> GetFileContents()
        {
            if (FileContents != null) return FileContents;

            Dictionary<string, List<string>> userPaths = new Dictionary<string, List<string>>();

            string[] lines = File.ReadAllLines(FilePath);
            string currentUser = null;

            foreach (string line in lines)
            {
                if (line.Length < 2)
                {
                    continue; // Ignore invalid lines
                }

                char type = line[0];
                // Skips the type and |
                string content = line.Substring(2);

                // Indicates this is a user
                if (type == 'u')
                {
                    currentUser = content;
                    if (!userPaths.ContainsKey(currentUser))
                    {
                        userPaths[currentUser] = new List<string>();
                    }
                }
                // Indicates this is a path
                else if (type == 'p' && currentUser != null)
                {
                    userPaths[currentUser].Add(content);
                }
            }
            FileContents = userPaths;
            return FileContents;
        }

        public static void SaveToFile()
        {
            WriteToFile(FileContents);
            FileContents = null;
        }

        private static void WriteToFile(Dictionary<string, List<string>> userPaths)
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (var kvp in userPaths)
                {
                    writer.WriteLine($"u|{kvp.Key}");

                    foreach (string path in kvp.Value)
                    {
                        writer.WriteLine($"p|{path}");
                    }
                }
            }
        }

        public static string GetUsernameByPath(string pathToCheck)
        {
            Dictionary<string, List<string>> userPaths = GetFileContents();

            foreach (var kvp in userPaths)
            {
                if (kvp.Value.Contains(pathToCheck))
                {
                    return kvp.Key;
                }
            }

            return null; // Path not found in the file
        }
    }
}
