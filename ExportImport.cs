using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Memoria.PSX;
using Memoria.BaseOperations;
using Microsoft.Win32;

namespace Memoria
{
    public partial class ExportImport : BaseForm
    {
        string Ext { get; set; }
        string assemblyPath
        {
            get
            {
                return (Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                    Path.DirectorySeparatorChar + "Templates" + Path.DirectorySeparatorChar).Replace("file:\\", "").Replace("file://", "");
            }
        }
        string path { get; set; }
        bool Importing { get; set; }
        RegistryKey regKey = null;                       
        byte[] buffer, header;
        int StartOffset, EndOffset;
        char[] Validator
        {
             get
             {
                return new char[] { 'G', 'J', 'Ø', 'R', 'U', 'L', 'V' };
             }
        }

        /// <summary>
        /// A new Export/Import Form.
        /// </summary>
        /// <param name="formText">The name of the Form.</param>
        /// <param name="ext">File extensions to look for.</param>
        /// <param name="import">True if we import; False if we export.</param>
        /// <param name="reg">A registry key to store folder paths in. Can be null.</param>
        /// <param name="buffer">The buffer to import to/Export from.</param>
        /// <param name="startOffset">The 1st offset in the buffer to export/import within the buffer.</param>
        /// <param name="endOffset">The inclusive last offset to export/import within the buffer.</param>
        /// <param name="header">If a whole block is imported we also need header data. Null otherwise.</param>
        public ExportImport(string formText, string ext, bool import, RegistryKey reg, ref byte[] buffer, int startOffset, int endOffset, byte[] header)
        {
            this.Icon = null;
            if (startOffset >= endOffset || startOffset < 0 || endOffset < 0 ||
                endOffset >= buffer.Length || startOffset >= buffer.Length)
            { MessageBox.Show("Invalid offset value detected."); this.Close(); return; }

            InitializeComponent();
            Setup();
            Importing = import;
            this.Text = formText; Ext = ext;
            EndOffset = endOffset;
            StartOffset = startOffset;
            this.buffer = buffer; this.header = header;
            regKey = reg; path = assemblyPath;
            switch (Ext)
            {
                case ".mmrb":
                    path += "Memory Cards" + Path.DirectorySeparatorChar;
                    ImportFileDialog.Filter = "Memoria memory card block template (*" +Ext +")|*" + Ext;
                    break;
                case ".mmrc":
                    path += "Tetra Master Cards" + Path.DirectorySeparatorChar;
                    ImportFileDialog.Filter = "Memoria Tetra Master card template (*" + Ext + ")|*" + Ext;
                    break;
                case ".mmrac":
                    path += "All Characters" + Path.DirectorySeparatorChar;
                    ImportFileDialog.Filter = "Memoria all characters template (*" + Ext +")|*" + Ext;
                    break;
                case ".mmrsc":
                    path += "Single Character" + Path.DirectorySeparatorChar;
                    ImportFileDialog.Filter = "Memoria single character template (*" + Ext +")|*" + Ext;
                    break;
                case ".mmri":
                    path += "Items" + Path.DirectorySeparatorChar;
                    ImportFileDialog.Filter = "Memoria item template (*" + Ext + ")|*" + Ext;
                    break;
                default:
                    this.Close();
                    break;
            }
            btnBrowse.Enabled = btnBrowse.Visible = Importing;
            lbFiles.Enabled = true;//Importing;
            tbTemplateName.Enabled = tbTemplateName.Visible = !Importing;
            btnOk.Text = Importing ? "Import" : "Export";
            if (Importing)
            {
                lbFiles.Location = new Point(lbFiles.Location.X, tbTemplateName.Location.Y);
                lbFiles.Height += tbTemplateName.Height;
                label1.Text = "Choose Template to Import.";
            }
        }

