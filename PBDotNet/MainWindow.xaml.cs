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
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace PBDotNet
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Workspace workspace;
        private Orca.Version pbVersion;
        private IHighlightingDefinition highlightingDefinition = null;

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

            using (Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PBDotNet.PBSyntax.xshd")) {
                using (XmlTextReader reader = new XmlTextReader(s)) {
                    highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            txtSource.SyntaxHighlighting = this.highlightingDefinition;
            txtUoSource.SyntaxHighlighting = this.highlightingDefinition;
            txtUoIVariables.SyntaxHighlighting = this.highlightingDefinition;
            txtUoSVariables.SyntaxHighlighting = this.highlightingDefinition;
            txtUoGVariables.SyntaxHighlighting = this.highlightingDefinition;
            txtUoExtFunctions.SyntaxHighlighting = this.highlightingDefinition;
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

            dialog.Filter = "Powerbuilder Library (*.pbl)|*.pbl";

            if (!dialog.ShowDialog().Value)
                return;

            this.openLibrary(dialog.FileName);
        }

        private void openLibrary(string libraryPath) {
            ILibrary lib = null;
            if (libraryPath.EndsWith(".pbl"))
                lib = new Library(libraryPath, this.pbVersion);
            else
                lib = new VirtualLibrary(libraryPath);

            listPowerObjects.ItemsSource = lib.EntryList;

            listWorkspace.DataContext = null;
            listWorkspace.Visibility = System.Windows.Visibility.Collapsed;
            // TODO: Workspace ist jetzt ein Dock
            //gridWorkspace.Width = new GridLength(0);
        }

        private void openWorkspace(string workspacePath) {
            workspace = new Workspace(workspacePath, this.pbVersion);

            stWorkspace.Text = workspacePath;
            stVersion.Text = workspace.MajorVersion + "." + workspace.MinorVersion;

            listWorkspace.DataContext = workspace.Targets;
            listWorkspace.Visibility = System.Windows.Visibility.Visible;
            // TODO: Workspace ist jetzt ein Dock
            //gridWorkspace.Width = new GridLength(300);
        }

        private void listWorkspace_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Library)
            {
                Library lib = (Library)e.NewValue;
                List<LibEntry> libs = new Orca(this.pbVersion).DirLibrary(lib.Dir + "\\" + lib.File);
                List<LibEntryView> libsViews = new List<LibEntryView>();

                foreach(LibEntry l in libs) libsViews.Add(new LibEntryView(l));

                listPowerObjects.ItemsSource = libsViews;
            }
        }

        private void listPowerObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PBDotNetLib.orca.ILibEntry lib;
            LibEntryView libView = null;
            TypeView typeView = null;
            Powerscript.Type[] types;
            Powerscript.Datawindow dw;

            if (e.NewValue is LibEntryView) {
                libView = (LibEntryView)e.NewValue;
                typeView = libView.Type;
            } else if (e.NewValue is TypeView) {
                typeView = (TypeView)e.NewValue;
                libView = (typeView).Parent;
            }
            
            if (libView != null)
            {
                lib = libView.LibEntry;

                txtSource.TextArea.Document.Text = lib.Source ?? "";
                
                switch (lib.Type)
                {
                    case Objecttype.Datawindow:                        
                        tabUserobject.Visibility = System.Windows.Visibility.Collapsed;
                        tabDatawindow.Visibility = System.Windows.Visibility.Visible;

                        dw = Powerscript.Datawindow.GetDatawindowFromSource(lib.Source);
                        txtDwRelease.Text = dw.Release;

                        if (dw.Object != null) {
                            listDatawindowObjs.ItemsSource = dw.Object.Childs;
                        }

                        break;
                    case Objecttype.Structure:
                        tabUserobject.Header = "Structure";
                        goto case Objecttype.Window;
                    case Objecttype.Menu:
                        tabUserobject.Header = "Menu";
                        goto case Objecttype.Window;
                    case Objecttype.Function:
                        tabUserobject.Header = "Function";
                        goto case Objecttype.Window;
                    case Objecttype.Application:
                        tabUserobject.Header = "Application";
                        goto case Objecttype.Window;
                    case Objecttype.Userobject:
                        tabUserobject.Header = "UserObject";
                        goto case Objecttype.Window;
                    case Objecttype.Window:
                        tabUserobject.Visibility = System.Windows.Visibility.Visible;
                        tabDatawindow.Visibility = System.Windows.Visibility.Collapsed;

                        if(lib.Type == Objecttype.Window)
                            tabUserobject.Header = "Window";

                        types = Powerscript.Type.GetTypesFromSource(lib.Source);
                        if (types == null)
                        {
                            MessageBox.Show("keine Types");
                            return;
                        }

                        gridUoTypes.ItemsSource = types;                        

                        if (libView.Types == null) {
                            List<TypeView> typeViews = new List<TypeView>();
                            foreach (Powerscript.Type t in types) {
                                if (t.Name == libView.Name){
                                    libView.Type = new TypeView(t, libView);
                                    typeView = libView.Type;
                                }else{
                                    typeViews.Add(new TypeView(t, libView));
                                }
                            }
                            libView.Types = typeViews;
                        }

                        gridUoProps.ItemsSource = typeView.Type.Properties;

                        if (types.Length > 0)
                        {
                            txtUoIVariables.TextArea.Document.Text = types[0].InstanceVariables ?? "" ;
                            txtUoSVariables.TextArea.Document.Text = types[0].SharedVariables ?? "";
                            txtUoGVariables.TextArea.Document.Text = types[0].GlobalVariables ?? "";
                            txtUoExtFunctions.TextArea.Document.Text = types[0].ExternalFunctions ?? "";
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

            txtUoSource.TextArea.Document.Text = ((Powerscript.Event[])gridUoEvents.ItemsSource)[gridUoEvents.SelectedIndex].Source;
        }

        private void gridUoTypes_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void gridUoMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridUoMethods.SelectedIndex < 0)
                return;

            txtUoSource.TextArea.Document.Text = ((Powerscript.Method[])gridUoMethods.ItemsSource)[gridUoMethods.SelectedIndex].Source;
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

        private void menuOpenFolder_Click(object sender, RoutedEventArgs e) {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK){
                this.openLibrary(dialog.SelectedPath);
            }
        }

        private class TypeView {
            public LibEntryView parent;
            private Powerscript.Type type;

            public Powerscript.Type Type {
                get {
                    return this.type;
                }
            }

            public LibEntryView Parent {
                get {
                    return this.parent;
                }
            }

            public string Name {
                get {
                    return this.type.Name;
                }
            }

            public TypeView(Powerscript.Type type, LibEntryView parent) {
                this.type = type;
                this.parent = parent;
            }            
        }

        
        private class LibEntryView : INotifyPropertyChanged {
            private ILibEntry libEntry;
            private IEnumerable<TypeView> types;
            public TypeView Type { get; set; }

            public LibEntryView(ILibEntry libEntry) {
                this.libEntry = libEntry;
            }

            public ILibEntry LibEntry {
                get {
                    return this.libEntry;
                }
            }

            public string Name {
                get {
                    return this.libEntry.Name;
                }
            }

            public IEnumerable<TypeView> Types {
                get {
                    return this.types;
                }
                set{
                    this.types = value;
                    
                    if (this.PropertyChanged != null) {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Types"));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
