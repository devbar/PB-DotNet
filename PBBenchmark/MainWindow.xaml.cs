using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PBDotNetLib.pbuilder;
using PBDotNetLib.orca;
using System.Threading;
using Powerscript = PBDotNetLib.pbuilder.powerscript;
using System.Configuration;

namespace PBBenchmark
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<LibEntry> entries;
        private Workspace workspace;
        private object lockObj = new object();
        private Orca.Version pbVersion;

        public MainWindow()
        {
            InitializeComponent();
            string pbVersion = ConfigurationManager.AppSettings["PBVersion"];
            if (String.IsNullOrEmpty(pbVersion)) {
                this.pbVersion = Orca.Version.PB126;
            } else {
                switch (pbVersion) {
                    case "115":
                        this.pbVersion = Orca.Version.PB115;
                        break;
                    case "125":
                        this.pbVersion = Orca.Version.PB125;
                        break;
                    case "126":
                        this.pbVersion = Orca.Version.PB126;
                        break;
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DateTime start;
            TimeSpan result;
            OpenFileDialog dialog = new OpenFileDialog();

            if (!dialog.ShowDialog().Value)
                return;

            workspace = new Workspace(dialog.FileName, this.pbVersion);

            if (workspace.Targets.Length > 0)
            {
                start = DateTime.Now;                
                ReadTarget(workspace.Targets[0]);
                result = DateTime.Now.Subtract(start);
                Result("ReadTarget: " + result.Minutes + ":" + result.Seconds);

                start = DateTime.Now;
                ReadCode();
                result = DateTime.Now.Subtract(start);
                Result("ReadCode: " + result.Minutes + ":" + result.Seconds);

                start = DateTime.Now;
                ParseCode();
                result = DateTime.Now.Subtract(start);
                Result("ParseCode: " + result.Minutes + ":" + result.Seconds);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DateTime start;
            TimeSpan result;
            OpenFileDialog dialog = new OpenFileDialog();

            if (!dialog.ShowDialog().Value)
                return;

            workspace = new Workspace(dialog.FileName, this.pbVersion);

            if (workspace.Targets.Length > 0)
            {
                start = DateTime.Now;
                ReadTargetThreaded(workspace.Targets[0]);
                result = DateTime.Now.Subtract(start);
                Result("ReadTarget (Thread): " + result.Minutes + ":" + result.Seconds);
            }
        }

        private void Result(string message)
        {
            textBoxResult.Text += message + "\r\n";
        }

        private void ReadCode()
        {
            Orca orca = new Orca(this.pbVersion);

            foreach(LibEntry entry in entries)
            {
                orca.FillCode(entry);
            }
        }

        private void ReadTarget(Target target)
        {
            StringBuilder sb = new StringBuilder();

            entries = new List<LibEntry>();

            Library[] test1 = target.Libraries;
            foreach (Library lib in test1)
            {
                LibEntry[] test = lib.EntryList;
                foreach (LibEntry entry in test)
                {
                    sb.Append(entry.Name + "\r\n");
                    //addText(entry.Name);
                }
                entries.AddRange(test);
            }

            addText(sb.ToString());
        }

        private void ParseCode()
        {

            foreach (LibEntry entry in entries)
            {
                switch (entry.Type)
                {
                    case LibEntry.Objecttype.Datawindow:
                        Powerscript.Datawindow.GetDatawindowFromSource(entry.Source);
                        
                        break;
                    case LibEntry.Objecttype.Structure:
                    case LibEntry.Objecttype.Menu:
                    case LibEntry.Objecttype.Function:
                    case LibEntry.Objecttype.Application:
                    case LibEntry.Objecttype.Userobject:
                    case LibEntry.Objecttype.Window:
                        Powerscript.Type.GetTypesFromSource(entry.Source);
                        break;

                }
            }
        }

        private void ReadTargetStart(object t)
        {
            Tuple<Target, int, int> v = (Tuple<Target, int, int>)t;

            for (int i = v.Item2; i < v.Item1.Libraries.Length; i = i + v.Item3)
            {
                Library lib = v.Item1.Libraries[i];

                LibEntry[] test = lib.EntryList;
                foreach (LibEntry entry in test)
                {
                    //addText(entry.Name);
                }
            }

            addText("Done");
        }

        private void ReadTargetThreaded(Target target)
        {
            Thread t1, t2;

            t1 = new Thread(new ParameterizedThreadStart(this.ReadTargetStart));
            t1.Start(new Tuple<Target, int, int>(target, 0, 2));

            t2 = new Thread(new ParameterizedThreadStart(this.ReadTargetStart));
            t2.Start(new Tuple<Target, int, int>(target, 1, 2));
        }

        private void addText(string text)
        {
            /*lock (lockObj)
            {*/
                if (!textBox1.Dispatcher.CheckAccess())
                {
                    textBox1.Dispatcher.Invoke((Action)delegate { addText(text); });
                    return;
                }

                textBox1.Text += text + "\r\b";
            /*}*/
        }

        


    }
}
