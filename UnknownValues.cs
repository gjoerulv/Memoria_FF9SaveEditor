using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Memoria.BaseOperations;
using Memoria.PSX;
using System.Text;

namespace Memoria
{
    public partial class UnknownValues : BaseForm
    {
        #region UNKNOWN offsets

        public static byte[] UNKNOWN_CHARACTER_OFFSETS = 
        {
            0x8, 0x9, 0xA, 0x14, 0x15, 0x16, 0x1C, 0x1D, 0x1E,
            0x21, 0x22, 0x23, 0x34, 0x35, 0x36, 0x37, 0x46, 0x47, 0x48,
            0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51,
            0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B
        };

        public static uint[] UNKNOWN_START_OFFSETS = 
        {
            0x100, 0x10E, 0x134, 0x232, 0x25E, 0x3B9, 0xEE4, 0xEEE, 0xEF2, 0x1120, 0x13FA
        };

        public static uint[] UNKNOWN_END_OFFSETS = 
        {
            0x105, 0x110, 0x230, 0x252, 0x3B0, 0x9D0, 0xEE8, 0xEF0, 0xF20, 0x1178, 0x13FE
        };


        #endregion

        private const string HEADER = "Offset   BlockA  BlockB  A - B";
        private uint cardCount = 0;
        private BackgroundWorker bw;
        StringBuilder rep = new StringBuilder();
        int cba, cbb, cbca, cbcb;

        byte[] buffer = new byte[MemCard.MEMORY_CARD_SIZE];
        public UnknownValues(byte[] buffer)
        {
            InitializeComponent();
            Setup();
            textBox1.Font = new Font(new FontFamily("Lucida Console"), fontSize);
            this.Icon = Memoria.Properties.Resources.mem;
            Array.Copy(buffer, this.buffer, buffer.Length);
            cbBlockA.SelectedIndex = cbBlockB.SelectedIndex =
            cbCharA.SelectedIndex = cbCharB.SelectedIndex = 0;
            radioButton_CheckedChanged(null, null);
            bw = new BackgroundWorker();
            bw.DoWork +=new DoWorkEventHandler(DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Completed);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rep = null;
            rep = new StringBuilder();
            textBox1.Text = "";
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = label4.Enabled = cbCharA.Enabled = cbCharB.Enabled = (rbCharMode.Checked | rbCharacterAll.Checked);
        }

        private void AddTextToReport(uint offset, byte blockAValue, byte blockBValue, bool isCardCheck)
        {
            string s = "0x";
            s += Strings.AdjustString(offset.ToString("x"), 4, '0'); s += "   ";
            s += Strings.AdjustString(blockAValue.ToString("x"), 2, ' '); s += "      ";
            s += Strings.AdjustString(blockBValue.ToString("x"), 2, ' '); s += "      ";
            int i = blockAValue - blockBValue; s += i.ToString();
            if (isCardCheck)
            {
                if (cardCount % 6 == 0 || cardCount == 0)
                {
                    if (cardCount > 0)
                        rep.Append(Environment.NewLine);
                    rep.Append(Environment.NewLine + "Card: " + ((cardCount / 6) + 1));
                }
                cardCount++;
            }
            rep.Append(Environment.NewLine + s);
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            btnClear.Enabled = false;
            btnCompare.Enabled = false;
            if (rep.ToString().EndsWith("Comparisson complete!"))
                rep.Append(Environment.NewLine + Environment.NewLine);
            rep.Append(HEADER); rep.Append(Environment.NewLine);
            cba = cbBlockA.SelectedIndex; 
            cbb = cbBlockB.SelectedIndex; 
            cbca = cbCharA.SelectedIndex; 
            cbcb = cbCharB.SelectedIndex;
            bw.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Compare(cba, cbb, cbca, cbcb);
        }

        private void Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            btnClear.Enabled = true;
            btnCompare.Enabled = true;
            textBox1.Text = rep.ToString();
        }

