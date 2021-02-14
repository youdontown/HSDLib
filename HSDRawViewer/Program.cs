﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace HSDRawViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            MainForm.Init();
            ApplicationSettings.Init();
            if(args.Length > 0)
                MainForm.Instance.OpenFile(args[0]);

            Application.Run(MainForm.Instance);
        }
    }
}
