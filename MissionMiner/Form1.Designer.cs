namespace MissionMiner
{
    partial class Form1
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
            this.btnDrones = new System.Windows.Forms.Button();
            this.cbxAgents = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnDrones
            // 
            this.btnDrones.Location = new System.Drawing.Point(12, 227);
            this.btnDrones.Name = "btnDrones";
            this.btnDrones.Size = new System.Drawing.Size(103, 23);
            this.btnDrones.TabIndex = 0;
            this.btnDrones.Text = "Config Drones";
            this.btnDrones.UseVisualStyleBackColor = true;
            this.btnDrones.Click += new System.EventHandler(this.btnDrones_Click);
            // 
            // cbxAgents
            // 
            this.cbxAgents.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbxAgents.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbxAgents.DisplayMember = "Name";
            this.cbxAgents.FormattingEnabled = true;
            this.cbxAgents.Location = new System.Drawing.Point(99, 12);
            this.cbxAgents.Name = "cbxAgents";
            this.cbxAgents.Size = new System.Drawing.Size(173, 21);
            this.cbxAgents.TabIndex = 1;
            this.cbxAgents.ValueMember = "ID";
            this.cbxAgents.SelectedIndexChanged += new System.EventHandler(this.cbxAgents_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.cbxAgents);
            this.Controls.Add(this.btnDrones);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDrones;
        private System.Windows.Forms.ComboBox cbxAgents;
    }
}

