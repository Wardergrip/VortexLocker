using System;
using System.Collections.Generic;
using System.Text;

namespace VortexLocker.Utils
{
    public static class CmdHelper
    {
        public static string Run(string command, string commandParameters = "")
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
            //pProcess.StartInfo.WorkingDirectory = workingDir;
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
            return Run("git", "config user.name");
        }
    }
}
