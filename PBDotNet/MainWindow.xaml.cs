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
using Powerscript=PBDotNetLib.pbuilder.powerscript;
using System.Configuration;
using PBDotNet.Util;

namespace PBDotNet
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Workspace workspace;
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

            if (CmdlineArgs.Workspace != null) {
                this.openWorkspace(CmdlineArgs.Workspace);
            } else if (CmdlineArgs.Library != null) {
                this.openLibrary(CmdlineArgs.Library);
            }
        }
        
        private void menuOpenWorkspace_Click(object sender, RoutedEventArgs e){
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Powerbuilder Workspace (*.pbw)|*.pbw";

            if (!dialog.ShowDialog().Value)
                return;

            this.openWorkspace(dialog.FileName);
        }


        private void menuOpenLibrary_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Powerbuilder Librar (*.pbl)|*.pbl";

            if (!dialog.ShowDialog().Value)
                return;

            this.openLibrary(dialog.FileName);
        }

        private void openLibrary(string libraryPath) {
            Library lib = new Library(libraryPath, this.pbVersion);
            lsObjects.ItemsSource = new Orca(this.pbVersion).DirLibrary(lib.Dir + "\\" + lib.File);

            listWorkspace.DataContext = null;
            listWorkspace.Visibility = System.Windows.Visibility.Collapsed;
            gridWorkspace.Width = new GridLength(0);
        }

        private void openWorkspace(string workspacePath) {
            workspace = new Workspace(workspacePath, this.pbVersion);

            stWorkspace.Text = workspacePath;
            stVersion.Text = workspace.MajorVersion + "." + workspace.MinorVersion;

            listWorkspace.DataContext = workspace.Targets;
            listWorkspace.Visibility = System.Windows.Visibility.Visible;
            gridWorkspace.Width = new GridLength(300);
        }

        private void listWorkspace_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Library)
            {
                Library lib = (Library)e.NewValue;
                lsObjects.ItemsSource = new Orca(this.pbVersion).DirLibrary(lib.Dir + "\\" + lib.File);
            }
        }

        private void lsObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PBDotNetLib.orca.LibEntry lib;
            Powerscript.Type[] types;
            Powerscript.Datawindow dw;

            lib = (PBDotNetLib.orca.LibEntry)lsObjects.SelectedItem;
            if (lib != null)
            {
                new Orca(this.pbVersion).FillCode(lib);

                txtSource.Text = lib.Source;
                
                switch (lib.Type)
                {
                    case LibEntry.Objecttype.Datawindow:                        
                        tabUserobject.Visibility = System.Windows.Visibility.Collapsed;
                        tabDatawindow.Visibility = System.Windows.Visibility.Visible;

                        dw = Powerscript.Datawindow.GetDatawindowFromSource(lib.Source);
                        txtDwRelease.Text = dw.Release;
                        listDatawindowObjs.ItemsSource = dw.Object.Childs;

                        break;
                    case LibEntry.Objecttype.Structure:
                        tabUserobject.Header = "Structure";
                        goto case LibEntry.Objecttype.Window;
                    case LibEntry.Objecttype.Menu:
                        tabUserobject.Header = "Menu";
                        goto case LibEntry.Objecttype.Window;
                    case LibEntry.Objecttype.Function:
                        tabUserobject.Header = "Function";
                        goto case LibEntry.Objecttype.Window;
                    case LibEntry.Objecttype.Application:
                        tabUserobject.Header = "Application";
                        goto case LibEntry.Objecttype.Window;
                    case LibEntry.Objecttype.Userobject:
                        tabUserobject.Header = "UserObject";
                        goto case LibEntry.Objecttype.Window;
                    case LibEntry.Objecttype.Window:
                        tabUserobject.Visibility = System.Windows.Visibility.Visible;
                        tabDatawindow.Visibility = System.Windows.Visibility.Collapsed;

                        if(lib.Type == LibEntry.Objecttype.Window)
                            tabUserobject.Header = "Window";

                        types = Powerscript.Type.GetTypesFromSource(lib.Source);
                        if (types == null)
                        {
                            MessageBox.Show("keine Types");
                            return;
                        }

                        gridUoTypes.ItemsSource = types;

                        if (types.Length > 0)
                        {
                            txtUoIVariables.Text = types[0].InstanceVariables;
                            txtUoSVariables.Text = types[0].SharedVariables;
                            txtUoGVariables.Text = types[0].GlobalVariables;
                            txtUoExtFunctions.Text = types[0].ExternalFunctions;
                            gridUoMethods.ItemsSource = types[0].Methods;
                        }
                        break;

                }
            }
        }

        private void gridUoTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridUoTypes.SelectedIndex < 0)
                return;

            Powerscript.Type type = ((Powerscript.Type[])gridUoTypes.ItemsSource)[gridUoTypes.SelectedIndex];

            gridUoProps.ItemsSource = type.Properties;
            gridUoEvents.ItemsSource = type.Events;
        }

        private void gridUoEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridUoEvents.SelectedIndex < 0)
                return;

            txtUoSource.Text = ((Powerscript.Event[])gridUoEvents.ItemsSource)[gridUoEvents.SelectedIndex].Source;
        }

        private void gridUoTypes_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void gridUoMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridUoMethods.SelectedIndex < 0)
                return;

            txtUoSource.Text = ((Powerscript.Method[])gridUoMethods.ItemsSource)[gridUoMethods.SelectedIndex].Source;
        }

        private void listWorkspace_SelectedItemChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void listDatawindowObjs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Powerscript.Datawindow.Element)
            {
                Powerscript.Datawindow.Element element = (Powerscript.Datawindow.Element)e.NewValue;
                txtDwValue.Text = element.Value;
                dgProperties.ItemsSource = element.Attributes;
            }
        }
    }
}