        private void ExportImport_Load(object sender, EventArgs e)
        {
            try
            {
                //this.Icon = Memoria.Properties.Resources.mem;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.EndsWith(Ext))
                        lbFiles.Items.Add(Strings.TrimFolders(file.Remove(file.LastIndexOf('.'))));
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (!Importing) return;
            try
            {
                ImportFileDialog.FileName = (string)GRegistry.GetRegValue(regKey, this.Text + "_path", ImportFileDialog.FileName);
                if (ImportFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string file = ImportFileDialog.FileName;
                    GRegistry.SetRegValue(regKey, this.Text + " Path", file, RegistryValueKind.String);
                    Transfer(file);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (Importing)
                {
                    if (lbFiles.SelectedItem == null)
                        return;
                    Transfer(GetLisBoxFile());
                }
                else if (!String.IsNullOrEmpty(tbTemplateName.Text))
                {
                    string file = tbTemplateName.Text;
                    if (file.EndsWith(Ext))
                        file = file.Remove(file.LastIndexOf(Ext));
                    file = path + file + Ext;
                    DialogResult res = DialogResult.Yes;
                    if (File.Exists(file))
                        res = MessageBox.Show(file + " already exists.\r\nReplace it?", "Template Exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (res == DialogResult.Yes)
                        Transfer(path + tbTemplateName.Text + Ext);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Transfer(string file)
        {
            byte[] temp;
            //The extra length if header is not null.
            int hl = header == null ? 0 : header.Length;
            if (Importing)
            {
                if(!File.Exists(file))
                {
                    MessageBox.Show(file + " does not exist or was deleted.\r\nChoose another template or create one.", "Template Gone!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                temp = File.ReadAllBytes(file);
                if (!ValidateTemplate(temp))
                    return;                
                string tempF = Strings.TrimFolders(file);
                if (!File.Exists(path + tempF))
                {
                    DialogResult res = DialogResult.Yes;
                    res = MessageBox.Show(file + " does not exists in template folder.\r\nImport it to template folder?", "Template not in folder.", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.Yes)
                        CreateFile(path + tempF, File.ReadAllBytes(file));
                }

                if (hl > 0)
                {
                    int start = StartOffset / MemCard.SAVE_BLOCK_SIZE * MemCard.BLOCK_HEADER_SIZE;
                    if (start < MemCard.BLOCK_HEADER_SIZE)
                    {
                        MessageBox.Show("Error. Wrong header info.");
                        return;
                    }
                    for (int i = start, j = 0; i < (start + hl); i++, j++)
                        buffer[i] = temp[j + Validator.Length + Ext.Length];
                }
                
                for (int i = StartOffset, j = 0; i < EndOffset + 1; i++, j++)
                    buffer[i] = temp[j + Validator.Length + Ext.Length + hl];
            }
            else
            {
                temp = new byte[Validator.Length + Ext.Length + (EndOffset - StartOffset) + 1 + hl];
                
                for (int i = 0; i < Validator.Length; i++) 
                    temp[i] = (byte)Validator[i];

                for (int i = Validator.Length, j = 0; i < Validator.Length + Ext.Length; i++, j++)
                    temp[i] = (byte)Ext[j];
                
                if (hl > 0)
                {
                    for (int i = 0; i < hl; i++)
                        temp[i + Validator.Length + Ext.Length] = header[i];
                }
                
                for (int i = StartOffset, j = 0; i < EndOffset + 1; i++, j++)
                    temp[j + Validator.Length + Ext.Length + hl] = buffer[i];
                
                CreateFile(file, temp);
            }

            this.Close();
        }

        private void lbFiles_DoubleClick(object sender, EventArgs e)
        {
            if (!Importing) return;
            try
            {
                if (lbFiles.SelectedItem == null)
                    return;
                Transfer(GetLisBoxFile());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private string GetLisBoxFile()
        {
            return path + lbFiles.SelectedItem + Ext;
        }

        private void CreateFile(string path, byte[] fileBuffer)
        {
            FileStream file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            file.Write(fileBuffer, 0, fileBuffer.Length);
            file.Close();
        }

        private bool ValidateTemplate(byte[] file)
        {
            bool ret = true; int hl = header == null ? 0 : header.Length;
            if (file.Length != Validator.Length + Ext.Length + (EndOffset - StartOffset) + 1 + hl)
                ret = false; 
            for (int i = 0; i < Validator.Length; i++)
            {
                if (file[i] != (byte)Validator[i])
                {
                    ret = false; break;
                }
            }
            for (int i = Validator.Length, j = 0; i < Validator.Length + Ext.Length; i++, j++)
            {
                if (file[i] != (byte)Ext[j])
                {
                    ret = false; break;
                }
            }
            if (!ret) MessageBox.Show("This is not a valid MEMORIA template.");
            return ret;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbFiles.SelectedItem == null)
                    return;
                if (MessageBox.Show("Delete template?\r\nIt cannot be restored once deleted.", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    if(File.Exists(GetLisBoxFile()))
                        File.Delete(GetLisBoxFile());
                    lbFiles.Items.Remove(lbFiles.SelectedItem);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}