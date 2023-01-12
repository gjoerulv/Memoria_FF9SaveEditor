namespace Memoria
{
    partial class UnknownValues
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
            this.btnClose = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rbCharMode = new System.Windows.Forms.RadioButton();
            this.rbBlockMode = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbCards = new System.Windows.Forms.RadioButton();
            this.rbCharacterAll = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.cbBlockA = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbBlockB = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCharA = new System.Windows.Forms.ComboBox();
            this.cbCharB = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(479, 280);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(67, 27);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBox1.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(298, 315);
            this.textBox1.TabIndex = 0;
            // 
            // rbCharMode
            // 
            this.rbCharMode.AutoSize = true;
            this.rbCharMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbCharMode.Location = new System.Drawing.Point(6, 21);
            this.rbCharMode.Name = "rbCharMode";
            this.rbCharMode.Size = new System.Drawing.Size(111, 19);
            this.rbCharMode.TabIndex = 2;
            this.rbCharMode.Text = "Characters Only";
            this.rbCharMode.UseVisualStyleBackColor = true;
            this.rbCharMode.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // rbBlockMode
            // 
            this.rbBlockMode.AutoSize = true;
            this.rbBlockMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbBlockMode.Location = new System.Drawing.Point(6, 46);
            this.rbBlockMode.Name = "rbBlockMode";
            this.rbBlockMode.Size = new System.Drawing.Size(153, 19);
            this.rbBlockMode.TabIndex = 3;
            this.rbBlockMode.Text = "Block, no character data";
            this.rbBlockMode.UseVisualStyleBackColor = true;
            this.rbBlockMode.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbCards);
            this.groupBox1.Controls.Add(this.rbCharacterAll);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.rbCharMode);
            this.groupBox1.Controls.Add(this.rbBlockMode);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.groupBox1.Location = new System.Drawing.Point(307, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compare mode";
            // 
            // rbCards
            // 
            this.rbCards.AutoSize = true;
            this.rbCards.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbCards.Location = new System.Drawing.Point(164, 71);
            this.rbCards.Name = "rbCards";
            this.rbCards.Size = new System.Drawing.Size(56, 19);
            this.rbCards.TabIndex = 6;
            this.rbCards.Text = "Cards";
            this.rbCards.UseVisualStyleBackColor = true;
            this.rbCards.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // rbCharacterAll
            // 
            this.rbCharacterAll.AutoSize = true;
            this.rbCharacterAll.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rbCharacterAll.Location = new System.Drawing.Point(6, 71);
            this.rbCharacterAll.Name = "rbCharacterAll";
            this.rbCharacterAll.Size = new System.Drawing.Size(123, 19);
            this.rbCharacterAll.TabIndex = 5;
            this.rbCharacterAll.Text = "All Character Data";
            this.rbCharacterAll.UseVisualStyleBackColor = true;
            this.rbCharacterAll.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.radioButton1.Location = new System.Drawing.Point(151, 21);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(67, 19);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "All data";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // cbBlockA
            // 
            this.cbBlockA.FormattingEnabled = true;
            this.cbBlockA.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cbBlockA.Location = new System.Drawing.Point(369, 133);
            this.cbBlockA.Name = "cbBlockA";
            this.cbBlockA.Size = new System.Drawing.Size(52, 23);
            this.cbBlockA.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(304, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "BlockA";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(427, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "BlockB";
            // 
            // cbBlockB
            // 
            this.cbBlockB.FormattingEnabled = true;
            this.cbBlockB.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cbBlockB.Location = new System.Drawing.Point(494, 132);
            this.cbBlockB.Name = "cbBlockB";
            this.cbBlockB.Size = new System.Drawing.Size(52, 23);
            this.cbBlockB.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(304, 173);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "CharA";
            // 
            // cbCharA
            // 
            this.cbCharA.FormattingEnabled = true;
            this.cbCharA.Items.AddRange(new object[] {
            "Zidane",
            "Vivi",
            "Dagger",
            "Steiner",
            "Freya",
            "Quina",
            "Eiko",
            "Amarant",
            "Beatrix"});
            this.cbCharA.Location = new System.Drawing.Point(360, 170);
            this.cbCharA.Name = "cbCharA";
            this.cbCharA.Size = new System.Drawing.Size(109, 23);
            this.cbCharA.TabIndex = 4;
            // 
            // cbCharB
            // 
            this.cbCharB.FormattingEnabled = true;
            this.cbCharB.Items.AddRange(new object[] {
            "Zidane",
            "Vivi",
            "Dagger",
            "Steiner",
            "Freya",
            "Quina",
            "Eiko",
            "Amarant",
            "Beatrix"});
            this.cbCharB.Location = new System.Drawing.Point(360, 199);
            this.cbCharB.Name = "cbCharB";
            this.cbCharB.Size = new System.Drawing.Size(109, 23);
            this.cbCharB.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(304, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "CharB";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(310, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 15);
            this.label5.TabIndex = 13;
            this.label5.Text = "CharA loads from BlockA";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(310, 255);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "CharB loads from BlockB";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(307, 280);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(76, 27);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(389, 280);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(84, 27);
            this.btnCompare.TabIndex = 7;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // UnknownValues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(558, 315);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbCharB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbCharA);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbBlockB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbBlockA);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UnknownValues";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Uknown Values Report";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton rbCharMode;
        private System.Windows.Forms.RadioButton rbBlockMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbBlockA;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbBlockB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbCharA;
        private System.Windows.Forms.ComboBox cbCharB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton rbCards;
        private System.Windows.Forms.RadioButton rbCharacterAll;
    }
}