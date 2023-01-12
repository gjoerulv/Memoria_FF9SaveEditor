using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Memoria.BaseOperations;
using Memoria.Override;
using Memoria.Properties;
using Memoria.PSX;
using Microsoft.Win32;
//using MsgBoxCheck;

namespace Memoria
{
    public partial class SelectSave : BaseForm
    {

        #region Constants

        /// <summary>
        /// The name of the form.
        /// </summary>
        readonly string FORM_NAME = "Memoria : FFIX Save Editor v" + Program.Version;

        /// <summary>
        /// The Registry path.
        /// </summary>
        const string REG_PATH = "Software\\MEMORIA";

        /// <summary>
        /// x86 bitness (4).
        /// </summary>
        const byte WIN32 = 4;

        /// <summary>
        /// x64 bitness (8).
        /// </summary>
        const byte WIN64 = 8;

        /// <summary>
        /// Code for NTCS US dDiscs.
        /// </summary>
        char[][] NTSCDiscs = new char[][]
        {
            SaveMap.DISC1U, SaveMap.DISC2U,
            SaveMap.DISC3U, SaveMap.DISC4U
        };

        /// <summary>
        /// Code for NTCS JAP dDiscs.
        /// </summary>
        char[][] NTSCJAPDiscs = new char[][]
        {
            SaveMap.DISC1J, SaveMap.DISC2J,
            SaveMap.DISC3J, SaveMap.DISC4J
        };

        /// <summary>
        /// Code for PAL EU discs.
        /// </summary>
        char[][] PALDiscs = new char[][]
        {
            SaveMap.DISC1E, SaveMap.DISC2E,
            SaveMap.DISC3E, SaveMap.DISC4E
        };

        /// <summary>
        /// Code for PAL french discs.
        /// </summary>
        char[][] PALFrDiscs = new char[][]
        {
            SaveMap.DISC1Fr, SaveMap.DISC2Fr,
            SaveMap.DISC3Fr, SaveMap.DISC4Fr
        };

        /// <summary>
        /// Code for PAL german french discs.
        /// </summary>
        char[][] PALGrDiscs = new char[][]
        {
            SaveMap.DISC1Ger, SaveMap.DISC2Ger,
            SaveMap.DISC3Ger, SaveMap.DISC4Ger
        };

        /// <summary>
        /// Code for Italyn PAL french discs.
        /// </summary>
        char[][] PALItDiscs = new char[][]
        {
            SaveMap.DISC1It, SaveMap.DISC2It,
            SaveMap.DISC3It, SaveMap.DISC4It
        };

        /// <summary>
        /// Code for PAL french discs.
        /// </summary>
        char[][] PALSpDiscs = new char[][]
        {
            SaveMap.DISC1Sp, SaveMap.DISC2Sp,
            SaveMap.DISC3Sp, SaveMap.DISC4Sp
        };

        /// <summary>
        /// The Euro/Us FF9 chatacter table.
        /// </summary>
        public Hashtable characterTable = new Hashtable() //29 38 33
		{
            {'0',0x00},{'1',0x01},{'2',0x02},{'3',0x03},{'4',0x04},{'5',0x05},{'6',0x06},{'7',0x07},{'8',0x08},{'9',0x09},{'+',0x0A},{'-',0x0B},{'*',0x0C},{'=',0x0D},{'%',0x0E},{' ',0x0F},
            {'A',0x10},{'B',0x11},{'C',0x12},{'D',0x13},{'E',0x14},{'F',0x15},{'G',0x16},{'H',0x17},{'I',0x18},{'J',0x19},{'K',0x1A},{'L',0x1B},{'M',0x1C},{'N',0x1D},{'O',0x1E},{'P',0x1F},
            {'Q',0x20},{'R',0x21},{'S',0x22},{'T',0x23},{'U',0x24},{'V',0x25},{'W',0x26},{'X',0x27},{'Y',0x28},{'Z',0x29},{'(',0x2A},{'!',0x2B},{'?',0x2C},{'“',0x2D},{':',0x2E},{'.',0x2F},
            {'a',0x30},{'b',0x31},{'c',0x32},{'d',0x33},{'e',0x34},{'f',0x35},{'g',0x36},{'h',0x37},{'i',0x38},{'j',0x39},{'k',0x3A},{'l',0x3B},{'m',0x3C},{'n',0x3D},{'o',0x3E},{'p',0x3F},
            {'q',0x40},{'r',0x41},{'s',0x42},{'t',0x43},{'u',0x44},{'v',0x45},{'w',0x46},{'x',0x47},{'y',0x48},{'z',0x49},{')',0x4A},{',',0x4B},{'/',0x4C},{'•',0x4D},{'~',0x4E},{'&',0x4F},
            {'Á',0x50},{'À',0x51},{'Â',0x52},{'Ä',0x53},{'É',0x54},{'È',0x55},{'Ê',0x56},{'Ë',0x57},{'Í',0x58},{'Ì',0x59},{'Î',0x5A},{'Ï',0x5B},{'Ó',0x5C},{'Ò',0x5D},{'Ô',0x5E},{'Ö',0x5F},
            {'Ú',0x60},{'Ù',0x61},{'Û',0x62},{'Ü',0x63},{'á',0x64},{'à',0x65},{'â',0x66},{'ä',0x67},{'é',0x68},{'è',0x69},{'ê',0x6A},{'ë',0x6B},{'í',0x6C},{'ì',0x6D},{'î',0x6E},{'ï',0x6F},
            {'ó',0x70},{'ò',0x71},{'ô',0x72},{'ö',0x73},{'ú',0x74},{'ù',0x75},{'û',0x76},{'ü',0x77},{'Ç',0x78},{'Ñ',0x79},{'ç',0x7A},{'ñ',0x7B},{'Œ',0x7C},{'ß',0x7D},{'’',0x7E},{'”',0x7F},
            {'_',0x80},{'】',0x81},{'【',0x82},{'∴',0x83},{'∵',0x84},{'♪',0x85},{'→',0x86},{'∈',0x87},{'ⅹ',0x88},{'♦',0x89},{'§',0x8A},{'‹',0x8B},{'›',0x8C},{'←',0x8D},{'∋',0x8E},{'↑',0x8F},
            {'△',0x90},{'□',0x91},{'∞',0x92},{'♥',0x93},
                       {'≪',0xA1},{'≫',0xA2},{'↓',0xA3},{'─',0xA4},{'°',0xA5},{'★',0xA6},{'♂',0xA7},{'♀',0xA8},{'☺',0xA9},           {'„',0xAB},{'‘',0xAC},{'#',0xAD},{'※',0xAE},{';',0xAF},
            {'¡',0xB0},{'¿',0xB1},
        };

        public Bitmap[] cardPics = new Bitmap[SaveMap.CARD_MAX_CARD_TYPES];

        #endregion

        #region Variables

