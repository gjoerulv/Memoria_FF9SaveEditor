using System;
using System.Drawing;
using System.Windows.Forms;
using Memoria.BaseOperations;

namespace Memoria
{
    public sealed partial class HexEdit : BaseForm
    {

        byte[] buffer; int startOffset, count, startingCell, endingCell;
        const string LEGAL_CHARS = "0123456789ABCDEF";
        bool loaded = false, deleting = false;
        object prevValue = null, prevAValue = null;
        byte block = 0;
        char[] trimChars = { 'x', '0' };
        SelectSave selectSave;

        public HexEdit(int startOffset, int count, SelectSave selectSave) 
        {
            this.buffer = new byte[selectSave.mainBufferCopy.Length];
            selectSave.saveForm.editBuffer.CopyTo(this.buffer, 0);
            this.startOffset = startOffset;
            this.count = count;
            startingCell = startOffset % 16;
            endingCell = (startOffset + count) % 16;
            if(endingCell == 0) endingCell = 16;
            InitializeComponent();
            panel2.Height = CalcNewValue(panel2.Height);
            this.selectSave = selectSave;
            block = this.selectSave.saveForm.currentBlock;
            FillGrid();
            loaded = true;
            
            

            Setup();
            if (oss[0])
            {
                this.Icon = Memoria.Properties.Resources.mem;
            }
            else
            {
                this.Icon = null;
                this.ShowIcon = false;
            }

            int defaultCalc = (int)(fontSize * fontSize);
            int change = defaultCalc * (100 - (int)fontSize);
            //gwEdit.RowHeadersWidth = defaultCalc + (change / 100);
            gwEdit.RowHeadersWidth = (int)(fontSize * 12.0f);
            gwEdit.Font = new Font(new FontFamily("Lucida Console"), 
                fontSize - (fontSize / 10.0f) - 0.1f);
            gwASCII.Font = new Font(new FontFamily("Lucida Console"), 
                fontSize - (fontSize / 2.0f / 10.0f) - 0.1f);
            int totalWidth = gwEdit.RowHeadersWidth;
            foreach (DataGridViewColumn col in gwEdit.Columns)
            {
                change = -40 + defaultCalc;
                change = 100 + change;
                col.Width = col.Width * change / 100;
                totalWidth += col.Width;
            }
            gwEdit.Width = totalWidth + 1;
            totalWidth = 0;
            foreach (DataGridViewColumn col in gwASCII.Columns)
            {
                change = -45 + defaultCalc;
                change = 100 + change;
                col.Width = col.Width * change / 100;
                totalWidth += col.Width;
            }
            gwASCII.Width = totalWidth + gwASCII.Columns[0].Width + 8;
            this.Width = gwASCII.Width + gwEdit.Width + 36;
            HexEdit_Resize_1(null, null);
        }

        private void FillGrid()
        {
            if(buffer == null) return;

            //Number cells not editable.
            int redundantCells = (startingCell + (16 - endingCell));
            //Total cells displayed.
            int totalDisplayedCells = count + redundantCells;
            //Calculate rows in gridview.
            int numberOfRows = (totalDisplayedCells / 16);// +startingCell > 0 ? 1 : 0;
            //Fill gridview with empty rows.
            for(int i = 0, j = startOffset - startingCell; i < numberOfRows; i++, j += 16)
            {
                gwEdit.Rows.Add();
                gwASCII.Rows.Add();
                gwEdit.Rows[i].HeaderCell.Value = FormatRowHeaderText(j);
            }

            int cellIndex, rowIndex;
            for(int i = 0, j = startOffset - startingCell; i < totalDisplayedCells; i++, j++)
            {
                cellIndex = i % 16; rowIndex = i / 16;
                if (j < buffer.Length)
                {
                    gwEdit[cellIndex, rowIndex].Value = FormatCellContent(buffer[j]);
                    gwASCII[cellIndex, rowIndex].Value = (char)buffer[j];
                }

                if(i < startingCell || i >= count + startingCell)
                {
                    gwEdit[cellIndex, rowIndex].ReadOnly = true;
                    gwEdit[cellIndex, rowIndex].Style.BackColor = Color.DimGray;
                    gwASCII[cellIndex, rowIndex].ReadOnly = true;
                    gwASCII[cellIndex, rowIndex].Style.BackColor = Color.DimGray;
                }
            }
        }

