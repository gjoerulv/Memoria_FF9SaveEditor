using System;
using System.Drawing;
//using System.Linq;
using System.Windows.Forms;

namespace Memoria
{
    public partial class RR2016_Selector : Form
    {
        public int slotID, saveID;
        public RR2016_Selector()
        {
            InitializeComponent();
        }

        private void RR2016_Selector_Load(object sender, EventArgs e)
        {
            btnFileSelect.Enabled = false;
            saveID = -1; slotID = -1;
            int height = this.Height - btnCloseRR.Location.X + 24;
            int width = this.Width - 8;
            pnlFiles.Width = width; pnlSlots.Width = width;
            pnlSlots.Height = height; pnlFiles.Height = height;

            pnlFiles.Location = new Point(0, 0);
            pnlFiles.Size = new Size(width, height);
            pnlSlots.Location = new Point(0, 0);
            pnlSlots.Size = new Size(width, height);

            pnlFiles.Dock = DockStyle.Top; pnlSlots.Dock = DockStyle.Top;
            pnlSlots.Visible = false;
            pnlFiles.Controls.Clear();
            pnlSlots.Controls.Clear();
            if (pnlFiles.Controls.Count < 1)
            {
                int fWidth = pnlFiles.Width / 2 - 8;
                int fHeight = pnlFiles.Height / 5 - 2;
                for(int i = 0; i < 10; i++)
                {
                    Button btnFile = new Button();
                    btnFile.Name = "btnRRFile" + i;
                    btnFile.TabIndex = i;
                    btnFile.Text = "Slot " + (i + 1);
                    btnFile.Tag = i;
                    btnFile.Height = fHeight;
                    btnFile.Width = fWidth;
                    pnlFiles.Controls.Add(btnFile);
                    btnFile.Location = new Point(4 + i % 2 * fWidth, 4 + i / 2 * fHeight);
                    btnFile.Click += onClickBtnRR;
                }
            }

            if (pnlSlots.Controls.Count < 1)
            {
                int fWidth = pnlSlots.Width / 3 - 9;
                int fHeight = pnlSlots.Height / 5 - 2;
                for (int i = 0; i < 15; i++)
                {
                    Button btnFile = new Button();
                    btnFile.Name = "btnRRSlot" + i;
                    btnFile.TabIndex = i;
                    btnFile.Text = "File " + (i + 1);
                    btnFile.Tag = i;
                    btnFile.Height = fHeight;
                    btnFile.Width = fWidth;
                    pnlSlots.Controls.Add(btnFile);
                    btnFile.Location = new Point(3 + i % 3 * fWidth, 4 + i / 3 * fHeight);
                    btnFile.Click += onClickBtnRR;
                }
            }
        }

        private void onClickBtnRR(object sender, EventArgs e)
        {
            //slot max = 9 save max = 14
            int id = Convert.ToInt32((sender as Button).Tag);
            if ((sender as Button).Text[0] == 'S')
            {
                slotID = id;
                SwitchMode(false);
            }
            else
            {
                saveID = id;
                Close();
            }
        }

        private void btnCloseRR_Click(object sender, EventArgs e)
        {
            slotID = -1; saveID = -1;
            Close();
        }

        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            SwitchMode(true);
        }

        private void SwitchMode(bool fileMode)
        {
            pnlSlots.Visible = !fileMode;
            pnlFiles.Visible = fileMode;
            btnFileSelect.Enabled = !fileMode;

        }
    }
}
