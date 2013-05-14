namespace MissionMiner
{
    partial class MissionMinerUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnDrones = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richConsole = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnOptimizer = new System.Windows.Forms.Button();
            this.btnAutoModuleConfig = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkLevel4 = new System.Windows.Forms.CheckBox();
            this.checkLevel3 = new System.Windows.Forms.CheckBox();
            this.checkLevel2 = new System.Windows.Forms.CheckBox();
            this.checkLevel1 = new System.Windows.Forms.CheckBox();
            this.lblState = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDrones
            // 
            this.btnDrones.Location = new System.Drawing.Point(6, 283);
            this.btnDrones.Name = "btnDrones";
            this.btnDrones.Size = new System.Drawing.Size(390, 23);
            this.btnDrones.TabIndex = 0;
            this.btnDrones.Text = "Config Drones";
            this.btnDrones.UseVisualStyleBackColor = true;
            this.btnDrones.Click += new System.EventHandler(this.btnDrones_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(410, 338);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblState);
            this.tabPage1.Controls.Add(this.richConsole);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(402, 312);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Console";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richConsole
            // 
            this.richConsole.BackColor = System.Drawing.Color.Black;
            this.richConsole.ForeColor = System.Drawing.Color.White;
            this.richConsole.Location = new System.Drawing.Point(6, 26);
            this.richConsole.Name = "richConsole";
            this.richConsole.Size = new System.Drawing.Size(390, 280);
            this.richConsole.TabIndex = 0;
            this.richConsole.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnOptimizer);
            this.tabPage2.Controls.Add(this.btnAutoModuleConfig);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.btnDrones);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(402, 312);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnOptimizer
            // 
            this.btnOptimizer.Location = new System.Drawing.Point(6, 225);
            this.btnOptimizer.Name = "btnOptimizer";
            this.btnOptimizer.Size = new System.Drawing.Size(390, 23);
            this.btnOptimizer.TabIndex = 3;
            this.btnOptimizer.Text = "Config Optimizer";
            this.btnOptimizer.UseVisualStyleBackColor = true;
            this.btnOptimizer.Click += new System.EventHandler(this.btnOptimizer_Click);
            // 
            // btnAutoModuleConfig
            // 
            this.btnAutoModuleConfig.Location = new System.Drawing.Point(6, 254);
            this.btnAutoModuleConfig.Name = "btnAutoModuleConfig";
            this.btnAutoModuleConfig.Size = new System.Drawing.Size(390, 23);
            this.btnAutoModuleConfig.TabIndex = 2;
            this.btnAutoModuleConfig.Text = "Config AutoModule";
            this.btnAutoModuleConfig.UseVisualStyleBackColor = true;
            this.btnAutoModuleConfig.Click += new System.EventHandler(this.btnAutoModuleConfig_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkLevel4);
            this.groupBox1.Controls.Add(this.checkLevel3);
            this.groupBox1.Controls.Add(this.checkLevel2);
            this.groupBox1.Controls.Add(this.checkLevel1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(390, 56);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mission Levels";
            // 
            // checkLevel4
            // 
            this.checkLevel4.AutoSize = true;
            this.checkLevel4.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkLevel4.Location = new System.Drawing.Point(339, 20);
            this.checkLevel4.Name = "checkLevel4";
            this.checkLevel4.Size = new System.Drawing.Size(44, 31);
            this.checkLevel4.TabIndex = 3;
            this.checkLevel4.Text = "Level 4";
            this.checkLevel4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkLevel4.UseVisualStyleBackColor = true;
            this.checkLevel4.CheckedChanged += new System.EventHandler(this.checkLevel4_CheckedChanged);
            // 
            // checkLevel3
            // 
            this.checkLevel3.AutoSize = true;
            this.checkLevel3.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkLevel3.Location = new System.Drawing.Point(228, 20);
            this.checkLevel3.Name = "checkLevel3";
            this.checkLevel3.Size = new System.Drawing.Size(44, 31);
            this.checkLevel3.TabIndex = 2;
            this.checkLevel3.Text = "Level 3";
            this.checkLevel3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkLevel3.UseVisualStyleBackColor = true;
            this.checkLevel3.CheckedChanged += new System.EventHandler(this.checkLevel3_CheckedChanged);
            // 
            // checkLevel2
            // 
            this.checkLevel2.AutoSize = true;
            this.checkLevel2.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkLevel2.Location = new System.Drawing.Point(117, 20);
            this.checkLevel2.Name = "checkLevel2";
            this.checkLevel2.Size = new System.Drawing.Size(44, 31);
            this.checkLevel2.TabIndex = 1;
            this.checkLevel2.Text = "Level 2";
            this.checkLevel2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkLevel2.UseVisualStyleBackColor = true;
            this.checkLevel2.CheckedChanged += new System.EventHandler(this.checkLevel2_CheckedChanged);
            // 
            // checkLevel1
            // 
            this.checkLevel1.AutoSize = true;
            this.checkLevel1.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkLevel1.Location = new System.Drawing.Point(6, 20);
            this.checkLevel1.Name = "checkLevel1";
            this.checkLevel1.Size = new System.Drawing.Size(44, 31);
            this.checkLevel1.TabIndex = 0;
            this.checkLevel1.Text = "Level 1";
            this.checkLevel1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.checkLevel1.UseVisualStyleBackColor = true;
            this.checkLevel1.CheckedChanged += new System.EventHandler(this.checkLevel1_CheckedChanged);
            // 
            // lblState
            // 
            this.lblState.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblState.Location = new System.Drawing.Point(6, 3);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(390, 20);
            this.lblState.TabIndex = 1;
            this.lblState.Text = "State";
            this.lblState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MissionMinerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 362);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MissionMinerUI";
            this.Text = "Mission Miner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDrones;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox richConsole;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkLevel4;
        private System.Windows.Forms.CheckBox checkLevel3;
        private System.Windows.Forms.CheckBox checkLevel2;
        private System.Windows.Forms.CheckBox checkLevel1;
        private System.Windows.Forms.Button btnAutoModuleConfig;
        private System.Windows.Forms.Button btnOptimizer;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Timer timer1;
    }
}

