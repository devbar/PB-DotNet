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
using PBDotNetLib.orca;

namespace PBGenerator {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Browse_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true) {
                Path.Text = openFileDialog.FileName;
            }
        }

        private void Generate_OnClick(object sender, RoutedEventArgs e)
        {
            Orca.Version version;
            switch (PbVersion.Text) {
                case "PB 11.5":
                    version = Orca.Version.PB115;
                    break;
                case "PB 12.5":
                    version = Orca.Version.PB125;
                    break;
                case "PB 12.6":
                    version = Orca.Version.PB126;
                    break;
                default:
                    MessageBox.Show("Please select PB version!");
                    return;
            }

            var orca = new Orca(version);
            orca.CreateDynamicLibrary(Path.Text, "");

            MessageBox.Show("Done!");
        }
    }
}