        private void ChangeOffsetValue(int offset, byte value)
        {
            if(offset >= buffer.Length || offset < 0) return;
            buffer[offset] = value;
        }

        private string FormatRowHeaderText(int offset)
        {
            if(selectSave.saveForm.IsNormalMemCard)
                offset -= PSX.MemCard.SAVE_BLOCK_SIZE * block;
            string ret = offset.ToString("x").ToUpper();
            while(ret.Length < 5) ret = "0" + ret;
            return "0x" + ret;
        }

        private string FormatCellContent(byte value)
        {
            string s = value.ToString("x").ToUpper();
            if(s.Length < 2) s = "0" + s;
            return s;
        }

        private bool ValidateInputObject(object o)
        {
            if(o == null) return false;
            if(o.ToString() == "") return false;
            return true;
        }

        private void gwEnterCell(object sender, DataGridViewCellEventArgs e)
        {
            if(!loaded) return;
            gwASCII.ClearSelection(); gwEdit.ClearSelection();
            gwASCII[e.ColumnIndex, e.RowIndex].Selected = true;
            gwEdit[e.ColumnIndex, e.RowIndex].Selected = true;
            gwEdit.CurrentCell = gwEdit[e.ColumnIndex, e.RowIndex];
            gwASCII.CurrentCell = gwASCII[e.ColumnIndex, e.RowIndex];
            gwASCII.Update(); gwEdit.Update();
            string s = gwEdit.CurrentCell.OwningRow.HeaderCell.Value.ToString();//.Trim(trimChars);
            string s2 = s.EndsWith("00") ? "0" : "";
            s = s.Trim(trimChars);
            s += s2 + gwEdit.CurrentCell.OwningColumn.HeaderText;
            int i = Int32.Parse(s, System.Globalization.NumberStyles.HexNumber);
            label1.Text = "0x" + s + "/" + i;
        }

        private void gwEdit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(!loaded) return;
            object o = gwEdit[e.ColumnIndex, e.RowIndex].Value;
            if(!ValidateInputObject(o))
                goto fail;
            string s = o.ToString(); s = s.ToUpper();
            foreach(char c in s)
            {
                if(!LEGAL_CHARS.Contains(c.ToString()))
                    goto fail;
            }
            gwASCII[e.ColumnIndex, e.RowIndex].Value = (char)byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
            if(s.Length < 2) s = "0" + s;
            gwEdit[e.ColumnIndex, e.RowIndex].Value = s;
            return;
            fail: gwEdit[e.ColumnIndex, e.RowIndex].Value = prevValue;            
        }