        byte[] discNrs = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] SlotNrs = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] BlockNrs = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        string copiedSlotRegion = "";

        public int slotID_rr2016 = -1;
        public int saveID_rr2016 = -1;

        /// <summary>
        /// The "About" form. Cannot open if not null.
        /// </summary>
        public object about = null;

        /// <summary>
        /// Indicates whether a memory card is open or not.
        /// </summary>
        bool fileOpen = false;

        /// <summary>
        /// Determines whether a slot/block is openable.
        /// </summary>
        bool canOpenASlot = false;

        /// <summary>
        /// The opened memory card.
        /// </summary>
        string file = "";

        /// <summary>
        /// The selected block index.
        /// </summary>
        private byte blockIndex = 0;

        /// <summary>
        /// The save locations on the card.
        /// </summary>
        public string[] locations = new string[15];

        /// <summary>
        /// The party Leaders.
        /// </summary>
        public string[] partyLeaders = new string[15];

        byte framerate = 50;

        private const byte importHeader = 0x88;

        bool closing = false, spaceWasPushed = false;
        Timer timer;

        Bitmap prop { get { return Resources.Properties_24x24; } }
        Bitmap Zidane { get { return Resources.Zidane; } }
        Bitmap Vivi { get { return Resources.Vivi; } }
        Bitmap Dagger { get { return Resources.Dagger; } }
        Bitmap Steiner { get { return Resources.Steiner; } }
        Bitmap Freya { get { return Resources.Freya; } }
        Bitmap Quina { get { return Resources.Quina; } }
        Bitmap Eiko { get { return Resources.Eiko; } }
        Bitmap Amarant { get { return Resources.Amarant; } }
        Bitmap Beatrix { get { return Resources.Beatrix; } }
        Bitmap Blank { get { return Resources.Blank; } }
        Bitmap Marcus { get { return Resources.Marcus; } }
        Bitmap Cinna { get { return Resources.Cinna; } }

        public PSX.FileInfo info = new PSX.FileInfo();
        public MemCardFileType Type
        {
            get
            {
                info.type = MemCardFileType.MCR; MemCardFileType tempType = MemCardFileType.MCR;
                if (string.IsNullOrEmpty(file)) return tempType;
                string ext = "";
                if (file.Contains("."))
                    ext = file.Substring(file.LastIndexOf('.'));
                ext = ext.ToLower();
                switch (ext)
                {
                    case ".mcr":
                    case ".mcd":
                    case ".bin":
                    case ".mc":
                    case ".mci":
                    case ".ps":
                    case ".psm":
                    case ".dff":
                        tempType = MemCardFileType.MCR; break;
                    case ".gme":
                        tempType = MemCardFileType.DexDrive; break;
                    case ".psx":
                    case ".pda":
                    case ".mcx":
                    case ".mcb":
                        tempType = MemCardFileType.PSX; break;
                    case ".mem":
                    case ".vgs":
                        tempType = MemCardFileType.VGM; break;
                    case ".psv":
                        tempType = MemCardFileType.PSV; break;
                    case ".vmp":
                        tempType = MemCardFileType.PSP; break;
                    case ".ps1":
                    case ".mcs":
                        tempType = MemCardFileType.SIMPLE; break;
                    case ".mmrb":
                        tempType = MemCardFileType.TEMPLATE; break;
                    case ".sav":
                        tempType = MemCardFileType.SAV; break;
                    case ".dat":
                        tempType = MemCardFileType.DAT; break;
                    default:
                        tempType = MemCardFileType.RAW; break;
                }
                info.type = tempType;
                return tempType;
            }
        }
        byte[] extraHeader;

        BackgroundWorker ImageLoader;

        public Save saveForm;

        /// <summary>
        /// OS bitrate. 
        /// </summary>
        public int bitness;

        #region Buffers

        /// <summary>
        /// Main buffer. Contains all bytes in the memory card opened.
        /// </summary>
        private byte[] mainBuffer = new byte[MemCard.MEMORY_CARD_SIZE];

        /// <summary>
        /// Copy of main buffer. Used to check if the user can save.
        /// </summary>
        public byte[] mainBufferCopy = new byte[MemCard.MEMORY_CARD_SIZE];

        /// <summary>
        /// Contains all the undo buffers. Copy from/into the mainBufferCopy.
        /// </summary>
        byte[][] undoBuffers = new byte[4][];

        /// <summary>
        /// Contains all the redo buffers. Copy from/into the mainBufferCopy.
        /// </summary>
        byte[][] redoBuffers = new byte[4][];

        /// <summary>
        /// Contains copied or cut data.
        /// </summary>
        byte[] cutCopyBuffer = new byte[MemCard.BLOCK_HEADER_SIZE + MemCard.SAVE_BLOCK_SIZE];

        #endregion

        /// <summary>
        /// Controls the registry values.
        /// </summary>
        public RegistryKey regKey = null;

        byte undoLeft = 0;
        byte redoLeft = 0;

        List<string> prevOpenedFiles = new List<string>();

        #endregion

        #region Constructor/Form Load

        /*const string formatter = "{0,25:E16}{1,30}";
        public string GetBytesDouble(double argument)
        {
            byte[] byteArray = BitConverter.GetBytes(argument);
            return string.Format(formatter, argument,
                BitConverter.ToString(byteArray));
        }*/

        public SelectSave()
        {
            InitializeComponent();
            Setup();
            this.Text = FORM_NAME;

            int totalWidth = 0;
            foreach (DataGridViewColumn col in gridView.Columns)
            {
                //int change = -50 + (int)(fontSize * fontSize);
                //change = 100 + change;
                //col.Width = col.Width * change / 100;
                totalWidth += col.Width;
            }
            Width = totalWidth + 16;
            int totalHeight = menuStrip.Height + statusStrip.Height + toolStripTop.Height + 24;
            totalHeight += 15 * gridView.RowTemplate.Height;
            Height = totalHeight;

            bitness = IntPtr.Size;

            if (oss[0])
            {
                this.Icon = Memoria.Properties.Resources.mem;
            }
            else
            {
                this.Icon = null;
                this.ShowIcon = false;
            }

            for (byte b = 0; b < undoBuffers.Length; b++)
            {
                undoBuffers[b] = null;
            }
            for (byte b = 0; b < redoBuffers.Length; b++)
            {
                redoBuffers[b] = null;
            }

            CopyBuffers(mainBuffer, mainBufferCopy);
            undoLeft = redoLeft = 0;

            //Set the registry key...
            try
            {
                //...and the grid view timer.
                timer = new Timer(); timer.Interval = 200;
                timer.Tick += new System.EventHandler(this.Tick);

                ImageLoader = new BackgroundWorker();
                ImageLoader.DoWork += new DoWorkEventHandler(ImageLoader_DoWork);
                ImageLoader.RunWorkerAsync();

                if (!oss[1] && !oss[2] && oss[0])
                {
                    regKey = Registry.CurrentUser.CreateSubKey(REG_PATH, RegistryKeyPermissionCheck.ReadWriteSubTree);

                    if (bitness == WIN32 && regKey != null)
                        regKey = Registry.CurrentUser.OpenSubKey(REG_PATH, true);
                    else if (bitness == WIN64)
                        regKey = GRegistry.OpenSubKey(Registry.CurrentUser, REG_PATH, true,
                            GRegistry.eRegWow64Options.KEY_WOW64_64KEY | GRegistry.eRegWow64Options.KEY_ALL_ACCESS);
                }

                SetFormSizeFromReg(regKey);
            }
            catch (Exception ex) { statusLabel.Text = ex.Message; }
            finally
            {
                if (oss[1] || oss[2])
                {
                    toolStripSplitButtonOpen.HideDropDown();
                }

                else
                {
                    if (regKey == null && oss[0])
                        regKey = Registry.CurrentUser.CreateSubKey(REG_PATH, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    FillPrevOpenedFiles(true);
                }
            }
        }

        public RR2016_Selector rr2016Selector;
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                DoResize();
                About a = new About(null);
                saveForm = new Save(this);
                a.Show(this);
                this.BringToFront();

                rr2016Selector = new RR2016_Selector();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImageLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SetupCardPictures();
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Card load fail: " + ex.Message;
                for (int i = 0; i < cardPics.Length; i++)
                    cardPics[i] = new Bitmap(47, 58);
            }
        }

        private void SetupCardPictures()
        {
            //Draw all Card Pictures.
            Bitmap cardMap = Resources.Cards, cardPic = new Bitmap(47, 58);
            for (int i = 0; i < cardPics.Length; i++)
            {
                //Decide upper left corner of card bitmap.
                int X = 3 + ((i % 10) * 2) + ((i % 10) * 47),
                    Y = 2 + ((i / 10) * 60);

                //Draw the new return image
                for (int rX = 0; rX < cardPic.Width && X < cardMap.Width; X++, rX++)
                {
                    for (int rY = 0; rY < cardPic.Height && Y < cardMap.Height; Y++, rY++)
                        cardPic.SetPixel(rX, rY, cardMap.GetPixel(X, Y));
                    Y = 2 + ((i / 10) * 60);
                }

                cardPics[i] = new Bitmap(cardPic);
            }
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="path">The path to save the file to (filename).</param>
        private void Save_File()
        {
            if (!fileOpen)
                throw new Exception("No file is open");
            DialogResult res = DialogResult.Yes;
            if (Type == MemCardFileType.PSV || Type == MemCardFileType.PSP)
            {
                string message = Type == MemCardFileType.PSP ? "This is a VMP file." : "This is a PSV file.";
                message += Environment.NewLine + "Saving this Type of file will most likely corrupt the save data.\r\nIt is possible to use the \"Save As...\" function to save this file as another format.\r\nProceed to save (and corrupt the file)?";

                res = MessageBox.Show(message, "Warning, Please read.",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
            else if(info.IsRR2016SaveType)
            {

                object prevWarned = GRegistry.GetRegValue(regKey, "RRsave2016Warning", null);
                if (prevWarned == null)
                    res = MessageBox.Show("FF9 rerelease (2016) savetype must still be considered a bit experimental. Please make sure to backup your files. Continue to save?", "Warning, Please read.",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                GRegistry.SetRegValue(regKey, "RRsave2016Warning", 1, RegistryValueKind.DWord);
            }
            if (res == DialogResult.Yes)
            {
                CopyBuffers(mainBufferCopy, mainBuffer);
                bool success = true;
                if(info.IsRR2016SaveType)
                {
                    ReUtils.DataManager rrDataHandler = new ReUtils.DataManager();
                    rrDataHandler.Save(slotID_rr2016, saveID_rr2016, file, mainBuffer, out success);
                }
                else
                {
                    byte[] stream = MemCard.ToType(mainBuffer, Type, blockIndex, extraHeader);
                    using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fs.Write(stream, 0, stream.Length);
                        fs.Close();
                    }
                    GC.Collect(); GC.WaitForPendingFinalizers();
                }
                if (!success)
                    throw new Exception("An error occurred during saving process.");
                EnableSaveButtons();
                SetFormText(file);
                RearrangePrevOpenedFiles(file);
            }
        }

        private void Load_File()
        {
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                info.type = Type; //just in case;

                if (info.IsRR2016SaveType && (saveID_rr2016 < 0 || slotID_rr2016 < 0))
                {
                    MessageBox.Show("Failed to load file, application will now exit.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                byte[] stream = { };
                string producCode = null;
                if (!info.IsRR2016SaveType)
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        stream = new byte[fs.Length];
                        fs.Read(stream, 0, stream.Length);
                        fs.Close();
                    }
                    GC.Collect(); GC.WaitForPendingFinalizers();

                    if (Type == MemCardFileType.RAW)
                        producCode = Strings.TrimFolders(file);
                    mainBuffer = MemCard.ToMCR(stream, Type, producCode);
                    mainBufferCopy = new byte[mainBuffer.Length];
                    CopyBuffers(mainBuffer, mainBufferCopy);
                }
                else
                {
                    ReUtils.DataManager storage = new ReUtils.DataManager();
                    storage.Load(slotID_rr2016, saveID_rr2016, file, out stream);

                    //File.WriteAllBytes("ekkodil", stream);
                    producCode = Strings.TrimFolders(file);
                    mainBuffer = new byte[stream.Length];
                    mainBufferCopy = new byte[stream.Length];
                    Array.Copy(stream, mainBuffer, stream.Length);
                    Array.Copy(stream, mainBufferCopy, stream.Length);
                }

                extraHeader = MemCard.ExtraHeader(stream, Type);
                FillGrid(mainBuffer, true);
                SetFormText(file);
                undoLeft = redoLeft = 0;
                SetUndoRedoEnabled();
                EnableSaveButtons();
                formatToolStripMenuItem.Enabled = true;
                for (byte b = 0; b < undoBuffers.Length; b++)
                    undoBuffers[b] = new byte[MemCard.MEMORY_CARD_SIZE];
                for (byte b = 0; b < redoBuffers.Length; b++)
                    redoBuffers[b] = new byte[MemCard.MEMORY_CARD_SIZE];
                RearrangePrevOpenedFiles(file);

            }
            else
                throw new Exception("File does not exist. No file was opened.");
        }

        #endregion

        #region Button Clicks

        private void SelectSave_Click(object sender, EventArgs e)
        {
            if (gridView.SelectedRows == null || !fileOpen || !canOpenASlot)
                return;
            try
            {
                DataGridViewDisableButtonCell c =
                (DataGridViewDisableButtonCell)gridView.SelectedRows[0].Cells[0];
                if (!c.Enabled)
                    return;

                if (saveForm == null)
                    saveForm = new Save(this);

                //block = null;
                GC.Collect(); GC.WaitForPendingFinalizers();
                saveForm.Start(blockIndex, mainBufferCopy, BlockNrs);
                GC.Collect(); GC.WaitForPendingFinalizers();

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            DialogResult res = DialogResult.No;
            if (!CheckBuffers() && fileOpen)
                res = MessageBox.Show("Card not saved. Save before exit?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes && fileOpen)
                Save_Click(null, null);
            if (res != DialogResult.Cancel)
            {
                closing = true;
                Application.Exit();
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = DialogResult.No;
                if (!CheckBuffers() && fileOpen)
                    res = MessageBox.Show("Card not saved. Save before open?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes && fileOpen)
                    Save_Click(null, null);
                if (res == DialogResult.No || res == DialogResult.Yes)
                {
                    openFileDialog.Filter = FILE_FILTER_ALL + '|' + FILE_FILTER_CARDS +
                    '|' + FILE_FILTER_SINGLE + '|' + FILTER_RR2016 + '|' + FILE_FILTER_CORRUPT;
                    toolStripSplitButtonOpen.HideDropDown();
                    //openFileDialog.FileName = Strings.TrimFolders((string)GRegistry.GetRegValue(regKey, "Open_FilePath", file));
                    openFileDialog.FileName = (string)GRegistry.GetRegValue(regKey, "Open_FilePath", file);
                    if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        string prevFile = file;
                        file = openFileDialog.FileName;
                        info.type = Type;
                        GRegistry.SetRegValue(regKey, "Open_FilePath", file, RegistryValueKind.String);
                        if (info.IsRR2016SaveType)
                        {
                            rr2016Selector = new RR2016_Selector();
                            rr2016Selector.ShowDialog(this);
                            slotID_rr2016 = rr2016Selector.slotID;
                            saveID_rr2016 = rr2016Selector.saveID;

                            if(slotID_rr2016 < 0 || saveID_rr2016 < 0)
                            {
                                file = prevFile; info.type = Type;
                                return;
                            }
                        }
                        Load_File();
                    }
                    else if (fileOpen && !string.IsNullOrEmpty(file))
                        RearrangePrevOpenedFiles(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrevOpenedFiles_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = DialogResult.No;
                if (!CheckBuffers() && fileOpen)
                    res = MessageBox.Show("Card not saved. Save before open?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes && fileOpen)
                    Save_Click(null, null);
                if (res == DialogResult.No || res == DialogResult.Yes)
                {
                    string prevFile = file;
                    file = (string)((ToolStripMenuItem)sender).Text;
                    info.type = Type;
                    GRegistry.SetRegValue(regKey, "Open_FilePath", file, RegistryValueKind.String);
                    if (info.IsRR2016SaveType)
                    {
                        rr2016Selector = new RR2016_Selector();
                        rr2016Selector.ShowDialog(this);
                        slotID_rr2016 = rr2016Selector.slotID;
                        saveID_rr2016 = rr2016Selector.saveID;

                        if (slotID_rr2016 < 0 || saveID_rr2016 < 0)
                        {
                            file = prevFile; info.type = Type;
                            return;
                        }
                    }
                    
                    Load_File();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(file))
                {
                    MessageBox.Show("Source file is deleted or moved.\r\nPlease select where to save.", "Could not find source.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SaveAs_Click(null, null);
                    return;
                }
                else
                {

                    Save_File();
                    EnableSaveButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                toolStripSplitButtonOpen.HideDropDown();
                //saveFileDialog.FilterIndex =  .FileName = file;//(string)GRegistry.GetRegValue(regKey, "Save_FilePath", file);
                //PSX.FileInfo f = new PSX.FileInfo();
                info.type = Type;
                if (info.IsRR2016SaveType)
                    saveFileDialog.Filter = FILTER_RR2016;
                else if (info.MaxNumberOfSaves > 1)
                    saveFileDialog.Filter = FILE_FILTER_CARDS;
                else
                    saveFileDialog.Filter = FILE_FILTER_CARDS + '|' + FILE_FILTER_SINGLE;
                saveFileDialog.FileName = Type == MemCardFileType.RAW ? "" : Strings.TrimFolders(file);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file = saveFileDialog.FileName;
                    //if(saveFileDialog.FilterIndex == 17)
                    //{
                    //    string temp = file;
                    //    temp = Strings.TrimFolders(temp);
                    //    string s = temp;
                    //    if(s.Contains(".") && s.LastIndexOf('.') == s.Length - 4)
                    //        s = s.Remove(s.LastIndexOf('.'));
                    //    temp = file.Replace(temp, s);
                    //    file = temp;
                    //}

                    //GRegistry.SetRegValue(regKey, "Save_FilePath", file, RegistryValueKind.String);
                    RearrangePrevOpenedFiles(file);
                    Save_File();
                    EnableSaveButtons();
                }
                else if (fileOpen && file != "")
                    RearrangePrevOpenedFiles(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileOpen || info.IsRR2016SaveType)
                    return;
                byte[] blocksToDelete = MemCard.GetBlockNrsInChain(mainBufferCopy, blockIndex);
                StackUndoBuffer();
                foreach (byte b in blocksToDelete)
                    DeleteSlot(b);
                FillGrid(mainBufferCopy, false);
                EnableSaveButtons();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DeleteSlot(byte blockNr)
        {
            if (info.IsRR2016SaveType) return;
            int start = MemCard.BLOCK_HEADER_SIZE * blockNr;
            for (int i = start; i < start + MemCard.BLOCK_HEADER_SIZE; i++)
            {
                if (i == start || i == start + MemCard.BLOCK_HEADER_SIZE - 1)
                    mainBufferCopy[i] = 0xA0;
                else if (i == start + 8 || i == start + 9)
                    mainBufferCopy[i] = 0xFF;
                else
                    mainBufferCopy[i] = 0;
            }
            start = MemCard.SAVE_BLOCK_SIZE * blockNr;
            for (int i = start; i < start + MemCard.SAVE_BLOCK_SIZE; i++)
                mainBufferCopy[i] = 0x0;
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileOpen || !CanCopyOrPasteBlock(true))
                    return;
                CopyBuffer();
                pasteToolStripMenuItem.Enabled = true;
                pasteToolStripMenuItem1.Enabled = true;
                toolStripButtonPaste.Enabled = true;
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CanCopyOrPasteBlock(true)) return;
                Copy_Click(null, null);
                Delete_Click(null, null);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void CopyBuffer()
        {
            if (!fileOpen || info.IsRR2016SaveType)
                return;
            int start = MemCard.BLOCK_HEADER_SIZE * blockIndex;
            for (int i = start, j = 0; i < start + MemCard.BLOCK_HEADER_SIZE; i++, j++)
                cutCopyBuffer[j] = mainBufferCopy[i];
            start = MemCard.SAVE_BLOCK_SIZE * blockIndex;
            for (int i = start, j = MemCard.BLOCK_HEADER_SIZE; i < start + MemCard.SAVE_BLOCK_SIZE; i++, j++)
                cutCopyBuffer[j] = mainBufferCopy[i];
            copiedSlotRegion = (string)gridView.Rows[blockIndex - 1].Cells["Reg"].Value;
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileOpen || !CanCopyOrPasteBlock(true) || info.IsRR2016SaveType)
                    return;
                StackUndoBuffer();
                int start = MemCard.BLOCK_HEADER_SIZE * blockIndex;
                for (int i = start, j = 0; i < start + MemCard.BLOCK_HEADER_SIZE; i++, j++)
                    mainBufferCopy[i] = cutCopyBuffer[j];
                start = MemCard.SAVE_BLOCK_SIZE * blockIndex;
                for (int i = start, j = MemCard.BLOCK_HEADER_SIZE; i < start + MemCard.SAVE_BLOCK_SIZE; i++, j++)
                    mainBufferCopy[i] = cutCopyBuffer[j];

                if (!string.IsNullOrEmpty(copiedSlotRegion))
                    updateSlotNr(copiedSlotRegion);

                FillGrid(mainBufferCopy, false);
                EnableSaveButtons();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private bool CanCopyOrPasteBlock(bool promt)
        {
            if (blockIndex < 1 || blockIndex > 15 || info.IsRR2016SaveType) return false;

            int i = MemCard.BLOCK_HEADER_SIZE * blockIndex;
            if (mainBufferCopy[i] == 160) return true;

            int spaceOffset = i + 5;
            int nextPointerOffset = i + MemCard.NEXT_BLOCK_POINTER_OFFSET;

            if (mainBufferCopy[spaceOffset] != 32 || mainBufferCopy[spaceOffset + 1] != 0 ||
                mainBufferCopy[nextPointerOffset] != 255)
            {
                if (promt)
                {
                    byte[] b = MemCard.GetBlockNrsInChain(mainBufferCopy, blockIndex);
                    string bl = ""; foreach (byte bb in b) bl += "" + bb + ", ";
                    bl = bl.TrimEnd(new char[] { ' ', ',' });

                    MessageBox.Show("Can not copy/cut from or paste/import into block index " + blockIndex + ".\r\nIt is a multi save block (" + b.Length + " blocks: " + bl + ").", "Unable to Copy/Cut/Paste.", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                return false;
            }

            return true;
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileOpen || info.IsRR2016SaveType)
                    return;
                if (undoLeft > 0)
                {
                    undoLeft--;
                    if (redoLeft < 4)
                        redoLeft++;
                    PushUndoRedoBuffers(redoBuffers);
                    CopyBuffers(undoBuffers[0], mainBufferCopy);
                    PopUndoRedoBuffers(undoBuffers);
                    FillGrid(mainBufferCopy, false);
                }
                SetUndoRedoEnabled();
                EnableSaveButtons();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileOpen || info.IsRR2016SaveType)
                    return;
                if (redoLeft > 0)
                {
                    redoLeft--;
                    if (undoLeft < 4)
                        undoLeft++;
                    PushUndoRedoBuffers(undoBuffers);
                    CopyBuffers(redoBuffers[0], mainBufferCopy);
                    PopUndoRedoBuffers(redoBuffers);
                    FillGrid(mainBufferCopy, false);
                }
                SetUndoRedoEnabled();
                EnableSaveButtons();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public void StackUndoBuffer()
        {
            if (info.IsRR2016SaveType) return;
            PushUndoRedoBuffers(undoBuffers);
            if (undoLeft < 4)
                undoLeft++;
            redoLeft = 0;
            SetUndoRedoEnabled();
        }

        private void Convert_Click(object sender, EventArgs e)
        {
            if (!canOpenASlot || !fileOpen || info.IsRR2016SaveType)
                return;
            if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == "US")
                PAL_Click(null, null);
            else if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == "EU")
                Japan_Click(null, null);
            else if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == "Japan")
                France_Click(null, null);
            else if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == "France")
                Germany_Click(null, null);
            else if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == "Germany")
                Italy_Click(null, null);
            else if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == "Italy")
                Spain_Click(null, null);
            else if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == "Spain")
                US_Click(null, null);

        }

        private void ConvertBlockRegion(string regionString, char[] regionCharArray, char[][] discs)
        {
            try
            {
                //gridView_SelectionChanged(null, null);
                if (!canOpenASlot || !fileOpen || info.IsRR2016SaveType) return;

                if ((string)gridView.SelectedRows[0].Cells["Reg"].Value == regionString)
                    return;

                byte firstID = 0, discnr;

                StackUndoBuffer();

                discnr = discNrs[blockIndex - 1];
                if (discnr < 1 || discnr > 4)
                    throw new Exception("Convert_Click reports: Wrong disc number");

                List<char> code = new List<char>(20);
                firstID = FirstAvailableGameID(regionString);

                foreach (char c in regionCharArray) { code.Add(c); }
                foreach (char c in discs[discnr - 1]) { code.Add(c); }
                code.Add('\x2D');
                foreach (char c in Strings.GetSlotGameIDnr(firstID)) { code.Add(c); }

                MemCard.SetBlockGameID(code.ToArray(), mainBufferCopy, blockIndex);
                MemCard.CalaculateHeaderChecksum(blockIndex, mainBufferCopy);

                SetFileNameTime(blockIndex, mainBufferCopy);
                SetFileNumber(firstID);

                FillGrid(mainBufferCopy, false);
                EnableSaveButtons();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void US_Click(object sender, EventArgs e)
        {
            ConvertBlockRegion("US", SaveMap.NTSC, NTSCDiscs);
        }

        private void Japan_Click(object sender, EventArgs e)
        {
            ConvertBlockRegion("Japan", SaveMap.NTSCJAP, NTSCJAPDiscs);
        }

        private void PAL_Click(object sender, EventArgs e)
        {
            ConvertBlockRegion("EU", SaveMap.PAL, PALDiscs);
        }

        private void Spain_Click(object sender, EventArgs e)
        {
            ConvertBlockRegion("Spain", SaveMap.PAL, PALSpDiscs);
        }

        private void Italy_Click(object sender, EventArgs e)
        {
            ConvertBlockRegion("Italy", SaveMap.PAL, PALItDiscs);
        }

        private void Germany_Click(object sender, EventArgs e)
        {
            ConvertBlockRegion("Germany", SaveMap.PAL, PALGrDiscs);
        }

        private void France_Click(object sender, EventArgs e)
        {
            ConvertBlockRegion("France", SaveMap.PAL, PALFrDiscs);
        }

        private void More_Info_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fileOpen || info.IsRR2016SaveType /* || !canOpenASlot*/)
                    return;
                string title = MemCard.GetBlockTitle(blockIndex, mainBufferCopy);
                string productCode = MemCard.GetBlockProductCode(mainBufferCopy, blockIndex);
                string gameID = MemCard.GetBlockGameID(mainBufferCopy, blockIndex);
                char c = MemCard.GetRegionChar(mainBufferCopy, blockIndex);
                Slot slot = new Slot(title, MemCard.GetBlockIcon(mainBufferCopy, blockIndex), productCode, gameID, c);
                slot.Text = "Info Block " + blockIndex;
                slot.Show();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private void Properies_Click(object sender, EventArgs e)
        {
            UnknownValues u = new UnknownValues(mainBufferCopy);
            u.ShowDialog();
        }

        private void ExportImportClick(object sender, EventArgs e)
        {
            string temp = file;
            try
            {
                
                //gridView_SelectionChanged(null, null);
                int currentRow = -1;
                if (gridView.SelectedRows != null)
                    currentRow = gridView.SelectedRows[0].Index;
                if (!fileOpen || currentRow <= -1) return;

                bool Importing = false, fillAgain = false, isFF9 = true;
                PortableType type; byte tag;

                if (sender is ToolStripMenuItem)
                    tag = Convert.ToByte((sender as ToolStripMenuItem).Tag);
                else if (sender is ToolStripItem)
                    tag = Convert.ToByte((sender as ToolStripItem).Tag);
                else
                    tag = Convert.ToByte((sender as Control).Tag);

                #region RR2016
                if(info.IsRR2016SaveType) //DUMP/IMPORT RR
                {
                    isFF9 = false;//set this here to avoid 2x fill of grid.

                    //TODO: for now throw exception if RR2016 type.
                    if (tag <  100) //This means a Template.
                        throw new Exception("ExportImportClick TODO: template not yet implemented for RR2016 type save.");

                    bool import = tag == 100;
                    if(import)
                    {
                        object prevWarned = GRegistry.GetRegValue(regKey, "RRimpss2016Warning", null);
                        if(prevWarned == null)
                            MessageBox.Show(this, "!Only import previously exported data (or data you KNOW is raw, decrypted FF9 data). FF9RR will not support anything else, like PS3 virtual saves.", 
                                "Careful!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        openFileDialog.Filter = FILTER_RR2016_SINGLE_FILE;
                        openFileDialog.FileName = "";
                        openFileDialog.Title = "Import raw, decrypted RR2016 file.";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            byte[] stream = { };
                            using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                            {
                                stream = new byte[fs.Length];
                                fs.Read(stream, 0, stream.Length);
                                fs.Close();
                            }
                            GC.Collect(); GC.WaitForPendingFinalizers();

                            if (stream.Length == mainBufferCopy.Length)
                            {
                                Importing = true;
                                StackUndoBuffer();
                                CopyBuffers(stream, mainBufferCopy);
                                GRegistry.SetRegValue(regKey, "RRimpss2016Warning", 1, RegistryValueKind.DWord);
                            }
                            else
                            {
                                throw new Exception("The size of this file is wrong.");
                            }
                            
                        }
                    }
                    else
                    {
                        saveFileDialog.Filter = FILTER_RR2016_SINGLE_FILE;
                        saveFileDialog.FileName = "FF9_2016_save__slot" +  (slotID_rr2016 + 1) + "_file" + (saveID_rr2016 + 1);
                        saveFileDialog.Title = "Export raw, decrypted RR2016 file.";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite))
                            {
                                fs.Write(mainBufferCopy, 0, mainBufferCopy.Length);
                                fs.Close();
                            }
                            GC.Collect(); GC.WaitForPendingFinalizers();
                        }
                    }
                }
                #endregion
                else if (tag < 100) //This means a Template.
                {
                    type = (PortableType)tag;
                    byte[] header = MemCard.GetBlockHeader(mainBufferCopy, blockIndex);
                    if (type.ToString().Contains("Import"))
                    {
                        Importing = true;
                        StackUndoBuffer();
                    }
                    FormLoader.OpenExportImport(ref mainBufferCopy, type, blockIndex, 0, regKey, header);
                }//Template


                else if (tag == 100) //Import SS
                {
                    openFileDialog.Filter = FILE_FILTER_SINGLE + '|' + "PS3 Virtual Save (*.psv)|*.psv";
                    openFileDialog.FileName = "";
                    openFileDialog.Title = "Import Single Save Format";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        file = openFileDialog.FileName;
                        byte[] stream = { };
                        using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            stream = new byte[fs.Length];
                            fs.Read(stream, 0, stream.Length);
                            fs.Close();
                        }
                        GC.Collect(); GC.WaitForPendingFinalizers();

                        string producCode = null;
                        if (Type == MemCardFileType.RAW)
                            producCode = Strings.TrimFolders(file);
                        stream = MemCard.ToMCR(stream, Type, producCode);
                        if (!MemCard.GetBlockTitle(1, stream).Contains("FF9/"))
                            isFF9 = false;
                        byte[] header = MemCard.GetBlockHeader(stream, 1);
                        byte[] data = MemCard.GetBlockData(stream, 1);

                        StackUndoBuffer();

                        //start importing
                        int offset = blockIndex * MemCard.BLOCK_HEADER_SIZE;
                        foreach (byte b in header) mainBufferCopy[offset++] = b;
                        offset = blockIndex * MemCard.SAVE_BLOCK_SIZE;
                        foreach (byte b in data) mainBufferCopy[offset++] = b;


                        Importing = true;
                        GC.Collect(); GC.WaitForPendingFinalizers();
                    }
                }

                //TODO: DRY fail
                else if (tag == 200) //Export SS
                {
                    saveFileDialog.Filter = FILE_FILTER_SINGLE;
                    saveFileDialog.FileName = "";
                    saveFileDialog.Title = "Export Single Save Format";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        file = saveFileDialog.FileName;
                        byte[] stream = MemCard.ToType(mainBufferCopy, Type, blockIndex, extraHeader);
                        using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
                        {
                            fs.Write(stream, 0, stream.Length);
                            fs.Close();
                        }
                        GC.Collect(); GC.WaitForPendingFinalizers();
                    }
                }
                else
                    return;

                file = temp;

                if (Importing)
                {
                    FillGrid(mainBufferCopy, false);
                    if (isFF9)
                    {
                        string reg = (string)gridView.Rows[currentRow].Cells["Reg"].Value;
                        if (!string.IsNullOrEmpty(reg))
                        {
                            fillAgain = true;
                            updateSlotNr(reg);
                            //Do it twice in case imported region does not exist on card
                            //(which would generate the 2nd free slot, not the 1st).
                            updateSlotNr(reg);
                        }
                    }

                }

                if (fillAgain)
                    FillGrid(mainBufferCopy, false);
                EnableSaveButtons();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); file = temp; }
            finally { file = temp; }
        }

        private void Help_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(
                    (Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
                    Path.DirectorySeparatorChar + "HEEELP.html").Replace("file:\\", "").Replace("file://", ""));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            //MessageBox.Show("Nothing yet."); 
            //HexEdit h = new HexEdit( 0, 500, this);
            //h.ShowDialog();
        }

        private void About_Click(object sender, EventArgs e)
        {
            if (about == null)
            {
                about = new About(this);
                ((About)about).Show();
            }
            else
                ((About)about).Focus();
        }

        private void Format_Click(object sender, EventArgs e)
        {
            if (info.IsRR2016SaveType) return;
            if (!fileOpen)
            {
                MessageBox.Show("No file is open. Nothing formated.");
                return;
            }
            try
            {
                DialogResult res = DialogResult.No;
                res = MessageBox.Show("Delete all saves and format card?\r\nIt is possible to undo this action.", "Format card?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    StackUndoBuffer();
                    MemCard.FormatCard(ref mainBufferCopy);
                    FillGrid(mainBufferCopy, false);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        #endregion

        #region GridView

        private void gridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (fileOpen)
                SelectSave_Click(null, null);
        }

        private void gridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space && fileOpen)
            {
                spaceWasPushed = true;
                SelectSave_Click(null, null);
                timer.Enabled = true; timer.Start();
            }
        }

        private void gridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            bool canCheckInfoAndCopy = false, canImport = false;
            if (fileOpen && gridView.Rows[e.RowIndex].Cells.Count > 1)
            {
                blockIndex = Convert.ToByte(gridView.Rows[e.RowIndex].Cells["Block"].Value);
                string v = (string)gridView.Rows[e.RowIndex].Cells["Party"].Value;
                canOpenASlot = (v != "Empty slot") & (v != "Other title");
                canCheckInfoAndCopy = (v != "Empty slot");
                canImport = CanCopyOrPasteBlock(false) || info.IsRR2016SaveType;
            }
            SetToolButtonsEnabled(canImport, canCheckInfoAndCopy, canOpenASlot);
        }

        private void gridView_SelectionChanged()
        {
            bool canCheckInfoAndCopy = false, canImport = false;
            if (fileOpen && gridView.SelectedRows.Count > 0)
            {
                if (gridView.SelectedRows[0].Cells.Count > 1)
                {
                    blockIndex = Convert.ToByte(gridView.SelectedRows[0].Cells["Block"].Value);
                    string v = (string)gridView.SelectedRows[0].Cells["Party"].Value;
                    canOpenASlot = (v != "Empty slot") & (v != "Other title");
                    canCheckInfoAndCopy = (v != "Empty slot");
                }
                canImport = CanCopyOrPasteBlock(false) || info.IsRR2016SaveType;
            }
            SetToolButtonsEnabled(canImport, canCheckInfoAndCopy, canOpenASlot);
        }

        private void SetToolButtonsEnabled(bool canImport, bool canCopy, bool canOpen)
        {
            btnConvert.Enabled = btnConvert2.Enabled = btnConvert3.Enabled = canOpenASlot && !info.IsRR2016SaveType;

            toolStripButtonCopy.Enabled = copyToolStripMenuItem.Enabled = copyToolStripMenuItem1.Enabled =
            toolStripButtonCut.Enabled = cutToolStripMenuItem.Enabled = cutToolStripMenuItem1.Enabled =
            toolStripButtonDelete.Enabled = deleteToolStripMenuItem1.Enabled = deleteToolStripMenuItem.Enabled =
            btnMoreInfo.Enabled = btnMoreInfo2.Enabled = canCopy && !info.IsRR2016SaveType;

            //Import
            importToolStripMenuItem.Enabled = toolStripMenuItemImport.Enabled = toolStripSplitButtonImport.Enabled =
            ImportSSToolStripMenuItem.Enabled =
            ImportSSToolStripMenuItem1.Enabled = ImportSSToolStripMenuItem2.Enabled = canImport;

            //Cannot import template if RR2016 type.
            templateToolStripMenuItem.Enabled =
            templateToolStripMenuItemImportTemplate.Enabled = memoriaTemplateToolStripMenuItem.Enabled = canImport && !info.IsRR2016SaveType;

            //Export && edit
            toolStripButtonEdit.Enabled = editToolStripMenuItem.Enabled = EditToolStripMenuItem2.Enabled =
            exportToolStripMenuItem.Enabled = toolStripMenuItemExport.Enabled = ExportSSToolStripMenuItem2.Enabled =
            ExportSSToolStripMenuItem1.Enabled = toolStripSplitButtonExport.Enabled = ExportSSToolStripMenuItem.Enabled = canOpen;

            //Cannot export template if RR2016 type.
            templateToolStripMenuItem2.Enabled = templateToolStripMenuItemExportTemplate.Enabled =
                templateToolStripMenuItem1.Enabled = canOpen && !info.IsRR2016SaveType;

            selectSaveIDToolStripMenuItem.Enabled = info.IsRR2016SaveType;
        }

        public void FillGrid(byte[] buffer, bool changeSelection)
        {
            info.type = Type;
            this.SuspendLayout();
            gridView.SuspendLayout();
            if (info.IsRR2016SaveType)
            {
                if (buffer.Length != SaveFile_RR.SAVEFILE_DECRYPTED_SLOT_SIZE)
                    throw new Exception("This is not a valid save file");
                gridView.ClearSelection();
                gridView.Rows.Clear();
                BlockNrs = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int leaderID = 0;
                for (int i = SaveMap.PARTY_START_OFFSET_RR; i < SaveMap.PARTY_START_OFFSET_RR + 4;i++)
                {
                    if(mainBufferCopy[i] < byte.MaxValue)
                    {
                        leaderID = mainBufferCopy[i];
                        break;
                    }
                }
                if (leaderID > 8) leaderID = 0;

                DataGridViewDisableButtonCell c;
                DataGridViewRow row = new DataGridViewRow();
                gridView.Rows.Add(row);

                gridView.Rows[0].Cells["Block"].Value = 1;
                gridView.Rows[0].Cells["Slot"].Value = saveID_rr2016 + 1;
                gridView.Rows[0].Cells["Leader"].Value =
                    System.Text.Encoding.Default.GetString(mainBufferCopy,
                    SaveMap.CHARACTER_SECTION_START_RR + 57 + (SaveMap.CHARACTER_SECTION_LENGTH_RR * leaderID), 9) + 
                    "lvl " + mainBufferCopy[SaveMap.CHARACTER_SECTION_START_RR + 48 + (SaveMap.CHARACTER_SECTION_LENGTH_RR * leaderID)];
                gridView.Rows[0].Cells["Disc"].Value = 1;
                gridView.Rows[0].Cells["Reg"].Value = "";
                framerate = 1;
                double time = BitConverter.ToDouble(mainBufferCopy, SaveMap.PLAYTIME_AS_DOUBLE_OFFSET_RR);
                gridView.Rows[0].Cells["PlayTime"].Value = 
                    FormatTimeText(Convert.ToUInt32(time));
                gridView.Rows[0].Cells["Gil"].Value = OffsetManager.Swap_Bytes(mainBufferCopy, SaveMap.GIL_OFFSET_RR + 2, 3);
                gridView.Rows[0].Cells["Loc"].Value = "";//locations[k]; l++;

                    gridView.ClearSelection();
                fileOpen = true;
                gridView.Rows[0].Selected = true;
                saveAsStripMenuItem.Enabled = true;
                gridView_SelectionChanged();
                gridView.CurrentCell = gridView[0, 0];

            }
            else
            {
                if (buffer.Length != MemCard.MEMORY_CARD_SIZE)
                    throw new Exception("This is not a valid memory card");
                int selectedIndex = 0;
                if (!changeSelection)
                    selectedIndex = gridView.SelectedRows[0].Index;
                canOpenASlot = false;
                fileOpen = false;
                saveAsStripMenuItem.Enabled = false;

                //PSX.FileInfo file = new PSX.FileInfo();


                gridView.ClearSelection();
                gridView.Rows.Clear();

                int k = 0;
                bool[] emptyBlocks = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
                bool[] otherTitles = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
                bool[] drawBlocks = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
                byte[] discNrsTrue = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                BlockNrs = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                string[] regions = new string[15];
                for (int i = MemCard.BLOCK_HEADER_SIZE, j = 0;
                    i < MemCard.BLOCK_HEADER_SIZE * info.MaxNumberOfSaves + 1;
                    i += MemCard.BLOCK_HEADER_SIZE, j++)
                {
                    k = i + MemCard.REGION_CODE_OFFSET;
                    if ((buffer[k] == 0 && buffer[k + 1] == 0 && buffer[k + 2] == 0 && buffer[k + 3] == 0) &&
                        buffer[i + MemCard.BLOCK_HEADER_SIZE - 1] == 0xA0 &&
                        buffer[i] == 0xA0)
                    {
                        emptyBlocks[j] = true; drawBlocks[j] = true;
                        BlockNrs[j] = (byte)j; continue;
                    }
                    else if (buffer[i] == 82 || buffer[i] == 83)
                    {
                        //BlockNrs[j] = (byte)j;
                        continue;
                    }

                    if (buffer[k + 1] != '\x45' && buffer[k + 1] != '\x41' && buffer[k + 1] != '\x49' && buffer[k] != '\x42' && buffer[i] != 0x51)
                        continue;
                    byte[] check = { buffer[k], buffer[k + 1], buffer[k + 2], buffer[k + 3], buffer[k + 4], buffer[k + 5], buffer[k + 6] };
                    byte[] check2 = { buffer[k + 7], buffer[k + 8], buffer[k + 9], buffer[k + 10], buffer[k + 11],
                                  buffer[k + 12], buffer[k + 13], buffer[k + 14], buffer[k + 15], buffer[k + 16]
                                };
                    bool cont = false;

                    //DRY fail under ;)
                    #region check
                    //NTCS US Check
                    if (buffer[k + 1] == '\x41')
                    {
                        if (CheckCharArray(SaveMap.NTSC, check))
                            CheckRegioninfo(NTSCDiscs, "US", out cont, ref regions, ref check2, j);
                    }

                    //NTCS JAP Check
                    else if (buffer[k + 1] == '\x49')
                    {
                        if (CheckCharArray(SaveMap.NTSCJAP, check))
                            CheckRegioninfo(NTSCJAPDiscs, "Japan", out cont, ref regions, ref check2, j);
                    }

                    //PAL Check
                    else if (buffer[k + 1] == '\x45')
                    {

                        if (CheckCharArray(SaveMap.PAL, check))
                        {
                            //EU
                            if (buffer[k + 11] == 0x35)
                                CheckRegioninfo(PALDiscs, "EU", out cont, ref regions, ref check2, j);
                            //FRENCH
                            else if (buffer[k + 11] == 0x36)
                                CheckRegioninfo(PALFrDiscs, "France", out cont, ref regions, ref check2, j);
                            //GERMAN
                            else if (buffer[k + 11] == 0x37)
                                CheckRegioninfo(PALGrDiscs, "Germany", out cont, ref regions, ref check2, j);
                            //ItalyN
                            else if (buffer[k + 11] == 0x38)
                                CheckRegioninfo(PALItDiscs, "Italy", out cont, ref regions, ref check2, j);
                            //SPAIN
                            else if (buffer[k + 11] == 0x39)
                                CheckRegioninfo(PALSpDiscs, "Spain", out cont, ref regions, ref check2, j);
                        }
                    }

                    check2 = check = null;

                    if (!cont)
                    {
                        otherTitles[j] = true;
                        drawBlocks[j] = true;
                        continue;
                    }

                    #endregion

                    string s = Convert.ToChar(buffer[k + 18]).ToString() + Convert.ToChar(buffer[k + 19]);
                    SlotNrs[j] = Convert.ToByte(s);
                    SlotNrs[j] += 1;
                    BlockNrs[j] = (byte)(j + 1);
                    discNrsTrue[j] = (byte)(buffer[MemCard.SAVE_BLOCK_SIZE * BlockNrs[j] + 0x1C5] + 1);
                    drawBlocks[j] = true;


                    GC.Collect(); GC.WaitForPendingFinalizers();
                }
                uint[] gameTimes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                uint[] gils = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                for (int i = MemCard.SAVE_BLOCK_SIZE, j = 0; i < MemCard.MEMORY_CARD_SIZE; i += MemCard.SAVE_BLOCK_SIZE, j++)
                {
                    if (!drawBlocks[j] || emptyBlocks[j] || otherTitles[j])
                        continue;

                    k = SaveMap.PREVIEW_TIME_OFFSET + i;
                    byte[] var = { buffer[k], buffer[k + 1], buffer[k + 2], buffer[k + 3] };
                    gameTimes[j] = OffsetManager.Swap_Bytes(var, 3, 4);

                    k = SaveMap.PREVIEW_GIL_OFFSET + i;
                    var = new byte[] { buffer[k], buffer[k + 1], buffer[k + 2] };//, buffer[k + 3] }; //ELLER er d 4 bytes?
                    gils[j] = OffsetManager.Swap_Bytes(var, 2, 3);

                    locations[j] = LoadText(28, SaveMap.PREVIEW_NAME_OF_CURRENT_LOCATION_OFFSET + i, mainBufferCopy);
                    partyLeaders[j] = LoadText(8, SaveMap.PREVIEW_PARTY_LEADER_NAME_OFFSET + i, mainBufferCopy);

                    GC.Collect(); GC.WaitForPendingFinalizers();
                }

                k = -1; byte l = 0; bool anyFile = true;
                foreach (bool b in drawBlocks)
                {
                    k++;
                    if (!b)
                        continue;
                    DataGridViewDisableButtonCell c;
                    DataGridViewRow row = new DataGridViewRow();
                    gridView.Rows.Add(row);

                    if (otherTitles[k] || emptyBlocks[k])
                    {
                        anyFile = false;
                        c = (DataGridViewDisableButtonCell)gridView.Rows[l].Cells[0];
                        c.Enabled = false;
                        gridView.Rows[l].Cells["Block"].Value = k + 1;
                        BlockNrs[k] = 0; //none editable block.
                    }
                    if (emptyBlocks[k])
                    {
                        gridView.Rows[l].Cells["Party"].Value = "Empty slot"; l++;
                    }
                    else if (otherTitles[k])
                    {
                        gridView.Rows[l].Cells["Party"].Value = "Other title"; l++;
                    }
                    else if (!otherTitles[k] && !emptyBlocks[k])
                    {
                        anyFile = false;
                        gridView.Rows[l].Cells["Block"].Value = BlockNrs[k];
                        gridView.Rows[l].Cells["Slot"].Value = SlotNrs[k];
                        gridView.Rows[l].Cells["Leader"].Value = partyLeaders[k] +
                        " lvl " + mainBufferCopy[(MemCard.SAVE_BLOCK_SIZE * BlockNrs[k]) + SaveMap.PREVIEW_PARTY_LEADER_LEVEL_OFFSET];
                        gridView.Rows[l].Cells["Disc"].Value = discNrsTrue[k];
                        gridView.Rows[l].Cells["Reg"].Value = regions[k];
                        framerate = (byte)((regions[k] == "US" || regions[k] == "Japan") ? 60 : 50);
                        gridView.Rows[l].Cells["PlayTime"].Value = FormatTimeText(gameTimes[k]);
                        gridView.Rows[l].Cells["Gil"].Value = gils[k];
                        gridView.Rows[l].Cells["Loc"].Value = locations[k]; l++;
                    }
                }
                if (!anyFile)
                {
                    gridView.ClearSelection();
                    fileOpen = true;
                    gridView.Rows[selectedIndex].Selected = true;
                    saveAsStripMenuItem.Enabled = true;
                    gridView_SelectionChanged();
                    gridView.CurrentCell = gridView[0, selectedIndex];
                }
            }
            DoResize();
            gridView.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void gridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    gridView.CurrentCell = gridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
            }
            catch (Exception ex) { statusLabel.Text = ex.Message; }
        }

        private void gridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0 || e.ColumnIndex == 3)
            {

                DataGridViewDisableButtonCell c =
                    (DataGridViewDisableButtonCell)gridView.Rows[e.RowIndex].Cells[0];

                if (c.Enabled)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All
                        & ~DataGridViewPaintParts.ContentForeground);

                    if (e.ColumnIndex == 0)
                        e.Graphics.DrawImage(prop,
                            e.CellBounds.X + ((prop.Size.Width / 3) - (e.CellBounds.X / 10)),
                            e.CellBounds.Y + (prop.Size.Height / 10), 16, 16);

                    else if (e.ColumnIndex == 3)
                    {
                        byte bb = 1;
                        int start;
                        byte[] members = new byte[4];

                        if(!info.IsRR2016SaveType)
                        {
                            bb = Convert.ToByte(gridView.Rows[e.RowIndex].Cells["Block"].Value);
                            start = (MemCard.SAVE_BLOCK_SIZE * bb) + SaveMap.PARTY_MEMBER1_OFFSET;
                        }
                        else
                            start = SaveMap.PARTY_START_OFFSET_RR;

                        for (int i = start, j = 0; i < start + 4; i++, j++)
                            members[j] = mainBufferCopy[i];
                        int xPos = e.CellBounds.X + (e.CellBounds.Width - 76) / 2,
                            yPos = e.CellBounds.Y + (e.CellBounds.Height - 18) / 2,
                            xInc = 18 + (e.CellBounds.Width / xPos > 0 ? xPos : 1) + 1;
                        foreach (byte b in members)
                        {
                            if (b < 9)
                            {
                                switch (b)
                                {
                                    case 0:
                                        e.Graphics.DrawImage(Zidane, xPos, yPos, 18, 18);
                                        break;
                                    case 1:
                                        e.Graphics.DrawImage(Vivi, xPos, yPos, 18, 18);
                                        break;
                                    case 2:
                                        e.Graphics.DrawImage(Dagger, xPos, yPos, 18, 18);
                                        break;
                                    case 3:
                                        e.Graphics.DrawImage(Steiner, xPos, yPos, 18, 18);
                                        break;
                                    case 4:
                                        e.Graphics.DrawImage(Freya, xPos, yPos, 18, 18);
                                        break;
                                    case 5:
                                        e.Graphics.DrawImage(Quina, xPos, yPos, 18, 18);
                                        break;
                                    case 6:
                                        e.Graphics.DrawImage(Eiko, xPos, yPos, 18, 18);
                                        break;
                                    case 7:
                                        e.Graphics.DrawImage(Amarant, xPos, yPos, 18, 18);
                                        break;
                                    case 8:
                                        e.Graphics.DrawImage(Beatrix, xPos, yPos, 18, 18);
                                        break;
                                    case 9:
                                        e.Graphics.DrawImage(Blank, xPos, yPos, 18, 18);
                                        break;
                                    case 10:
                                        e.Graphics.DrawImage(Marcus, xPos, yPos, 18, 18);
                                        break;
                                    case 11:
                                        e.Graphics.DrawImage(Cinna, xPos, yPos, 18, 18);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            xPos += xInc;
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        private void gridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || spaceWasPushed || !fileOpen)
                return;
            gridView_RowEnter(null, e);
            if (e.ColumnIndex == 0)
            {
                DataGridViewDisableButtonCell c =
                    (DataGridViewDisableButtonCell)gridView.Rows[e.RowIndex].Cells[0];
                if (c.Enabled)
                    SelectSave_Click(null, null);
            }

        }

        #endregion

        #region Prev. Opened files

        private void RearrangePrevOpenedFiles(string path)
        {
            //TODO: Add OS check (oss[i] or os.platform).
            if (!oss[0]) return;
            if (string.IsNullOrEmpty(path))
                return;
            int i = 0;
            while (prevOpenedFiles.Contains(path))
            {
                prevOpenedFiles.Remove(path);
            }
            prevOpenedFiles.Insert(0, path);
            foreach (string s in prevOpenedFiles)
            {
                GRegistry.SetRegValue(regKey, "_" + i, s, RegistryValueKind.String);
                i++;
                if (i > 15)
                    break;
            }
            FillPrevOpenedFiles(false);
        }

        /// <summary>
        /// Fills the appropriate menu items with previously opened files, if they excist. 
        /// </summary>
        /// <param name="checkPrevOpened">
        /// If true, fills in all previously opened files by registry before adding new ones.
        /// </param>
        private void FillPrevOpenedFiles(bool checkPrevOpened)
        {
            //TODO: Add OS check (oss[i] or os.platform).
            if (checkPrevOpened)
                AddPrevOpenedFilesToMenuByKey();
            toolStripSplitButtonOpen.DropDownItems.Clear();
            toolStripMenuFile.DropDownItems.Clear();
            this.toolStripMenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsStripMenuItem,
            this.toolStripSeparator1,
            this.formatToolStripMenuItem,
            this.toolStripSeparator16,
            this.exitStripMenuItem1});
            bool first = true;
            ToolStripMenuItem item;
            string name = "itemss";
            string[] files = new string[prevOpenedFiles.Capacity];
            byte b = 0;
            foreach (string s in prevOpenedFiles)
            {
                if (!string.IsNullOrEmpty(s) && File.Exists(s))
                {
                    bool insert = true;
                    foreach (string ss in files)
                    {
                        if (ss == s)
                        {
                            insert = false;
                            break;
                        }
                    }
                    if (insert)
                    {
                        toolStripSplitButtonOpen.DropDownItems.Add(s, null, PrevOpenedFiles_Click);
                        if (first)
                        {
                            toolStripMenuFile.DropDownItems.Add(seperator);
                            first = false;
                        }
                        item = new ToolStripMenuItem(s, null, PrevOpenedFiles_Click, name + b);
                        toolStripMenuFile.DropDownItems.Add(item);
                    }
                }
                files[b] = s;
                b++;
            }
        }

        /// <summary>
        /// Adds all previously opened files to file menu item which exist in windows registry.
        /// </summary>
        private void AddPrevOpenedFilesToMenuByKey()
        {
            //TODO: Add OS check (oss[i] or os.platform).
            if (!oss[0]) return;
            for (byte i = 0; i < 16; i++)
            {
                string add = (string)GRegistry.GetRegValue(regKey, "_" + i, null);
                if (!String.IsNullOrEmpty(add) && File.Exists(add))
                    prevOpenedFiles.Add(add);
            }
        }

        #endregion

        #region Misc.

        protected void Tick(object sender, EventArgs e)
        {
            spaceWasPushed = false;
            timer.Stop(); timer.Enabled = false;
        }

        private void SelectSave_Resize(object sender, EventArgs e)
        {
            DoResize();
        }

        /// <summary>
        /// Adjust the control sizes upon form rezise.
        /// </summary>
        private void DoResize()
        {
            SendScreenToReg(regKey);
        }

        /// <summary>
        /// Returns true if mainBuffer equals mainBufferCopy.
        /// </summary>
        /// <returns>True if mainBuffer equals mainBufferCopy.</returns>
        private bool CheckBuffers()
        {
            int i = 0;
            foreach (byte b in mainBuffer)
            {
                if (b != mainBufferCopy[i])
                    return false;
                i++;
            }
            return true;
        }

        /// <summary>
        /// Enables or unenables save based on the buffers.
        /// </summary>
        public void EnableSaveButtons()
        {
            bool b = toolStripButtonSave.Enabled = saveToolStripMenuItem.Enabled = !CheckBuffers();
            if (b)
                SetFormText(file + '*');
            else
                SetFormText(file);
        }

        private void SelectSave_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closing)
            {
                DialogResult res = DialogResult.No;
                if (!CheckBuffers() && fileOpen)
                    res = MessageBox.Show("File not saved. Save before exit?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes && fileOpen)
                    Save_Click(null, null);
                if (res == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// Sets the form's text based on the file.
        /// </summary>
        /// <param name="path">The file.</param>
        private void SetFormText(string path)
        {
            this.Text = Strings.TrimFolders(path) + " - " + FORM_NAME;
            toolStripStatusLabel.Text = path.TrimEnd(new char[] { '*' });
        }

        private void Mouse_Leave(object sender, EventArgs e) { statusLabel.Text = "    -"; }

        private bool CheckCharArray(char[] chars, byte[] buffer)
        {
            if (buffer.Length < chars.Length || chars.Length < buffer.Length)
                throw new Exception("CheckCharArray reports: The buffer or the char array is too short.");
            byte i = 0;
            foreach (char c in chars)
            {
                if (buffer[i] != c)
                    return false;
                i++;
            }
            return true;
        }

        public string LoadText(int length, int offset, byte[] buffer)
        {
            if (length > 48)
                throw new Exception("LoadText reports: Too long string.");

            if(info.IsRR2016SaveType)
            {
                return System.Text.Encoding.Default.GetString(buffer, offset, length);
            }

            string s = "";
            for (int i = 0; i < length; i++)
            {
                if (buffer[offset + i] == '\xFF')
                    break;

                foreach (object c in characterTable.Keys)
                {
                    if (buffer[offset + i] == Convert.ToByte(characterTable[c]))
                        s += c;
                }
            }
            return s;
        }

        public void SetFileNameTime(byte blockNr, byte[] buffer)
        {
            //Find time value in buffer
            int pos = blockNr * MemCard.SAVE_BLOCK_SIZE + SaveMap.PREVIEW_TIME_OFFSET;
            byte[] var = { buffer[pos], buffer[pos + 1], buffer[pos + 2], buffer[pos + 3] };
            uint time = OffsetManager.Swap_Bytes(var, 3, 4);


            pos = MemCard.BLOCK_HEADER_SIZE * blockNr + MemCard.REGION_CODE_OFFSET + 1;
            framerate = (byte)(mainBufferCopy[pos] == '\x45' ? 50 : 60);

            //Get real time as a string...
            string[] timet = FormatTimeText(time).Split(new char[] { ':' });
            //...and convert to hours and minutes.
            byte h = Convert.ToByte(timet[0]);
            byte m = Convert.ToByte(timet[1]);
            //int s = Convert.ToInt32(timet[2]);

            pos = blockNr * MemCard.SAVE_BLOCK_SIZE + 27; //time starts at 26.

            //Set the file name time text correctly.
            buffer[pos] = (byte)((h / 10) + 79);
            buffer[pos + 2] = (byte)((h % 10) + 79);
            buffer[pos + 6] = (byte)((m / 10) + 79);
            buffer[pos + 8] = (byte)((m % 10) + 79);
        }

        public string FormatTimeText(uint time)
        {
            string seconds, minutes, hours;
            seconds = "" + (time / framerate);
            minutes = "" + (Convert.ToUInt32(seconds) / 60);
            hours = "" + (Convert.ToUInt32(minutes) / 60);
            seconds = "" + ((time / framerate) % 60);
            minutes = "" + (Convert.ToUInt32(minutes) % 60);
            seconds = Strings.AdjustString(seconds, 2, '0');
            minutes = Strings.AdjustString(minutes, 2, '0');
            hours = Strings.AdjustString(hours, 2, '0');
            return hours + ":" + minutes + ":" + seconds;
        }

        public void CopyBuffers(byte[] source, byte[] dest)
        {
            int size = MemCard.MEMORY_CARD_SIZE;
            if (info.IsRR2016SaveType)
            {
                size = SaveFile_RR.SAVEFILE_DECRYPTED_SLOT_SIZE;
                if (source.Length != SaveFile_RR.SAVEFILE_DECRYPTED_SLOT_SIZE)
                    throw new Exception("CopyBuffers reports: Source is not a savefile.");
                if (dest.Length != SaveFile_RR.SAVEFILE_DECRYPTED_SLOT_SIZE)
                    throw new Exception("CopyBuffers reports: Destination is not a savefile.");
            }
            else
            {
                if (source.Length != MemCard.MEMORY_CARD_SIZE)
                    throw new Exception("CopyBuffers reports: Source is not a memorycard.");
                if (dest.Length != MemCard.MEMORY_CARD_SIZE)
                    throw new Exception("CopyBuffers reports: Destination is not a memorycard.");
            }
            Array.Copy(source, dest, size);
        }

        private void PushUndoRedoBuffers(byte[][] undoRedoBuffer)
        {
            for (int i = undoRedoBuffer.Length - 1; i > 0; i--) { CopyBuffers(undoRedoBuffer[i - 1], undoRedoBuffer[i]); }
            CopyBuffers(mainBufferCopy, undoRedoBuffer[0]);
        }

        private void PopUndoRedoBuffers(byte[][] undoRedoBuffer)
        {
            for (int i = 0; i < undoRedoBuffer.Length - 1; i++) { CopyBuffers(undoRedoBuffer[i + 1], undoRedoBuffer[i]); }
        }

        private void SetUndoRedoEnabled()
        {
            toolStripButtonUndo.Enabled = undoToolStripMenuItem.Enabled = (!info.IsRR2016SaveType && undoLeft > 0);
            toolStripButtonRedo.Enabled = redoToolStripMenuItem.Enabled = (!info.IsRR2016SaveType && redoLeft > 0);
        }

        /// <summary>
        /// Find the first available gameID number of the FF9 slots based on a region.
        /// </summary>
        /// <param name="region">The region to campare the code with, as it appears in the gridView.</param>
        /// <returns>The first available gameID number.</returns>
        private byte FirstAvailableGameID(string region)
        {
            List<byte> slotIndexes = new List<byte>();
            List<byte> usedIDs = new List<byte>();
            foreach (DataGridViewRow row in gridView.Rows)
            {
                if ((string)row.Cells["Reg"].Value == region)
                    slotIndexes.Add((byte)(row.Cells["Block"].Value));
            }
            string check;
            foreach (byte b in slotIndexes)
            {
                int k = (b * MemCard.BLOCK_HEADER_SIZE) + MemCard.REGION_CODE_OFFSET;
                check = "" + (char)mainBufferCopy[k + 18] + (char)mainBufferCopy[k + 19];
                usedIDs.Add(Convert.ToByte(check));
            }
            for (byte i = 0; i < 15; i++)
            {
                if (!usedIDs.Contains(i))
                    return i;
            }
            int offset = (blockIndex * MemCard.BLOCK_HEADER_SIZE) + MemCard.REGION_CODE_OFFSET;
            check = "" + (char)mainBufferCopy[offset + 18] + (char)mainBufferCopy[offset + 19];
            return Convert.ToByte(check);
        }

        /// <summary>
        /// Updates file number in block title.
        /// </summary>
        /// <param name="number">Zero based file number.</param>
        private void SetFileNumber(byte number)
        {
            char[] digits = Strings.GetSlotGameIDnr(number);
            mainBufferCopy[(blockIndex * MemCard.SAVE_BLOCK_SIZE) + 21] = (byte)(digits[0] + 31);
            mainBufferCopy[(blockIndex * MemCard.SAVE_BLOCK_SIZE) + 23] = (byte)(digits[1] + 32);
            SetChecksum(blockIndex, ref mainBufferCopy);
        }

        private void CheckRegioninfo(char[][] discCode, string region, out bool cont, ref string[] regions, ref byte[] check2, int j)
        {
            byte disc = 0;
            foreach (char[] cs in discCode)
            {
                disc++;
                if (cont = CheckCharArray(cs, check2))
                {
                    discNrs[j] = disc;
                    regions[j] = region;
                    return;
                }
            }
            cont = false;
        }

        public void SetChecksum(byte blockNr, ref byte[] memcardBuffer)
        {
            Crypto.Crc16Ccitt c16 = new Crypto.Crc16Ccitt(0xFFFF, 0x8408);
            byte[] block;

            block = new byte[0x13FE];
            int start = MemCard.SAVE_BLOCK_SIZE * blockNr;
            for (int i = start, j = 0; i < start + 0x13FE; i++, j++)
                block[j] = memcardBuffer[i];
            int k = start + 0x13FE;
            foreach (byte b in OffsetManager.Flap_Values(c16.ComputeChecksum(block), 2))
            {
                memcardBuffer[k] = b; k++;
            }
        }

        private void selectSaveIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!info.IsRR2016SaveType) return;
                rr2016Selector = new RR2016_Selector();
                rr2016Selector.ShowDialog(this);
                slotID_rr2016 = rr2016Selector.slotID;
                saveID_rr2016 = rr2016Selector.saveID;
                if(saveID_rr2016 > -1 && slotID_rr2016 > -1)
                {
                    byte[] stream;
                    ReUtils.DataManager storage = new ReUtils.DataManager();
                    storage.Load(slotID_rr2016, saveID_rr2016, file, out stream);

                    mainBuffer = new byte[stream.Length];
                    mainBufferCopy = new byte[stream.Length];
                    Array.Copy(stream, mainBuffer, stream.Length);
                    Array.Copy(stream, mainBufferCopy, stream.Length);
                    FillGrid(mainBuffer, true);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void updateSlotNr(string region)
        {
            byte firstID = FirstAvailableGameID(region);
            char[] slotNr = Strings.GetSlotGameIDnr(firstID);
            int offset = (blockIndex * MemCard.BLOCK_HEADER_SIZE) + MemCard.REGION_CODE_OFFSET;
            mainBufferCopy[offset + 18] = (byte)slotNr[0];
            mainBufferCopy[offset + 19] = (byte)slotNr[1];
            MemCard.CalaculateHeaderChecksum(blockIndex, mainBufferCopy);
            SetFileNumber(firstID);
        }

        #endregion
    }
}