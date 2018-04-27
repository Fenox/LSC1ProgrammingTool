﻿using NLog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace LSC1DatabaseEditor
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetLogger("Usage");
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //if(Debugger.IsAttached)
            //    LSC1UserSettings.Instance.Context.Clear();

            //Task t = new Task(() => OfflineDatabase.UpdateAll(LSC1UserSettings.Instance.DBSettings));
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Fatal(e.ExceptionObject as Exception, "Fatal crash from {0}");
        }
    }
}
