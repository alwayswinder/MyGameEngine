﻿using PrimalEditor.GameProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace PrimalEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string MyGameEnginePath { get; private set; } = @"D:\MyGameEngine";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }
        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialog();
        }

        private void GetEnginePath()
        {
            var myGameEnginePath = Environment.GetEnvironmentVariable("MYGAME_ENGINE", EnvironmentVariableTarget.User);
            if (myGameEnginePath == null || !Directory.Exists(Path.Combine(myGameEnginePath, @"Engine\Engine\EngineAPI")))
            {
                var dlg = new EnginePathDialog();
                if(dlg.ShowDialog() == true)
                {
                    MyGameEnginePath = dlg.MyGameEnginePath;
                    Environment.SetEnvironmentVariable("MYGAME_ENGINE", MyGameEnginePath.ToUpper(), EnvironmentVariableTarget.User);
                }
            }
            else
            {
                MyGameEnginePath = myGameEnginePath;
            }
        }


        private void OpenProjectBrowserDialog()
        {
            var projectBrowser = new ProjectBrowserDialg();
            if(projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
            }
        }
    }
}