        private void Compare(int cbAindex, int cbBindex ,int cbCAindex, int cbCBindex)
        {
            try
            {
                uint posA = 0, posB = 0, displayOffset = 0;
                //Unknown offsets only (not character values)
                if (rbBlockMode.Checked)
                {
                    int i = UNKNOWN_END_OFFSETS.Length; byte b = 0;
                    do
                    {
                        SetPositions(ref posA, ref posB, ref displayOffset, UNKNOWN_START_OFFSETS[b], cbAindex, cbBindex);
                        uint end = (uint)((MemCard.SAVE_BLOCK_SIZE * (cbAindex + 1)) + UNKNOWN_END_OFFSETS[b]);
                        for(uint j = posA, k = posB; j < end; j++, k++)
                        {
                            if (buffer[j] != buffer[k])
                                AddTextToReport(displayOffset, buffer[j], buffer[k], false);
                            displayOffset++;
                        }
                        b++;
                        i--;
                    }
                    while (i > 0);
                }
                //Only characters
                else if (rbCharMode.Checked)
                {
                    SetPositions(ref posA, ref posB, ref displayOffset, SaveMap.CHARACTER_SECTION_START_OFFSET, cbAindex, cbBindex);
                    posA += (uint)(SaveMap.CHARACTER_BLOCK_SIZE * cbCAindex);
                    posB += (uint)(SaveMap.CHARACTER_BLOCK_SIZE * cbCBindex);
                    displayOffset = 0; int k = 0;
                    for (uint i = posA, j = posB; i < posA + SaveMap.CHARACTER_BLOCK_SIZE; i++, j++)
                    {
                        if (k > UNKNOWN_CHARACTER_OFFSETS.Length - 1)
                            break;
                        if (UNKNOWN_CHARACTER_OFFSETS[k] == displayOffset)
                        {
                            if (buffer[i] != buffer[j])
                                AddTextToReport(displayOffset, buffer[i], buffer[j], false);
                            k++;
                        }
                        displayOffset++;
                    }
                }
                //All data in block
                else if(radioButton1.Checked)
                {
                    SetPositions(ref posA, ref posB, ref displayOffset, 0, cbAindex, cbBindex);
                    for (uint i = posA, j = posB; i < posA + MemCard.SAVE_BLOCK_SIZE; i++, j++)
                    {
                        if (buffer[i] != buffer[j])
                            AddTextToReport(displayOffset, buffer[i], buffer[j], false);
                        displayOffset++;
                    }
                }
                //Compare all character data
                else if (rbCharacterAll.Checked)
                {
                    SetPositions(ref posA, ref posB, ref displayOffset, SaveMap.CHARACTER_SECTION_START_OFFSET, cbAindex, cbBindex);
                    posA += (uint)(SaveMap.CHARACTER_BLOCK_SIZE * cbCAindex);
                    posB += (uint)(SaveMap.CHARACTER_BLOCK_SIZE * cbCBindex);
                    displayOffset = 0;
                    for (uint i = posA, j = posB; i < posA + SaveMap.CHARACTER_BLOCK_SIZE; i++, j++)
                    {
                        AddTextToReport(displayOffset, buffer[i], buffer[j], false);
                        displayOffset++;
                    }
                }
                //Compare all cards
                else
                {
                    cardCount = 0;
                    SetPositions(ref posA, ref posB, ref displayOffset, SaveMap.CARD_START_OFFSET, cbAindex, cbBindex);
                    uint end = (uint)((MemCard.SAVE_BLOCK_SIZE * (cbAindex + 1)) + (MemCard.SAVE_BLOCK_SIZE - 0xC0C));
                    for (uint j = posA, k = posB; j < end; j++, k++)
                    {
                        AddTextToReport(displayOffset, buffer[j], buffer[k], true);
                        displayOffset++;                    
                    }
                }
                rep.Append(Environment.NewLine + Environment.NewLine + "Comparisson complete!");
                GC.Collect(); GC.WaitForPendingFinalizers();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void SetPositions(ref uint posA, ref uint posB, ref uint displayOffset, uint pluss, int cbAindex, int cbBindex)
        {
            posA = (uint)((MemCard.SAVE_BLOCK_SIZE * (cbAindex + 1)) + pluss);
            posB = (uint)((MemCard.SAVE_BLOCK_SIZE * (cbBindex + 1)) + pluss);
            displayOffset = pluss;
        }

    }
}
