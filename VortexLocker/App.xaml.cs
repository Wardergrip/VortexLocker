using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VortexLocker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string FileExtension = ".vortex";
        public static string FileArg { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length > 0)
            {
                string filePath = e.Args[0];
                if (filePath.EndsWith(FileExtension) == false)
                {
                    MessageBox.Show($"This fileformat is not supported. Supported extensions: {FileExtension}");
                    throw new Exception($"This fileformat is not supported. Supported extensions: {FileExtension}");
                }
                FileArg = filePath;
            }
            else
            {
                // Would be nice if this path is the previously loaded path.
                // Currently it's the folder of the .exe
                FileArg = null;
            }
        }
    }
}
