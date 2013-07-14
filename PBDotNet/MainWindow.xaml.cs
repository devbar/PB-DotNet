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

namespace PBDotNet
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Workspace workspace;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuOpenWorkspace_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (!dialog.ShowDialog().Value)
                return;

            workspace = new Workspace(dialog.FileName);

            stWorkspace.Text = dialog.FileName;
            stVersion.Text = workspace.MajorVersion + "." + workspace.MinorVersion;

            listWorkspace.DataContext = workspace.Targets;
        }

        private void listWorkspace_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Library)
            {
                Library lib = (Library)e.NewValue;
                lsObjects.ItemsSource = new Orca().DirLibrary(lib.Dir + "\\" + lib.File);
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
                new Orca().FillCode(lib);

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
