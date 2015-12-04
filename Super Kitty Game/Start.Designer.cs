namespace Super_Kitty_Game
{
    partial class Start
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
            this.btnStart = new System.Windows.Forms.Button();
            this.MasterCheckBox = new System.Windows.Forms.CheckBox();
            this.tbMasterIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(284, 22);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // MasterCheckBox
            // 
            this.MasterCheckBox.AutoSize = true;
            this.MasterCheckBox.Location = new System.Drawing.Point(12, 12);
            this.MasterCheckBox.Name = "MasterCheckBox";
            this.MasterCheckBox.Size = new System.Drawing.Size(64, 17);
            this.MasterCheckBox.TabIndex = 1;
            this.MasterCheckBox.Text = "Master?";
            this.MasterCheckBox.UseVisualStyleBackColor = true;
            this.MasterCheckBox.CheckedChanged += new System.EventHandler(this.MasterCheckBox_CheckedChanged);
            // 
            // tbMasterIP
            // 
            this.tbMasterIP.Location = new System.Drawing.Point(76, 35);
            this.tbMasterIP.Name = "tbMasterIP";
            this.tbMasterIP.Size = new System.Drawing.Size(100, 20);
            this.tbMasterIP.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Master IP";
            // 
            // Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 79);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMasterIP);
            this.Controls.Add(this.MasterCheckBox);
            this.Controls.Add(this.btnStart);
            this.Name = "Start";
            this.Text = "Start";
            this.Load += new System.EventHandler(this.Start_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox MasterCheckBox;
        private System.Windows.Forms.TextBox tbMasterIP;
        private System.Windows.Forms.Label label1;
    }
}