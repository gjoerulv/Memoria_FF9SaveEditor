using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memoria
{
    public partial class Slot : BaseForm
    {
        public Slot(string title, Bitmap bitmap, string productCode, string gameID, char np)
        {
            InitializeComponent();
            Setup();

            // panel1
            panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panel1.Location = new System.Drawing.Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(64, 64);
            panel1.TabIndex = 2;
            this.Icon = Memoria.Properties.Resources.mem;
            if (bitmap != null)
                panel1.BackgroundImage = bitmap;
            //end panel1

            panel2.Height = CalcNewValue(panel2.Height);

            label1.Text = title;
            label2.Text = "Product Code: " + productCode;
            label3.Text = "Game ID: " + gameID;
            if (np == 'A')
                label4.Text = "NTSC (US)";
            else if (np == 'E')
                label4.Text = "PAL";
            else if (np == 'I')
                label4.Text = "NTCS (JA)";
            else
                label4.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
