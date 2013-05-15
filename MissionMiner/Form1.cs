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
        ActiveButton Shrink = new ActiveButton();
        MissionMinerSettings Config = MissionMiner.Instance.Config;

        public MissionMinerUI()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IActiveMenu aMenu = ActiveMenu.GetInstance(this);
            aMenu.Items.Add(Shrink);
            Shrink.Text = "Shrink";
            Shrink.Click += new EventHandler(Shrink_Click);
            //uiData.GetData(() => this.Invoke(GetUIData));

            LoadSettings();

            LoggerHelper.Instance.Loggers.Where(a => a.Name != "State").ForEach(a => { a.RichEvent += ConsoleUpdate; });
        }

        private void LoadSettings()
        {
            checkLevel1.Checked = Config.Level1;
            checkLevel2.Checked = Config.Level2;
            checkLevel3.Checked = Config.Level3;
            checkLevel4.Checked = Config.Level4;
            checkUnknownMissionHalt.Checked = Config.UnknownMissionHalt;
            checkAlwaysOnTop.Checked = Config.AlwaysOnTop;
            this.TopMost = Config.AlwaysOnTop;
        }

        void Shrink_Click(object sender, EventArgs e)
        {
            if (this.Height == 420)
            {
                this.Height = 70;
                Shrink.Text = "Restore";
            }
            else
            {
                this.Height = 420;
                Shrink.Text = "Shrink";
            }
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


        private void btnDrones_Click(object sender, EventArgs e)
        {
            DroneControl.Instance.Configure();
        }

        private void checkLevel1_CheckedChanged(object sender, EventArgs e)
        {
            Config.Level1 = checkLevel1.Checked;
            Config.Save();
        }

        private void checkLevel2_CheckedChanged(object sender, EventArgs e)
        {
            Config.Level2 = checkLevel2.Checked;
            Config.Save();
        }

        private void checkLevel3_CheckedChanged(object sender, EventArgs e)
        {
            Config.Level3 = checkLevel3.Checked;
            Config.Save();
        }

        private void checkLevel4_CheckedChanged(object sender, EventArgs e)
        {
            Config.Level4 = checkLevel4.Checked;
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
            checkStopOnComplete.Checked = MissionMiner.Instance.StopOnComplete;
            if (MissionMiner.Instance.Idle && checkActive.Checked) checkActive.Checked = false;
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

        private void checkStopOnComplete_CheckedChanged(object sender, EventArgs e)
        {
            MissionMiner.Instance.StopOnComplete = checkStopOnComplete.Checked;
        }

        private void checkActive_CheckedChanged(object sender, EventArgs e)
        {
            if (checkActive.Checked)
            {
                miner.Start();
            }
            else
            {
                miner.Stop();
            }
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
