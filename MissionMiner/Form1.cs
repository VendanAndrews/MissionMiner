using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EveComFramework.DroneControl;
using TheCodeKing.ActiveButtons.Controls;

namespace MissionMiner
{
    public partial class Form1 : Form
    {
        MissionMiner miner = new MissionMiner();
        MissionMinerUIData uiData = new MissionMinerUIData();
        ActiveButton Active = new ActiveButton();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IActiveMenu aMenu = ActiveMenu.GetInstance(this);
            aMenu.Items.Add(Active);
            Active.Text = "Start";
            Active.Click += new EventHandler(Active_Click);
            uiData.GetData(() => this.Invoke(GetUIData));
        }

        public void GetUIData()
        {
            cbxAgents.Items.AddRange(uiData.Agents.Select(a => new { Name = a.Key, ID = a.Value }).ToArray());
            
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

        private void cbxAgents_SelectedIndexChanged(object sender, EventArgs e)
        {
            miner.Settings.Agent = cbxAgents.Text;
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
