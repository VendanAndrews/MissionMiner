using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EveComFramework.Core;

namespace MissionMiner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Config.Instance.DefaultProfile = args[0];
            }
            else
            {
                Config.Instance.DefaultProfile = "MissionMiner-Settings";
            } 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MissionMinerUI());
        }
    }
}
