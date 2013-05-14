using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EveComFramework.DroneControl;
using EveComFramework.Core;
using TheCodeKing.ActiveButtons.Controls;

namespace MissionMiner
{
    public partial class MissionMinerUI : Form
    {
        MissionMiner miner = MissionMiner.Instance;
        MissionMinerUIData uiData = new MissionMinerUIData();
        ActiveButton Active = new ActiveButton();
        MissionMinerSettings Config = MissionMiner.Instance.Config;

        public MissionMinerUI()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IActiveMenu aMenu = ActiveMenu.GetInstance(this);
            aMenu.Items.Add(Active);
            Active.Text = "Start";
            Active.Click += new EventHandler(Active_Click);
            //uiData.GetData(() => this.Invoke(GetUIData));

            LoadSettings();

            LoggerHelper.Instance.Loggers.Where(a => a.Name != "State").ForEach(a => { a.RichEvent += ConsoleUpdate; });
        }

        private void LoadSettings()
        {
            if (Config.Levels.Contains(1)) checkLevel1.Checked = true;
            if (Config.Levels.Contains(2)) checkLevel2.Checked = true;
            if (Config.Levels.Contains(3)) checkLevel3.Checked = true;
            if (Config.Levels.Contains(4)) checkLevel4.Checked = true;
            checkUnknownMissionHalt.Checked = Config.UnknownMissionHalt;
            checkAlwaysOnTop.Checked = Config.AlwaysOnTop;
            this.TopMost = Config.AlwaysOnTop;
        }

        public void GetUIData()
        {
            //cbxAgents.Items.AddRange(uiData.Agents.Select(a => new { Name = a.Key, ID = a.Value }).ToArray());
        }

        delegate void SetConsoleUpdate(string Module, string Message);
        public void ConsoleUpdate(string Module, string Message)
        {
            if (richConsole.InvokeRequired)
            {
                richConsole.BeginInvoke(new SetConsoleUpdate(ConsoleUpdate), Module, Message);
            }
            else
            {
                LoggerHelper.Instance.RichTextboxUpdater(richConsole, Module, Message);
            }
        }

        void Active_Click(object sender, EventArgs e)
        {
            if (miner.Idle)
            {
                miner.Start();
                Active.Text = "Stop";
            }
            else
            {
                miner.Clear();
                Active.Text = "Start";
            }
        }

        private void btnDrones_Click(object sender, EventArgs e)
        {
            DroneControl.Instance.Configure();
        }

        private void checkLevel1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkLevel1.Checked)
            {
                Config.Levels.Add(1);
            }
            else
            {
                Config.Levels.Remove(1);
            }
            Config.Save();
        }

        private void checkLevel2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkLevel2.Checked)
            {
                Config.Levels.Add(2);
            }
            else
            {
                Config.Levels.Remove(2);
            }
            Config.Save();
        }

        private void checkLevel3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkLevel3.Checked)
            {
                Config.Levels.Add(3);
            }
            else
            {
                Config.Levels.Remove(3);
            }
            Config.Save();
        }

        private void checkLevel4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkLevel3.Checked)
            {
                Config.Levels.Add(4);
            }
            else
            {
                Config.Levels.Remove(4);
            }
            Config.Save();
        }

        private void btnAutoModuleConfig_Click(object sender, EventArgs e)
        {
            MissionMiner.Instance.Automodule.Configure();
        }

        private void btnOptimizer_Click(object sender, EventArgs e)
        {
            MissionMiner.Instance.Optimizer.Configure();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MissionMiner.Instance.CurState != null)
            {
                lblState.Text = MissionMiner.Instance.CurState.ToString();
            }
            else
            {
                lblState.Text = "Idle";
            }
        }

        private void checkUnknownMissionHalt_CheckedChanged(object sender, EventArgs e)
        {
            Config.UnknownMissionHalt = checkUnknownMissionHalt.Checked;
            Config.Save();
        }

        private void checkAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            Config.AlwaysOnTop = checkAlwaysOnTop.Checked;
            this.TopMost = Config.AlwaysOnTop;
            Config.Save();
        }

    }

    public static class WindowsFormsControlInvoke
    {
        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle.
        /// </summary>
        /// <param name="control">The control whose window handle the delegate should be invoked on.</param>
        /// <param name="method">A delegate that contains a method to be called in the control's thread context.</param>
        public static void Invoke(this Control control, Action method)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(method);
            }
            else
            {
                method();
            }
        }

        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle, returning a
        /// value.
        /// </summary>
        /// <param name="control">The control whose window handle the delegate should be invoked on.</param>
        /// <param name="method">A delegate that contains a method to be called in the control's thread context and
        /// that returns a value.</param>
        /// <returns>The return value from the delegate being invoked.</returns>
        public static TResult Invoke<TResult>(this Control control, Func<TResult> method)
        {
            if (control.InvokeRequired)
            {
                return (TResult)control.Invoke(method);
            }
            else
            {
                return method();
            }
        }
    }
}
