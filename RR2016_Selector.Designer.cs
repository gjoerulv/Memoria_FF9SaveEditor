namespace Memoria
{
    partial class RR2016_Selector
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
            this.btnCloseRR = new System.Windows.Forms.Button();
            this.btnFileSelect = new System.Windows.Forms.Button();
            this.pnlFiles = new System.Windows.Forms.Panel();
            this.pnlSlots = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnCloseRR
            // 
            this.btnCloseRR.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCloseRR.Location = new System.Drawing.Point(197, 226);
            this.btnCloseRR.Name = "btnCloseRR";
            this.btnCloseRR.Size = new System.Drawing.Size(75, 23);
            this.btnCloseRR.TabIndex = 2;
            this.btnCloseRR.Text = "Cancel";
            this.btnCloseRR.UseVisualStyleBackColor = true;
            this.btnCloseRR.Click += new System.EventHandler(this.btnCloseRR_Click);
            // 
            // btnFileSelect
            // 
            this.btnFileSelect.Enabled = false;
            this.btnFileSelect.Location = new System.Drawing.Point(80, 226);
            this.btnFileSelect.Name = "btnFileSelect";
            this.btnFileSelect.Size = new System.Drawing.Size(111, 23);
            this.btnFileSelect.TabIndex = 1;
            this.btnFileSelect.Text = "Select Slots";
            this.btnFileSelect.UseVisualStyleBackColor = true;
            this.btnFileSelect.Click += new System.EventHandler(this.btnFileSelect_Click);
            // 
            // pnlFiles
            // 
            this.pnlFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiles.Location = new System.Drawing.Point(0, 0);
            this.pnlFiles.Name = "pnlFiles";
            this.pnlFiles.Size = new System.Drawing.Size(284, 220);
            this.pnlFiles.TabIndex = 3;
            // 
            // pnlSlots
            // 
            this.pnlSlots.Location = new System.Drawing.Point(12, 87);
            this.pnlSlots.Name = "pnlSlots";
            this.pnlSlots.Size = new System.Drawing.Size(179, 100);
            this.pnlSlots.TabIndex = 4;
            // 
            // RR2016_Selector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCloseRR;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.pnlSlots);
            this.Controls.Add(this.pnlFiles);
            this.Controls.Add(this.btnFileSelect);
            this.Controls.Add(this.btnCloseRR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "RR2016_Selector";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FF9 2016 Rerelease Save Selector";
            this.Load += new System.EventHandler(this.RR2016_Selector_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCloseRR;
        private System.Windows.Forms.Button btnFileSelect;
        private System.Windows.Forms.Panel pnlFiles;
        private System.Windows.Forms.Panel pnlSlots;
    }
}