using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VortexLocker.Utils
{
    public static class CmdHelper
    {
        public static string Run(string command, string commandParameters = "", string workingDir = null)
        {
            // https://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = command;
            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = commandParameters;
            pProcess.StartInfo.UseShellExecute = false;
            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;
            //Optional
            if (workingDir != null) pProcess.StartInfo.WorkingDirectory = workingDir;
            //Start the process
            pProcess.Start();
            //Get program output
            string output = pProcess.StandardOutput.ReadToEnd();
            //Wait for process to finish
            pProcess.WaitForExit();
            return output;
        }

        public static string GetGitUsername()
        {
            return Run("git", "config user.name").TrimEnd('\n');
        }

        public static string LockCommit(List<string> filesToLock, bool mentionFilesInCommitDesc = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[VORTEXLOCKER] Automated commit\n\n");
            if (mentionFilesInCommitDesc)
            {
                foreach (var file in filesToLock)
                {
                    sb.Append(file);
                    sb.Append("\n");
                }
            }
            return Run("git", $"commit -m \"[VORTEX] Locking files\" -m \"{sb}\"", Directory.GetParent(App.FileArg).FullName);
        }

        //public static string UnlockCommit(List<string> filesToUnlock, bool mentionFilesInCommitDesc = true)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("[VORTEXLOCKER] Automated commit\n\n");
        //    if (mentionFilesInCommitDesc)
        //    {
        //        foreach (var file in filesToUnlock)
        //        {
        //            sb.Append(file);
        //            sb.Append("\n");
        //        }
        //    }
        //    return Run("git", $"commit -m [VORTEX] Unlocking files -m {sb.ToString()}");
        //}

        public static string Stagechange(string filePath)
        {
            return Run("git", $"add {filePath}");
        }
        
        public static string Unstagechange(string filePath)
        {
            return Run("git", $"restore --stage {filePath}");
        }

        public static string PushCommits()
        {
            return Run("git", "push");
        }
    }
}
