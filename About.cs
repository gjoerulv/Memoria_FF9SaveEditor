using System;
using System.Windows.Forms;

namespace Memoria
{
    public partial class About : BaseForm
    {
        SelectSave s = null;

        /// <summary>
        /// Decides when splash screen closes.
        /// </summary>
        byte ticks = 0;

        public About(SelectSave s)
        {
            this.s = s;
            this.Icon = Memoria.Properties.Resources.mem;
            InitializeComponent();
            Setup();
            label1.Text = "Memoria v" + Program.Version;
            linkLabel1.Links[0].LinkData = "mailto:gjoerulv@hotmail.com";
            linkLabel2.Links[0].LinkData = "http://forums.qhimm.com/index.php?topic=11494.0";
            if (s != null)
            {
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                button1.Visible = linkLabel1.Visible = linkLabel2.Visible = true;
            }
            else
            {
                this.TopMost = true;
                timer1.Enabled = true;
                timer1.Start();
            }
        }

        private void About_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
            if(s != null)
                s.about = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ticks++;
            if (ticks > 1)
                this.Close();
        }

        private void linkLabe_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string target = e.Link.LinkData as String;
                System.Diagnostics.Process.Start(target);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }
    }
}
