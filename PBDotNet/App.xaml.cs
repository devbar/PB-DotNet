using PBDotNet.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace PBDotNet
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            if (e.Args.Length > 0) {
                string path = e.Args[0];

                if(System.IO.File.Exists(path)){
                    if(path.EndsWith(".pbw")){
                        CmdlineArgs.Workspace = path;
                    } else if (path.EndsWith(".pbl")) {
                        CmdlineArgs.Library = path;
                    }
                }                
            }
            base.OnStartup(e);
        }
    }
}