        private void gwEdit_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(!loaded) return;
            prevValue = gwEdit[e.ColumnIndex, e.RowIndex].Value;
            //begunEdit = true;
            gwEdit[e.ColumnIndex, e.RowIndex].Value = "";
        }

        private void gwASCII_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(!loaded) return;
            object o = gwASCII[e.ColumnIndex, e.RowIndex].Value;
            if(!ValidateInputObject(o))
            {
                gwASCII[e.ColumnIndex, e.RowIndex].Value = prevAValue;
                return;
            }
            string s = o.ToString(); 
            gwEdit[e.ColumnIndex, e.RowIndex].Value = FormatCellContent((byte)s[0]);
        }

        private void gwASCII_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(!loaded) return;
            prevAValue = gwASCII[e.ColumnIndex, e.RowIndex].Value;
            //gwASCII[e.ColumnIndex, e.RowIndex].Value = "";
        }

        private void gwDirtyChange(object sender, EventArgs e)
        {
            if(!loaded) return;
            if(!(sender as DataGridView).CurrentCell.IsInEditMode) return;
            if((sender as DataGridView).CurrentCell.ReadOnly) return;
            //if(begunEdit) { begunEdit = false; return; }
            object o = (sender as DataGridView).CurrentCell.Value;
            if(o == null) return;
            string s = o.ToString();
            if((((sender as DataGridView).CurrentCell) as DataGridViewTextBoxCell).MaxInputLength == s.Length)
                NextCell((sender as DataGridView).CurrentCell.ColumnIndex,
                         (sender as DataGridView).CurrentCell.RowIndex, 
                         (sender as DataGridView).RowCount);
        }

        private void gwKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                int ci = (sender as DataGridView).CurrentCell.ColumnIndex;
                int ri = (sender as DataGridView).CurrentCell.RowIndex;
                if(e.KeyData == Keys.Back)
                    deleting = true;
                else
                {
                    DeleteCurrentCell(ci, ri, sender);
                    NextCell(ci, ri, (sender as DataGridView).RowCount);
                }
                return;

            }
            else
                deleting = false;

            if(e.KeyData == Keys.Right || e.KeyData == Keys.Left)
            {
                int ci = (sender as DataGridView).CurrentCell.ColumnIndex;
                int ri = (sender as DataGridView).CurrentCell.RowIndex;
                if(e.KeyData == Keys.Right && ci == 15)
                {
                    e.Handled = true;
                    NextCell(ci, ri, (sender as DataGridView).RowCount);
                }
                else if(e.KeyData == Keys.Left && ci == 0)
                {
                    e.Handled = true;
                    PrevCell(ci, ri);
                }
                return;
            }
        }

        private void gwKeyPress(object sender, KeyPressEventArgs e)
        {
            //Deleting: Backspace was pushed
            if(deleting)
            {
                int ci = (sender as DataGridView).CurrentCell.ColumnIndex;
                int ri = (sender as DataGridView).CurrentCell.RowIndex;
                DeleteCurrentCell(ci, ri, sender);
                PrevCell(ci, ri);
            }
        }

        private void DeleteCurrentCell(int ci, int ri, object sender)
        {
            if(!(sender as DataGridView).CurrentCell.ReadOnly)
            {
                gwEdit[ci, ri].Value = "00";
                gwASCII[ci, ri].Value = '\x0';
            }
        }

        /// <summary>
        /// Set next cell active.
        /// </summary>
        /// <param name="ci">Column index.</param>
        /// <param name="ri">Row index.</param>
        /// <param name="rc">Row Count, number of rows.</param>
        private void NextCell(int ci, int ri, int rc)
        {
            if(ci < 15) ci++;
            else if(ci == 15 && ri < (rc - 1)) { ci = 0; ri++; }
            else return;
            SetActiveCell(ci, ri);
        }

        /// <summary>
        /// Set previous cell active.
        /// </summary>
        /// <param name="ci">Column index.</param>
        /// <param name="ri">Row index.</param>
        private void PrevCell(int ci, int ri)
        {
            if(ci > 0) ci--;
            else if(ci == 0 && ri > 0) { ci = 15; ri--; }
            else return;
            SetActiveCell(ci, ri);
        }

        /// <summary>
        /// Set active cell based on column and row index.
        /// </summary>
        /// <param name="ci">Column index.</param>
        /// <param name="ri">Row index.</param>
        private void SetActiveCell(int ci, int ri)
        {
            gwEdit.CurrentCell = gwEdit[ci, ri];
            gwASCII.CurrentCell = gwASCII[ci, ri];
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            int offset = startOffset;       
            foreach(DataGridViewRow row in gwEdit.Rows)
            {
                foreach(DataGridViewCell cell in row.Cells)
                {
                    if(cell.ReadOnly) continue;
                    ChangeOffsetValue(offset, byte.Parse(cell.Value.ToString(), 
                                      System.Globalization.NumberStyles.HexNumber));
                    offset++;
                }
            }
            selectSave.CopyBuffers(buffer, selectSave.saveForm.editBuffer);
            selectSave.saveForm.EnableApply();
            this.Close();
        }

        private void gwASCII_Scroll(object sender, ScrollEventArgs e)
        {
            gwEdit.FirstDisplayedScrollingRowIndex = gwASCII.FirstDisplayedScrollingRowIndex;
        }

        private void HexEdit_Resize_1(object sender, EventArgs e)
        {
            panel1.Height = this.Height - panel2.Height - 36;
        }
    }
}