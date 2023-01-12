using System;
using System.Drawing;
using System.Windows.Forms;
using Memoria.PSX;
using Memoria.BaseOperations;
using Memoria.Override;
using Memoria.Properties;


namespace Memoria
{
    public partial class Save : BaseForm
    {

        #region Constants

        #endregion

        #region Variables

        public bool IsNormalMemCard
        {
            get
            {
                return !master.info.IsRR2016SaveType;
            }
        }

        /// <summary>
        /// Returns currentBlock * MemCard.SAVE_BLOCK_SIZE if normal; else 0;
        /// </summary>
        public int CurrentOffset
        {
            get
            {
                if (!IsNormalMemCard) return 0;
                return currentBlock * MemCard.SAVE_BLOCK_SIZE;
            }
        }

        /// <summary>
        /// Current block on the memmory card we are working on. 1 - 15. Multiply this with SAVE_BLOCK_SIZE to find offset 0 of current block. 
        /// </summary>
        public byte currentBlock = 1;

        /// <summary>
        /// The SelectSave Form. To access variables from it.
        /// </summary>
        SelectSave master = null;

        /// <summary>
        /// Character that is being edited. 1 - 9.
        /// </summary>
        byte characterID = 1;

        /// <summary>
        /// CharacterID that is party leader. 1 - 9.
        /// </summary>
        byte partyLeaderID = 1;

        string partyLeaderName = "";

        /// <summary>
        /// The buffer will not update if this is false.
        /// </summary>
        bool updateBuffer = false;

        /// <summary>
        /// The cards in the current block.
        /// </summary>
        Card[] cards;// = new Card[SaveMap.CARD_MAX_NUMBERS];

        /// <summary>
        /// Buffer containing all the bytes from the current memory card. This buffer is passed down if "ok" or "apply" are clicked.
        /// </summary>
        public byte[] editBuffer = null;

        /// <summary>
        /// The party leader indexes of each Block, 1 - 9. 0 = no partyleader/empty block/other title.
        /// </summary>
        byte[] partyLeaders = new byte[15];

        /// <summary>
        /// Contains ifno on all items. All entries have item index followed by item name in this format: INDEX = ITEMNAME.
        /// </summary>
        string[] items;

        /// <summary>
        /// The index of the selected item being edited.
        /// </summary>
        byte selectedItemIndex;

        /// <summary>
        /// Previous Quantity of selected item, before edited. 
        /// If editing item quantity throws an exception, the edited cell will get this value.
        /// Update this value when changing rows and before selected item quantity is changed.
        /// </summary>
        byte previousQuantity;

        byte[] blocksNrs
        {
            get
            {
                if (toolStripDropDownButton == null) return null;
                if (toolStripDropDownButton.DropDownItems.Count < 2) return null;

                byte[] b = new byte[toolStripDropDownButton.DropDownItems.Count]; int i = 0;
                foreach (ToolStripDropDownItem item in toolStripDropDownButton.DropDownItems)
                {
                    b[i] = Byte.Parse(item.Text); i++;
                }
                return b;
            }
        }

        /// <summary>
        /// Control types which value is allowed to update buffer when changed.
        /// </summary>
        Type[] validControlUpdateBufferTypes = new Type[]
        {
            typeof(NumericUpDown),
            typeof(ComboBox),
            typeof(TextBox),
            typeof(DataGridViewComboBoxCell),
            typeof(DataGridViewTextBoxCell),
            typeof(ComboBoxEx)
        };

        byte[] randomArrows = new byte[105];

        string[] AblNames;

        int Item_typeOffset = 0;
        int itemQuantityOffset = 1;

        byte Framerate
        {
            get
            {
                if (!IsNormalMemCard) return 1;
                int i = MemCard.BLOCK_HEADER_SIZE * currentBlock + MemCard.REGION_CODE_OFFSET + 1;
                return (byte)(editBuffer[i] == '\x45' ? 50 : 60);
            }
        }

        #endregion

        #region Card Control

        /// <summary>
        /// The panel which displays the cards in a grid.
        /// </summary>
        Panel[] CardPanels;// = new Panel[SaveMap.CARD_MAX_NUMBERS];

        /// <summary>
        /// Represent a card arrow directon's image and it's bit value.
        /// </summary>
        public struct CardArrowImage
        {
            /// <summary>
            /// Up card arrow value and image.
            /// </summary>
            public static CardArrowImage UP
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.UP,
                        Image = Resources.a_up,
                    };
                }
            }

            /// <summary>
            /// Down card arrow value and image.
            /// </summary>
            public static CardArrowImage DOWN
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.DOWN,
                        Image = Resources.a_down,
                    };
                }
            }

            /// <summary>
            /// Left card arrow value and image.
            /// </summary>
            public static CardArrowImage LEFT
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.LEFT,
                        Image = Resources.a_left,
                    };
                }
            }

            /// <summary>
            /// Right card arrow value and image.
            /// </summary>
            public static CardArrowImage RIGHT
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.RIGHT,
                        Image = Resources.a_right,
                    };
                }
            }

            /// <summary>
            /// Up left card arrow value and image.
            /// </summary>
            public static CardArrowImage UP_LEFT
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.UP_LEFT,
                        Image = Resources.a_up_left,
                    };
                }
            }

            /// <summary>
            /// Down left card arrow value and image.
            /// </summary>
            public static CardArrowImage DOWN_LEFT
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.DOWN_LEFT,
                        Image = Resources.a_down_left,
                    };
                }
            }

            /// <summary>
            /// Up right card arrow value and image.
            /// </summary>
            public static CardArrowImage UP_RIGHT
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.UP_RIGHT,
                        Image = Resources.a_up_right,
                    };
                }
            }

            /// <summary>
            /// Down right card arrow value and image.
            /// </summary>
            public static CardArrowImage DOWN_RIGHT
            {
                get
                {
                    return new CardArrowImage
                    {
                        Arrow = CardArrow.DOWN_RIGHT,
                        Image = Resources.a_down_right,
                    };
                }
            }

            /// <summary>
            /// Gets or sets the bit value which represents the arrow.
            /// </summary>
            public CardArrow Arrow { get; set; }

            /// <summary>
            /// Gets or sets the card arrow image.
            /// </summary>
            public Image Image { get; set; }
        }

        #endregion

        #region Constructor/Load

        public Save(SelectSave s)
        {
            updateBuffer = false;
            InitializeComponent();

            panel4.Height = CalcNewValue(panel4.Height);
            panel5.Width = CalcNewValue(panel5.Width);
            tabControlCharacter.Width = CalcNewValue(tabControlCharacter.Width);

            this.master = s;
            SetFormSizeFromReg(master.regKey);

            if (oss[0])
            {
                this.Icon = Resources.mem;
            }
            else
            {
                this.Icon = null;
                this.ShowIcon = false;
            }



            #region Fill Comboboxes

            imageList.Images.AddRange(new Image[]
            {
                Resources.Zidane,
                Resources.Vivi,
                Resources.Dagger,
                Resources.Steiner,
                Resources.Freya,
                Resources.Quina,
                Resources.Eiko,
                Resources.Amarant,
                Resources.Beatrix,
                new Bitmap(32, 44)
            });
            cbPartyMember1.ImageList = cbPartyMember2.ImageList =
            cbPartyMember3.ImageList = cbPartyMember4.ImageList = imageList;
            cbPartyMember1.DropDownStyle = cbPartyMember2.DropDownStyle =
            cbPartyMember3.DropDownStyle = cbPartyMember4.DropDownStyle = ComboBoxStyle.DropDownList;

            object[] cbItems = new object[] {
                new ComboBoxExItem("", 0),
                new ComboBoxExItem("", 1),
                new ComboBoxExItem("", 2),
                new ComboBoxExItem("", 3),
                new ComboBoxExItem("", 4),
                new ComboBoxExItem("", 5),
                new ComboBoxExItem("", 6),
                new ComboBoxExItem("", 7),
                new ComboBoxExItem("", 8),
                new ComboBoxExItem("", 9),
                /*new ComboBoxExItem("", 10),
                new ComboBoxExItem("", 11)*/
            };

            cbPartyMember1.Items.AddRange(cbItems);
            cbPartyMember2.Items.AddRange(cbItems);
            cbPartyMember3.Items.AddRange(cbItems);
            cbPartyMember4.Items.AddRange(cbItems);

            cbAttackType.Items.Add(CardAttackType.P);
            cbAttackType.Items.Add(CardAttackType.M);
            cbAttackType.Items.Add(CardAttackType.X);
            cbAttackType.Items.Add(CardAttackType.A);
            cbCardsAllAType.Items.Add(CardAttackType.P);
            cbCardsAllAType.Items.Add(CardAttackType.M);
            cbCardsAllAType.Items.Add(CardAttackType.X);
            cbCardsAllAType.Items.Add(CardAttackType.A);
            string[] cardTypes = new string[]
            {
                    "Goblin",
                    "Fang",
                    "Skeleton",
                    "Flan",
                    "Zaghnol",
                    "Lizard Man",
                    "Zombie",
                    "Bomb",
                    "Ironite",
                    "Sahagin",
                    "Yeti",
                    "Mimic",
                    "Wyerd",
                    "Mandragora",
                    "Crawler",
                    "Sand Scorpion",
                    "Nymph",
                    "Sand Golem",
                    "Zuu",
                    "Dragonfly",
                    "Carrion Worm",
                    "Cerberus",
                    "Antlion",
                    "Cactuar",
                    "Gimme Cat",
                    "Ragtimer",
                    "Hedgehog Pie",
                    "Ralvuimahgo",
                    "Ochu",
                    "Troll",
                    "Blazer Beetle",
                    "Abomination",
                    "Zemzelett",
                    "Stroper",
                    "Tantarian",
                    "Grand Dragon",
                    "Feather Circle",
                    "Hecteyes",
                    "Ogre",
                    "Armstrong",
                    "Ash",
                    "Wraith",
                    "Gargoyle",
                    "Vepal",
                    "Grimlock",
                    "Tonberry",
                    "Veteran",
                    "Garuda",
                    "Malboro",
                    "Mover",
                    "Abadon",
                    "Behemoth",
                    "Iron Man",
                    "Nova Dragon",
                    "Ozma",
                    "Hades",
                    "Holy",
                    "Meteor",
                    "Flare",
                    "Shiva",
                    "Ifrit",
                    "Ramuh",
                    "Atomos",
                    "Odin",
                    "Leviathan",
                    "Bahamut",
                    "Ark",
                    "Fenrir",
                    "Madeen",
                    "Alexander",
                    "Excalibur 2",
                    "Ultima Weapon",
                    "Masamune",
                    "Elixir",
                    "Dark Matter",
                    "Ribbon",
                    "Tiger Racket",
                    "Save The Queen",
                    "Genji",
                    "Mythril Sword",
                    "Blue Narciss",
                    "Hilda Garde 3",
                    "Invincible",
                    "Cargo Ship",
                    "Hilda Garde 1",
                    "Red Rose",
                    "Theater Ship",
                    "Viltgance",
                    "Chocobo",
                    "Fat Chocobo",
                    "Mog",
                    "Frog",
                    "Oglop",
                    "Alexandria",
                    "Lindblum",
                    "Two Moons",
                    "Gargant",
                    "Namingway",
                    "Boco",
                    "Airship", "NONE"
            };

            cbCardType.Items.AddRange(cardTypes);
            cbCardsAllType.Items.AddRange(cardTypes);

            items = Resources.Items.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            cbItemName.Items.AddRange(items);

            cbEquipArms.Items.AddRange(items);
            cbEquipBody.Items.AddRange(items);
            cbEquipedAddOn.Items.AddRange(items);
            cbEquipHead.Items.AddRange(items);
            cbEquipWeapon.Items.AddRange(items);
            cbEquipArms.Items.RemoveAt(items.Length - 1);
            cbEquipBody.Items.RemoveAt(items.Length - 1);
            cbEquipedAddOn.Items.RemoveAt(items.Length - 1);
            cbEquipHead.Items.RemoveAt(items.Length - 1);
            cbEquipWeapon.Items.RemoveAt(items.Length - 1);

            ItemName.Items.Clear();
            ItemName.Items.AddRange(items);
            gwItems.Rows.Clear(); gwItems.Rows.Add(256);


            #endregion

            #region AP

            //AP
            for (int i = 0, j = 88; i < 48; i++, j++)
            {
                NumericUpDown nup = new NumericUpDown();
                Label lbl = new Label();

                nup.Name = "nupAP" + i; lbl.Name = "lblAP" + i;
                nup.Tag = string.Format("{0};1;1;0;{1}", j, j + 97);
                nup.ValueChanged += new EventHandler(ControlValueChange);
                nup.Maximum = 255; nup.Minimum = 0; nup.Width = 45; nup.TabIndex = i;

                lbl.Text = "A" + (i + 1); lbl.Width = 35; lbl.TextAlign = ContentAlignment.MiddleRight;

                int Y = (i / 6) * 32 + 24, X = (i % 6) * (lbl.Width + nup.Width + 4) + 4;
                lbl.Location = new Point(X, Y + 4); nup.Location = new Point(X + lbl.Width, Y);

                tabPageAbilities.Controls.Add(nup); tabPageAbilities.Controls.Add(lbl);
            }
            Label ll = new Label(); ll.Text = ""; ll.Width = tabPageAbilities.Width - 8;
            tabPageAbilities.Controls.Add(ll);
            btnCharForgetAll.Height = btnCharAblMaster.Height = 27;
            btnCharForgetAll.TabIndex = 49; btnCharAblMaster.TabIndex = 48;
            tabPageAbilities.Controls.Add(this.btnCharForgetAll);
            tabPageAbilities.Controls.Add(this.btnCharAblMaster);

            AblNames = Resources.Abl.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < AblNames.Length; i++)
            {
                PictureBox pb = new PictureBox(); Label lbl = new Label();

                pb.Name = "pbMS" + (i + 1); pb.Width = pb.Height = 26;
                lbl.Name = "lblMS" + (i + 1); lbl.Width = 84; lbl.Text = AblNames[i];
                lbl.TextAlign = ContentAlignment.MiddleRight;

                int Y = (i / 4) * 32 + 48, X = (i % 4) * (lbl.Width + pb.Width + 4) + 4;
                lbl.Location = new Point(X, Y + 3); pb.Location = new Point(lbl.Width + X, Y);

                pb.Tag = (i / 8) + ";" + (int)Math.Pow(2, i % 8);
                pb.Click += new EventHandler(MagicStone_Click);

                tabPagemagicStones.Controls.Add(lbl); tabPagemagicStones.Controls.Add(pb);
            }

            //tabPagemagicStones.BackgroundImage = Resources.TileG;

            //end AP
            #endregion

            #region Unknowns


            //add unknown character value controls
            Font f = new Font(Font.FontFamily, fontSize > BASE_FONT_SIZE - 1.5f ? fontSize - 1.5f : MIN_FONT_SIZE);
            for (int i = 0; i < UnknownValues.UNKNOWN_CHARACTER_OFFSETS.Length; i++)
            {
                uint offset = UnknownValues.UNKNOWN_CHARACTER_OFFSETS[i];

                Label lblUC = new Label();
                lblUC.AutoSize = false;
                lblUC.ForeColor = SystemColors.ControlText;
                lblUC.Name = "lblUC" + i;
                lblUC.Size = new Size(CalcNewValue(CalcNewValue(40)), CalcNewValue(15));
                lblUC.Font = f;
                lblUC.TabIndex = i + 1;
                lblUC.Location = new Point(lblUC.Location.X, lblUC.Location.Y + 5);
                lblUC.Text = "0x" + offset.ToString("x").ToUpper();


                NumericUpDown nupCU = new NumericUpDown();
                nupCU.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
                nupCU.Name = "nupCU" + i;
                nupCU.Size = new System.Drawing.Size(CalcNewValue(50), CalcNewValue(22));
                nupCU.TabIndex = i + 2;
                nupCU.Tag = offset + ";1;1;0";
                nupCU.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
                nupCU.Value = new decimal(new int[] { 1, 0, 0, 0 });
                nupCU.ValueChanged += new System.EventHandler(this.ControlValueChange);

                Panel pnlU = new Panel();
                pnlU.Size = new Size(CalcNewValue(lblUC.Width + nupCU.Width + 16), CalcNewValue(32));

                lblUC.Location = new Point(2, (pnlU.Height - lblUC.Height) / 2);
                nupCU.Location = new Point(lblUC.Width + 4, (pnlU.Height - nupCU.Height) / 2);

                pnlU.Controls.AddRange(new Control[] { lblUC, nupCU });
                pnlU.BackColor = Color.Transparent;
                lblUC.BackColor = Color.Transparent;
                flowLayoutPanel3.Controls.Add(pnlU);
            }

            #endregion

            //foreach (Control c in panel3.Controls)
            //    SetBackgroundImages(c);
            //foreach (TabPage page in tabControlCharacter.TabPages)
            //    SetBackgroundImages(page);
            //foreach (TabPage page in tabControl.TabPages)
            //    SetBackgroundImages(page);

            characterID = 1;
            lblCardStats.BackColor = Color.FromArgb(128, 40, 120, 200);
            cbArrowD.Tag = CardArrow.DOWN;
            cbArrowU.Tag = CardArrow.UP;
            cbArrowR.Tag = CardArrow.RIGHT;
            cbArrowL.Tag = CardArrow.LEFT;
            cbArrowDR.Tag = CardArrow.DOWN_RIGHT;
            cbArrowDL.Tag = CardArrow.DOWN_LEFT;
            cbArrowUR.Tag = CardArrow.UP_RIGHT;
            cbArrowUL.Tag = CardArrow.UP_LEFT;
            panelAllArrows.Tag = CardArrow.NONE;
            cbItemOp.SelectedIndex = 0;
            cbCardsAllAttckOp.SelectedIndex = 0;
            cbCardsAllMDefOp.SelectedIndex = 0;
            cbCardsAllPDefOp.SelectedIndex = 0;
            cbAllCharOp.SelectedIndex = 0;
            Save_Resize(null, null);


            //int y = 7;
            //foreach (Control c in tabPageStats.Controls)
            //{
            //    if (c is GroupBox)
            //    {
            //        c.Width = tabPageStats.Width - 12;
            //        c.Height = CalcNewValue(c.Height);
            //        c.Location = new Point(7, y);
            //        y = c.Location.Y + c.Height + 4;
            //    }
            //}

            btnCharMaster.Location = new Point(4, gpHPMP.Height + gpLevel.Height + gpStats.Height + 24);
            btnCharWimp.Location = new Point(btnCharMaster.Width + 8, btnCharMaster.Location.Y);
            btnCharWimp.Height = btnCharMaster.Height;

            foreach (TabPage page in tabControlCharacter.TabPages)
            {
                page.BackColor = Color.White;
                page.AutoScroll = true;
            }
            foreach (TabPage page in tabControl.TabPages)
            {
                page.AutoScroll = true;
            }
            Setup();
            SetKeyDownEvents(this.Controls);
        }

        private void SetKeyDownEvents(Control.ControlCollection ctrls)
        {
            foreach (Control c in ctrls)
            {
                c.KeyDown += new KeyEventHandler(Save_KeyDown);
                if (c.HasChildren)
                    SetKeyDownEvents(c.Controls);
            }
        }

        public void Start(byte cFile, byte[] buffer, byte[] blocks)
        {
            //The current block.
            this.currentBlock = cFile;
            this.Text = "Memoria: Block " + currentBlock;

            editBuffer = new byte[buffer.Length];
            Item_typeOffset = (!IsNormalMemCard ? 1 : 0);
            itemQuantityOffset = (!IsNormalMemCard ? 0 : 1);
            Array.Copy(buffer, this.editBuffer, buffer.Length);

            if (toolStripDropDownButton == null)
            {
                toolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
                toolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
                toolStripDropDownButton.Image = ((System.Drawing.Image)Resources.Properties_24x24);
                toolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolStripDropDownButton.Name = "toolStripDropDownButton";
                toolStripDropDownButton.Size = new System.Drawing.Size(77, 20);
                toolStripDropDownButton.Text = "Select Block";
                toolStripDropDownButton.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStripDropDownButton_DropDownItemClicked);
                statusStrip1.Items.Add(toolStripDropDownButton);
            }
            else
                toolStripDropDownButton.DropDownItems.Clear();
            ToolStripMenuItem[] tsItems = new ToolStripMenuItem[blocks.Length];
            byte count = 0, added = 0;
            foreach (byte b in blocks)
            {
                if (b != 0)
                {
                    tsItems[count] = new ToolStripMenuItem(b + "", Resources.Properties_24x24);
                    toolStripDropDownButton.DropDownItems.Add(tsItems[count]);
                    added++;
                }
                else
                    tsItems[count] = null;
                count++;
            }
            if (added < 2)
            {
                statusStrip1.Items.Remove(toolStripDropDownButton);
                toolStripDropDownButton = null;
                tsItems = null;
            }

            //add general unknown value buttons to trigger hex editing.
            flowLayoutPanel2.Controls.Clear();
            if (IsNormalMemCard)
            {
                for (int i = 0; i < UnknownValues.UNKNOWN_START_OFFSETS.Length; i++)
                {
                    Button btn = new Button();
                    btn.Name = "btnU" + i;
                    btn.TabIndex = i + 2;
                    uint start = UnknownValues.UNKNOWN_START_OFFSETS[i];
                    uint stop = UnknownValues.UNKNOWN_END_OFFSETS[i] - 1;
                    btn.Text = start.ToString("x").ToUpper() + " - " + stop.ToString("x").ToUpper();
                    btn.Tag = i;
                    btn.ForeColor = Color.FromArgb(48, 48, 48);
                    btn.Click += new EventHandler(btnUnknown_Changed);
                    btn.Width = btn.Text.Length * 10; btn.Height = 27;
                    flowLayoutPanel2.Controls.Add(btn);
                }
            }
            else
            {
                Button btn = new Button(); btn.Name = "btnU";
                btn.Text = "Open Hexedit."; btn.ForeColor = Color.FromArgb(48, 48, 48);
                btn.Click += new EventHandler(btnUnknown_Changed);
                btn.Width = btn.Text.Length * 10; btn.Height = 27;
                flowLayoutPanel2.Controls.Add(btn);
            }

            Load_Variables(false);
            EnableApply();
            this.ShowDialog();
        }

        private void SetBackgroundImages(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is Label || c is GroupBox || c is CheckBox || c is Button || c is PictureBox)
                    c.BackColor = Color.Transparent;
                else if (c is TabPage)
                    c.BackgroundImage = Memoria.Properties.Resources.TileG;
                if (c is TabPage || c is GroupBox)
                {
                    foreach (Control c2 in c.Controls)
                        SetBackgroundImages(c2);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (master == null)
                    this.Close();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        #endregion

        #region Clicks

        private void SaveChanges()
        {
            if (IsNormalMemCard)
            {
                if (partyLeaderID > 0 && partyLeaderID < 8)
                {
                    byte level = editBuffer[CurrentOffset + SaveMap.CHARACTER_SECTION_START_OFFSET +
                                            ((partyLeaderID - 1) * SaveMap.CHARACTER_BLOCK_SIZE) + SaveMap.CHARACTER_LEVEL_OFFSET];
                    editBuffer[CurrentOffset + SaveMap.PREVIEW_PARTY_LEADER_LEVEL_OFFSET] = level;
                }
                UpdateOffset(SaveMap.PREVIEW_GIL_OFFSET, (uint)numericUpDown1.Value, 3);
                master.SetFileNameTime(currentBlock, editBuffer);
                master.SetChecksum(currentBlock, ref editBuffer);
                master.StackUndoBuffer();
            }
            master.CopyBuffers(editBuffer, master.mainBufferCopy);
            master.EnableSaveButtons();
            master.FillGrid(master.mainBufferCopy, false);
        }

        public void EnableApply()
        {
            //TODO: This is DRY for SelectSave.CheckBuffers. Move to base operations.
            bool needApply = false;

            int i = 0;
            foreach (byte b in editBuffer)
            {
                if (b != master.mainBufferCopy[i])
                {
                    //MessageBox.Show(i.ToString("x"));
                    needApply = true;
                    break;
                }
                i++;
            }

            btnAPPLY.Enabled = needApply;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            if (btnAPPLY.Enabled)
                SaveChanges();
            this.Close();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            SaveChanges();
            btnAPPLY.Enabled = false;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Character_Click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(Button))
            {
                try
                {
                    characterID = Convert.ToByte(((Button)sender).Tag);
                    label23.Enabled = rbAliasChar.Enabled = rbRealChar.Enabled = (characterID > 5 & characterID < 9);
                    if (label23.Enabled)
                    {
                        label23.Text = "Display as ";
                        switch (characterID)
                        {
                            case 6:
                                label23.Text += "Cinna";
                                break;
                            case 7:
                                label23.Text += "Marcus";
                                break;
                            case 8:
                                label23.Text += "Blank";
                                break;
                            default: break;
                        }
                        AliasChange(null, null);
                    }
                    Load_Variables(true);
                }
                catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        #endregion

        /// <summary>
        /// Loads all variables from the buffer and asign the values to the controls.
        /// </summary>
        /// <param name="charactersOnly">True if only the character values shall load. False if all should load.</param>
        private void Load_Variables(bool charactersOnly)
        {
            updateBuffer = false;
            this.Text = "Memoria: Block " + currentBlock;
            switch (characterID)
            {
                case 1:
                    panel1.BackgroundImage = Resources.ZidaneBig;
                    break;
                case 2:
                    panel1.BackgroundImage = Resources.ViviBig;
                    break;
                case 3:
                    panel1.BackgroundImage = Resources.DaggerBig;
                    break;
                case 4:
                    panel1.BackgroundImage = Resources.SteinerBig;
                    break;
                case 5:
                    panel1.BackgroundImage = Resources.FreyaBig;
                    break;
                case 6:
                    if (!rbRealChar.Checked)
                        panel1.BackgroundImage = Resources.QuinaBig;
                    else
                        panel1.BackgroundImage = Resources.CinnaBig;
                    break;
                case 7:
                    if (!rbRealChar.Checked)
                        panel1.BackgroundImage = Resources.EikoBig;
                    else
                        panel1.BackgroundImage = Resources.MarcusBig;
                    break;
                case 8:
                    if (!rbRealChar.Checked)
                        panel1.BackgroundImage = Resources.AmarantBig;
                    else
                        panel1.BackgroundImage = Resources.BlankBig;
                    break;
                case 9:
                    panel1.BackgroundImage = Resources.BeatrixBig;
                    break;
                default:
                    break;
            }
            if (!charactersOnly)
            {
                LoadCards(true);
                LoadItems(true);
                LoadTime();
                updateBuffer = false;
            }
            LoadMS();
            SetControlValues(gpName.Controls, charactersOnly);
            SetControlValues(gpLevel.Controls, charactersOnly);
            SetControlValues(gpHPMP.Controls, charactersOnly);
            SetControlValues(gpStats.Controls, charactersOnly);
            SetControlValues(gbPartyStats.Controls, charactersOnly);
            SetControlValues(gbParty.Controls, charactersOnly);
            SetControlValues(tabPageMisc.Controls, charactersOnly);
            SetControlValues(tabPageEquipments.Controls, charactersOnly);
            SetControlValues(tabPageAbilities.Controls, charactersOnly);
            SetControlValues(tabPagemagicStones.Controls, charactersOnly);
            SetControlValues(gbBonusPools.Controls, charactersOnly);
            SetControlValues(gbHighScoresHC.Controls, charactersOnly);
            SetControlValues(tabPageHotCold.Controls, charactersOnly);
            SetControlValues(gbJumps.Controls, charactersOnly);
            foreach (Control c in flowLayoutPanel3.Controls)
            {
                if (c is Panel)
                    SetControlValues(c.Controls, charactersOnly);
            }
            //SetControlValues(flowLayoutPanel3.Controls, charactersOnly);
            SetPartyLeaderId();
            GC.Collect(); GC.WaitForPendingFinalizers();
            updateBuffer = true;
        }

        /// <summary>
        /// Load all items based on the update buffer. All items will be displayed in the itemGridview.
        /// </summary>
        /// <param name="set_tags">
        /// Only when form loads and when save block changes is it vital to set this to is true.
        /// Else it should be false to not impact performance.
        /// If true it decides which buffer offsets to effect pr. row on item updates.
        /// </param>
        private void LoadItems(bool set_tags)
        {
            int start = (!IsNormalMemCard ? SaveMap.ITEM_SECTION_START_RR : SaveMap.ITEM_START_OFFSET + CurrentOffset);
            for (int i = start, j = 0; i < start + 512; i += 2, j++)
            {
                if (j < gwItems.Rows.Count)
                {
                    if (editBuffer[i + Item_typeOffset] < items.Length - 2 && editBuffer[i + itemQuantityOffset] != 0)
                        UpdateItemRowInGrid(j, Resources.GoldCoin, items[editBuffer[i + Item_typeOffset]], editBuffer[i + itemQuantityOffset]);
                    else
                        UpdateItemRowInGrid(j, new Bitmap(24, 24), "EMPTY", 0);
                }
            }
            if (set_tags)
            {
                for (int i = 0; i < gwItems.Rows.Count; i++)
                {
                    gwItems[1, i].Tag = string.Format("0;1;3;{0};1", i);
                    gwItems[2, i].Tag = string.Format("1;1;3;{0};0", i);
                }
            }
            ChangeEditItem(selectedItemIndex, true);
        }

        private void UpdateItemRowInGrid(int rowindex, Image image, string name, byte quantity)
        {
            int ItemIndex = Array.IndexOf(items, name);
            gwItems["ItemIcon", rowindex].Value = image;
            gwItems["ItemName", rowindex].Value = ItemName.Items[ItemIndex];
            gwItems["Quantity", rowindex].Value = quantity;
        }

        /// <summary>
        /// Loads all the cards. The buffers must NOT update when this method is called.
        /// </summary>
        /// <param name="fillCardGrid">Shall the cardgrid be filled?</param>
        private void LoadCards(bool fillCardGrid)
        {
            cards = new Card[Card.MaxNumberOfCards(master.info)];
            numericUpDown2.Maximum = Convert.ToDecimal(cards.Length - 1);
            int co = (IsNormalMemCard ? SaveMap.CARD_START_OFFSET : SaveMap.CARD_SECTION_START_RR) + CurrentOffset;
            int nrOC = Card.NumberOfBytesPrCard(master.info);
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = new Card((byte)i,
                    editBuffer[co + Card.InternalOffset(master.info, (CardStat)3)],
                    editBuffer[co + Card.InternalOffset(master.info, (CardStat)2)],
                    editBuffer[co + Card.InternalOffset(master.info, (CardStat)4)],
                    editBuffer[co + Card.InternalOffset(master.info, (CardStat)5)],
                    editBuffer[co + Card.InternalOffset(master.info, 0)],
                    editBuffer[co + Card.InternalOffset(master.info, (CardStat)1)]);
                co += nrOC;
            }

            if (fillCardGrid)
            {
                CardPanels = new Panel[Card.MaxNumberOfCards(master.info)];
                //1st we check if all panels are initialized.
                bool init = false;
                for (int i = 0; i < CardPanels.Length; i++)
                {
                    init = CardPanels[i] == null;
                    if (init) break;
                }
                if (init)
                {
                    cbCardsAllType.SelectedIndex = 0;
                    cbCardsAllAType.SelectedIndex = 0;
                    for (int i = 0; i < CardPanels.Length; i++)
                    {
                        CardPanels[i] = new Panel();
                        CardPanels[i].Tag = (byte)i;
                        CardPanels[i].Name = "cp" + i;
                        CardPanels[i].Width = 47;
                        CardPanels[i].Height = 58;
                        CardPanels[i].BorderStyle = BorderStyle.FixedSingle;
                        CardPanels[i].BackgroundImageLayout = ImageLayout.Center;
                        CardPanels[i].TabStop = true;
                        CardPanels[i].TabIndex = i;
                        Label lbl;
                        lbl = new Label();
                        lbl.Name = "clbl" + i; lbl.Tag = (byte)i;
                        lbl.Location = new Point(0, 26);
                        lbl.Font = new Font(this.Font.Name, MIN_FONT_SIZE);
                        //lbl.ForeColor = Color.FromArgb(212, 210, 159, 54);
                        lbl.ForeColor = Color.FromArgb(250, 209, 104);
                        lbl.BackColor = Color.FromArgb(128, 40, 120, 200);
                        CardPanels[i].Controls.Add(lbl);
                        CardPanels[i].Cursor = Cursors.Hand;
                        CardPanels[i].Paint += new PaintEventHandler(panel_Paint);
                        CardPanels[i].Click += new EventHandler(Card_Click);
                        lbl.Click += new EventHandler(Card_Click);
                        lbl.Size = new Size(47, 15);
                        lbl.TextAlign = ContentAlignment.MiddleCenter;
                    }
                    flowLayoutPanel1.Controls.Clear();
                    flowLayoutPanel1.Controls.AddRange(CardPanels);
                }

                foreach (Card card in cards)
                    UpdateCardFlowLayoutPanel(card);
            }
            ChangeEditCard((int)numericUpDown2.Value);

            //TEST
        }

        /// <summary>
        /// Searches the flowlayout panel for a card id/index and updates the panel associated with the card's id/index if found.
        /// </summary>
        /// <param name="card">The card to use as search param.</param>
        private void UpdateCardFlowLayoutPanel(Card card)
        {

            foreach (Control c in flowLayoutPanel1.Controls)
            {
                if (c.GetType() == typeof(Panel))
                {
                    if ((byte)((Panel)c).Tag == card.CardNumber(master.info))
                    {
                        UpdateCardPanel(card, (Panel)c);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Updates a panel based on a card's properties.
        /// </summary>
        /// <param name="card">The card which properties are used to update the panel.</param>
        /// <param name="panel">The panel to update.</param>
        private void UpdateCardPanel(Card card, Panel panel)
        {
            try
            {
                panel.Controls[0].Text = Strings.GetCardString(card);
            }
            catch (Exception ex) { ex.ToString(); }
            try
            {
                if (card.Type == 0xFF)
                    panel.BackgroundImage = Resources.nocard;
                else
                    panel.BackgroundImage = master.cardPics[card.Type];
                panel.Refresh();
            }
            catch (Exception ex) { ex.ToString(); }
        }

        public void LoadMS()
        {
            /*int start = CurrentOffset +
                SaveMap.CHARACTER_SECTION_START_OFFSET + ((characterID - 1) * SaveMap.CHARACTER_BLOCK_SIZE) + 0x88;*/
            int start = CurrentOffset +
                (IsNormalMemCard ? SaveMap.CHARACTER_SECTION_START_OFFSET : SaveMap.CHARACTER_SECTION_START_RR) +
                ((characterID - 1) *
                (IsNormalMemCard ? SaveMap.CHARACTER_BLOCK_SIZE : SaveMap.CHARACTER_SECTION_LENGTH_RR)) +
                (IsNormalMemCard ? 136 : 233);
            foreach (Control c in tabPagemagicStones.Controls)
            {
                if (c is PictureBox)
                {
                    byte offset = Convert.ToByte(Strings.GetSeprStr((string)c.Tag, 0, ';'));
                    byte bit = Convert.ToByte(Strings.GetSeprStr((string)c.Tag, 1, ';'));
                    if ((editBuffer[offset + start] & bit) > 0)
                        (c as PictureBox).Image = Resources.MS;
                    else
                        (c as PictureBox).Image = Resources.SocketG;
                }
            }
        }

        private void LoadTime()
        {

            uint time = Convert.ToUInt32(GetGameTime());
            uint seconds = time / Framerate;
            uint minutes = seconds / 60;
            uint hours = minutes / 60;
            seconds = seconds % 60;
            minutes = minutes % 60;

            updateBuffer = false;
            nupTH.Value = Numbers.MaxMin((int)hours, (int)nupTH.Maximum, (int)nupTH.Minimum);
            nupTM.Value = Numbers.MaxMin((int)minutes, (int)nupTH.Maximum, (int)nupTH.Minimum);
            nupTS.Value = Numbers.MaxMin((int)seconds, (int)nupTH.Maximum, (int)nupTH.Minimum);
            updateBuffer = true;
        }

        /// <summary>
        /// Retrieves Game time of current block.
        /// </summary>
        /// <returns>Game time of current block.</returns>
        private object GetGameTime()
        {
            int k = (IsNormalMemCard ? SaveMap.GAME_TIME_OFFSET : SaveMap.PLAYTIME_AS_DOUBLE_OFFSET_RR) + CurrentOffset;
            byte[] var;
            if (IsNormalMemCard)
            {
                var = new byte[] { editBuffer[k], editBuffer[k + 1], editBuffer[k + 2], editBuffer[k + 3] };
                return OffsetManager.Swap_Bytes(var, 3, 4);
            }
            else
            {
                //var = new byte[] { editBuffer[k], editBuffer[k + 1], editBuffer[k + 2], editBuffer[k + 3], editBuffer[k + 4], editBuffer[k + 5], editBuffer[k + 6], editBuffer[k + 7] };
                return BitConverter.ToDouble(editBuffer, k);
            }
        }

        /// <summary>
        /// Change a value relative to the current block.
        /// </summary>
        /// <param name="sender">the control.</param>
        /// <param name="e">event.</param>
        private void ControlValueChange(object sender, EventArgs e)
        {
            if (!updateBuffer) return;
            if (!ValidateContolType(sender)) return;
            try
            {
                string[] tags;
                if (sender.GetType() == typeof(DataGridViewComboBoxCell) || sender.GetType() == typeof(DataGridViewTextBoxCell))
                    tags = ((string)((DataGridViewCell)sender).Tag).Split(new char[] { ';' });
                else
                    tags = ((string)((Control)sender).Tag).Split(new char[] { ';' });

                //type: 0 = absolute, 1 = character, 2 = cards, 3 = item, 4 = misc
                //This tells us where to begin to insert values
                byte type = Convert.ToByte(tags[2]);
                byte bytes = Convert.ToByte(tags[1]);
                int offset = 0;
                if (IsNormalMemCard)
                    offset = CurrentOffset;
                else
                {
                    if (tags.Length < 5)
                        return;// throw new Exception("Save::ControlValueChange: Missing data to calculate value.");
                    //offset = Convert.ToInt32(tags[4]);
                    if (type != 2) //CARDS are messy because they have 2 extra bytes. the tag[4] are set to 0, but offset are handled diffrently.
                        tags[0] = tags[4];
                    //We must adjust relative to character offset block as RR const are absolute
                    //if (type == 1)
                    //    offset -= SaveMap.CHARACTER_SECTION_START_OFFSET;
                }
                //tags = null;
                offset = GetRealOffset(type, offset, Convert.ToInt32(tags[0]));
                SetControlOffsetValue(sender, offset, bytes);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private bool ValidateContolType(object sender)
        {
            foreach (Type t in validControlUpdateBufferTypes)
            {
                if (t == sender.GetType())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Set a value to the editBuffer.
        /// </summary>
        /// <param name="sender">The contol that value is changed.</param>
        /// <param name="offset">The offset (relative to block, possibly also a section).</param>
        /// <param name="bytes">Number of bytes effected.</param>
        private void SetControlOffsetValue(object sender, int offset, byte bytes)
        {
            if (sender.GetType() == typeof(TextBox))
            {
                char c = ' ';
                byte paddingByte = (byte)(IsNormalMemCard ? 0xFF : 0);
                for (byte i = 0; i < bytes; i++)
                {
                    if (((TextBox)sender).Text.Length > i)
                    {
                        c = ((TextBox)sender).Text[i];
                        if (IsNormalMemCard && master.characterTable.Contains(c))
                        {
                            editBuffer[offset + i] = Convert.ToByte(master.characterTable[c]);
                            if (characterID == partyLeaderID)
                                editBuffer[CurrentOffset + SaveMap.PREVIEW_PARTY_LEADER_NAME_OFFSET + i] = Convert.ToByte(master.characterTable[c]);
                        }
                        else if (!IsNormalMemCard)
                        {
                            editBuffer[offset + i] = Convert.ToByte(c);
                        }
                        continue;
                    }
                    editBuffer[offset + i] = paddingByte;
                    if (IsNormalMemCard && characterID == partyLeaderID)
                        editBuffer[CurrentOffset + SaveMap.PREVIEW_PARTY_LEADER_NAME_OFFSET + i] = paddingByte;
                    break;
                }
                if (IsNormalMemCard && characterID == partyLeaderID)
                    editBuffer[CurrentOffset + SaveMap.PREVIEW_PARTY_LEADER_LEVEL_OFFSET] =
                        editBuffer[CurrentOffset + SaveMap.CHARACTER_SECTION_START_OFFSET +
                                   ((partyLeaderID - 1) * SaveMap.CHARACTER_BLOCK_SIZE) + SaveMap.CHARACTER_LEVEL_OFFSET];
                EnableApply();
                GC.Collect(); GC.WaitForPendingFinalizers();
                return;
            }
            uint value = 0;
            if (sender.GetType() == typeof(NumericUpDown))
                value = (uint)((NumericUpDown)sender).Value;
            else if (sender.GetType() == typeof(ComboBox))
            {
                if ((sender as ComboBox).SelectedItem.GetType() == typeof(CardAttackType))
                    value = (uint)((CardAttackType)(sender as ComboBox).SelectedItem);
                else
                    value = (uint)((ComboBox)sender).SelectedIndex;
            }
            else if (sender.GetType() == typeof(ComboBoxEx))
            {
                value = (uint)((ComboBoxEx)sender).SelectedIndex;
                if (!IsNormalMemCard && value > 8) value = 255;
                if (IsNormalMemCard &&
                    cbPartyMember1.SelectedIndex != partyLeaderID - 1 &&
                    cbPartyMember2.SelectedIndex != partyLeaderID - 1 &&
                    cbPartyMember3.SelectedIndex != partyLeaderID - 1 &&
                    cbPartyMember4.SelectedIndex != partyLeaderID - 1)
                {
                    int[] values = { cbPartyMember1.SelectedIndex, cbPartyMember2.SelectedIndex,
                                     cbPartyMember3.SelectedIndex, cbPartyMember4.SelectedIndex };
                    foreach (int i in values)
                    {
                        if (i > 8) continue;
                        partyLeaderID = (byte)(i + 1); byte temp = characterID; characterID = partyLeaderID;
                        string name = master.LoadText(8, CurrentOffset + SaveMap.CHARACTER_SECTION_START_OFFSET +
                                                     ((partyLeaderID - 1) * SaveMap.CHARACTER_BLOCK_SIZE) + SaveMap.CHARACTER_NAME_OFFSET, editBuffer);
                        TextBox t = new TextBox();
                        t.Text = name;
                        SetControlOffsetValue(t, GetRealOffset(1, CurrentOffset, 0), 8);

                        characterID = temp;
                        break;
                    }
                }
            }
            else if (sender.GetType() == typeof(DataGridViewTextBoxCell))
                value = Convert.ToUInt32(((DataGridViewTextBoxCell)sender).Value);
            else if (sender.GetType() == typeof(DataGridViewComboBoxCell))
            {
                value = (uint)Array.IndexOf(items, ((DataGridViewComboBoxCell)sender).Value);
                //Cannot exceed max byte value.
                //Doing so indicates Empty item bloc (value 0).
                if (value > byte.MaxValue) value = 0;
            }
            else
                return;
            if (bytes == 1)
                editBuffer[offset] = (byte)value;
            else
            {
                byte j = 0;
                foreach (byte b in OffsetManager.Flap_Values(value, bytes))
                {
                    editBuffer[offset + j] = b;
                    j++;
                }
            }

            //Only if we edit cards.
            if (tabControl.SelectedTab == tabPageCards)
                UpdateCard();
            EnableApply();
            GC.Collect(); GC.WaitForPendingFinalizers();
        }

        private void AliasChange(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitmapSmall, bitmapBig;
                Button panel;
                switch (characterID)
                {
                    case 6:
                        panel = btnQuina;
                        if (rbRealChar.Checked)
                        {
                            bitmapBig = Resources.CinnaBig;
                            bitmapSmall = Resources.Cinna;
                        }
                        else
                        {
                            bitmapBig = Resources.QuinaBig;
                            bitmapSmall = Resources.Quina;
                        }
                        break;
                    case 7:
                        panel = btnEiko;
                        if (rbRealChar.Checked)
                        {
                            bitmapBig = Resources.MarcusBig;
                            bitmapSmall = Resources.Marcus;
                        }
                        else
                        {
                            bitmapBig = Resources.EikoBig;
                            bitmapSmall = Resources.Eiko;
                        }
                        break;
                    case 8:
                        panel = btnAmarant;
                        if (rbRealChar.Checked)
                        {
                            bitmapBig = Resources.BlankBig;
                            bitmapSmall = Resources.Blank;
                        }
                        else
                        {
                            bitmapBig = Resources.AmarantBig;
                            bitmapSmall = Resources.Amarant;
                        }
                        break;
                    default:
                        return;
                }
                ChangePanelPic(bitmapSmall, bitmapBig, ref panel);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ChangePanelPic(Bitmap bitmapSmall, Bitmap bitmapBig, ref Button panel)
        {
            panel1.BackgroundImage = bitmapBig;
            panel.BackgroundImage = bitmapSmall;
        }

        private void SetControlValues(Control.ControlCollection groupControl, bool charactersOnly)
        {
            foreach (Control control in groupControl)
            {
                try
                {
                    if (!ValidateContolType(control))
                        continue;
                    if (Strings.GetSeprStr((string)control.Tag, 2, ';') != "1" && charactersOnly)
                        continue;

                }
                catch (Exception ex) { ex.ToString(); continue; }
                string[] tags = ((string)((Control)control).Tag).Split(new char[] { ';' });

                if ((!IsNormalMemCard && tags.Length < 5) || tags.Length < 3)
                {
                    control.Enabled = false;
                    continue;
                }
                else
                    control.Enabled = true;
                //type: 0 = absolute, 1 = character, 2 = cards, 3 = item
                //This tells us where to begin to insert values

                //We are not doing misc. operations on load.
                if (tags[2] == "4")
                    tags[2] = "0";
                byte type = Convert.ToByte(tags[2]);
                byte bytes = Convert.ToByte(tags[1]);
                int offset = CurrentOffset;
                if (!IsNormalMemCard && type != 2)
                    tags[0] = tags[4];
                offset = GetRealOffset(type, offset, Convert.ToInt32(tags[0]));

                if (control.GetType() == typeof(TextBox))
                {
                    ((TextBox)control).Text = master.LoadText(bytes, offset, editBuffer);
                    continue;
                }

                uint value;
                if (bytes > 1)
                    value = OffsetManager.Swap_Bytes(editBuffer, offset + bytes - 1, bytes);
                else
                    value = editBuffer[offset];
                if (control.GetType() == typeof(NumericUpDown))
                {
                    if (value < ((NumericUpDown)control).Minimum ||
                       value > ((NumericUpDown)control).Maximum)
                        value = (uint)Numbers.MaxMin(
                            (int)value, (int)((NumericUpDown)control).Maximum,
                                (int)((NumericUpDown)control).Minimum); //continue;
                    ((NumericUpDown)control).Value = value;
                }
                else if (control.GetType() == typeof(ComboBox))
                    ((ComboBox)control).SelectedIndex = (int)value;
                else if (control.GetType() == typeof(ComboBoxEx))
                    ((ComboBoxEx)control).SelectedIndex = Numbers.MaxMin((int)value, ((ComboBoxEx)control).Items.Count - 1, 0);
            }
            GC.Collect(); GC.WaitForPendingFinalizers();
        }

        private int GetRealOffset(byte type, int blockStartoffset, int sectionOffset)
        {
            switch (type)
            {
                //absolute
                case 0:
                    blockStartoffset += sectionOffset;
                    break;
                //character
                case 1:
                    blockStartoffset += SaveMap.RealCharacterStatOffset(sectionOffset, characterID, master.info);
                    //tags[3] = related Control to calculate total??
                    //object s = Controls[tags[3]];
                    break;
                //card
                case 2:
                    blockStartoffset += SaveMap.RealCardOffset(sectionOffset, (byte)(numericUpDown2.Value), master.info);
                    break;
                //item
                case 3:
                    blockStartoffset += SaveMap.RealItemOffset(sectionOffset, selectedItemIndex, master.info);
                    break;
                //multiple operations
                //case 4:
                //    offset += Convert.ToInt32(tags[0]);
                //    if (tags.Length < 4)
                //        break;                    
                //    SetControlOffsetValue(sender, offset + Convert.ToInt32(tags[3]), Convert.ToByte(tags[1]));
                //    break;
                default:
                    break;
            }
            return blockStartoffset;
        }

        /// <summary>
        /// Corrects the preview data based on the "real" data.
        /// </summary>
        private void CorrectPartyLeadPreview()
        {

        }

        private void SetPartyLeaderId()
        {
            if (!IsNormalMemCard) return;
            string prevName = master.LoadText(8, CurrentOffset + SaveMap.PREVIEW_PARTY_LEADER_NAME_OFFSET, editBuffer);
            int cid = 0;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0: cid = cbPartyMember1.SelectedIndex; break;
                    case 1: cid = cbPartyMember2.SelectedIndex; break;
                    case 2: cid = cbPartyMember3.SelectedIndex; break;
                    case 3: cid = cbPartyMember4.SelectedIndex; break;
                    default: cid = 0; break;
                }
                if (cid > 8) continue;
                string name = master.LoadText(8, CurrentOffset + SaveMap.CHARACTER_SECTION_START_OFFSET +
                                                     (cid * SaveMap.CHARACTER_BLOCK_SIZE) + SaveMap.CHARACTER_NAME_OFFSET, editBuffer);
                if (prevName == name)
                {
                    partyLeaderName = name; partyLeaderID = (byte)(cid + 1); break;
                }
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (updateBuffer)
                ChangeEditCard((int)numericUpDown2.Value);
        }

        private void Card_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(Panel) || sender.GetType() == typeof(Label))
                    ChangeEditCard(Convert.ToInt32(((Control)sender).Tag));
            }
            catch (Exception ex) { ex.ToString(); }
        }

        /// <summary>
        /// Changes card to edit.
        /// </summary>
        /// <param name="i">Card index.</param>
        private void ChangeEditCard(int i)
        {
            updateBuffer = false;
            try
            {
                numericUpDown2.Value = cards[i].CardNumber(master.info);
                panel2.Tag = cards[i].CardNumber(master.info);
                UpdateCardPanel(cards[i], panel2);
                nupCardAttack.Value = cards[i].Attack;
                nupCardPDef.Value = cards[i].PhysicalDefence;
                nupCardMDef.Value = cards[i].MagicDefence;
                int aIndex = (int)cards[i].AttackType;
                aIndex = aIndex >= SaveMap.CARD_MAX_ATTACK_TYPES ? SaveMap.CARD_MAX_ATTACK_TYPES - 1 : aIndex;
                cbAttackType.SelectedIndex = aIndex;
                if (cards[i].Type >= SaveMap.CARD_MAX_CARD_TYPES)
                    cbCardType.SelectedIndex = cbCardType.Items.Count - 1;
                else
                    cbCardType.SelectedIndex = cards[i].Type;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            finally { updateBuffer = true; }
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(Panel))
                {
                    byte nr = (byte)((Control)sender).Tag;

                    if (nr >= cards.Length) return;

                    if (cards[nr].Arrows == (byte)CardArrow.NONE)
                        return;

                    int midleX = ((Panel)sender).Width / 2 - 8,
                        midleY = ((Panel)sender).Height / 2 - 8,
                        FarRightX = ((Panel)sender).Width - 13,
                        FarDownY = ((Panel)sender).Height - 13;

                    if ((cards[nr].Arrows & (byte)CardArrow.UP) == (byte)CardArrow.UP)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.UP.Image, midleX - 1, 0);

                    if ((cards[nr].Arrows & (byte)CardArrow.DOWN) == (byte)CardArrow.DOWN)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.DOWN.Image, midleX - 1, FarDownY + 2);

                    if ((cards[nr].Arrows & (byte)CardArrow.DOWN_LEFT) == (byte)CardArrow.DOWN_LEFT)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.DOWN_LEFT.Image, 0, FarDownY);

                    if ((cards[nr].Arrows & (byte)CardArrow.DOWN_RIGHT) == (byte)CardArrow.DOWN_RIGHT)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.DOWN_RIGHT.Image, FarRightX, FarDownY);

                    if ((cards[nr].Arrows & (byte)CardArrow.LEFT) == (byte)CardArrow.LEFT)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.LEFT.Image, 0, midleY);

                    if ((cards[nr].Arrows & (byte)CardArrow.RIGHT) == (byte)CardArrow.RIGHT)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.RIGHT.Image, FarRightX + 2, midleY);

                    if ((cards[nr].Arrows & (byte)CardArrow.UP_LEFT) == (byte)CardArrow.UP_LEFT)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.UP_LEFT.Image, 0, 0);

                    if ((cards[nr].Arrows & (byte)CardArrow.UP_RIGHT) == (byte)CardArrow.UP_RIGHT)
                        e.Graphics.DrawImageUnscaled(CardArrowImage.UP_RIGHT.Image, FarRightX, 0);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void UpdateCard()
        {
            int i = (int)numericUpDown2.Value;
            if (i >= cards.Length) return;
            cards[i].Type = (byte)cbCardType.SelectedIndex;
            cards[i].AttackType = (CardAttackType)cbAttackType.SelectedItem;
            cards[i].Attack = (byte)nupCardAttack.Value;
            cards[i].PhysicalDefence = (byte)nupCardPDef.Value;
            cards[i].MagicDefence = (byte)nupCardMDef.Value;
            UpdateCardFlowLayoutPanel(cards[i]);
            UpdateCardPanel(cards[i], panel2);
            EnableApply();
        }

        private void Arrow_Checked_Changed(object sender, EventArgs e)
        {
            try
            {
                if (((Control)sender).Tag.GetType() == typeof(CardArrow))
                {
                    int i = (int)numericUpDown2.Value;
                    //int offset = Card.InternalOffset(selectsave.info, CardStat.Arrows) + 
                    //    (((IsNormalMemCard ? SaveMap.CARD_START_OFFSET : SaveMap.CARD_SECTION_START_RR) + 
                    //    (i * Card.NumberOfBytesPrCard(selectsave.info))) + CurretOffset);
                    int offset = Card.AbsoluteCardOffset(master.info, CardStat.Arrows, i, CurrentOffset);
                    if ((CardArrow)((Control)sender).Tag == CardArrow.NONE)
                    {
                        if (cards[i].Arrows < 255)
                            editBuffer[offset] = cards[i].Arrows = 255;
                        else
                            editBuffer[offset] = cards[i].Arrows = 0;
                    }
                    else
                    {
                        byte oldValue = editBuffer[offset];
                        byte newValue = (byte)((byte)((Control)sender).Tag ^ oldValue);
                        editBuffer[offset] = cards[i].Arrows = newValue;
                    }
                    UpdateCard();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace); }
        }

        private void AllCardsOperation(byte type)
        {
            updateBuffer = false;
            //int numberOfBytesPrCard = Card.NumberOfBytesPrCard(master.info);
            //int start = ((SaveMap.CARD_START_OFFSET + ((int)numericUpDown2.Value * 6)) + (currentBlock * MemCard.SAVE_BLOCK_SIZE));
            //int start = (((IsNormalMemCard ? SaveMap.CARD_START_OFFSET : SaveMap.CARD_SECTION_START_RR)
            //    /*+ ((int)numericUpDown2.Value * numberOfBytesPrCard)*/) + CurrentOffset);
            //for (int i = start, j = 0; /*i < start + (cards.Length * numberOfBytesPrCard) - 1 &&*/ j < cards.Length; i += numberOfBytesPrCard, j++)
            //    SingleCardOperation(type, j);


            DialogResult res = DialogResult.No;
            if (type == 38 || type == 0)
            {
                int cOffset = (IsNormalMemCard ? SaveMap.CARD_START_OFFSET : SaveMap.CARD_SECTION_START_RR) + CurrentOffset;
                int numberOfBytesPrCard = Card.NumberOfBytesPrCard(master.info);
                int internalCoffset = Card.InternalOffset(master.info, CardStat.Arrows);

                //Unique arrows or not
                if (type == 38)
                {
                    res = MessageBox.Show(this, "Would you like to make all Arrows unique? Click \"No\" to make all random.", "Random or unique arrows", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        byte i = 156;
                        for (int x = 0; x < 100; x++)
                        {
                            editBuffer[cOffset + internalCoffset] = i;
                            i += 1;
                            cOffset += numberOfBytesPrCard;
                        }
                    }
                }
                //Master all cards. Ask if set to max values
                else if (type == 0)
                {
                    res = MessageBox.Show(this, "Would you like to make all cards set to max stats (Atk, Def, mDef) by card type (Goblin, Fang etc.)? Click \"No\" to make all stats 255.", "Set Max stats", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        int mIndex;
                        for (int i = 0; i < cards.Length; i++)
                        {
                            mIndex = (int)cards[i].Type;
                            if (mIndex > cards.Length - 1) continue;
                            cards[i].AttackType = CardAttackType.A;
                            cards[i].Attack = MAX_CARD_STATS[mIndex, 0];
                            cards[i].PhysicalDefence = MAX_CARD_STATS[mIndex, 1];
                            cards[i].MagicDefence = MAX_CARD_STATS[mIndex, 2];
                            cards[i].WriteAttributesToBuffer(ref editBuffer, CurrentOffset, master.info);
                        }
                    }
                }
                if (res == DialogResult.Cancel) return;
            }

            //int start = (IsNormalMemCard ? SaveMap.CARD_START_OFFSET : SaveMap.CARD_SECTION_START_RR) + CurrentOffset;
            if (res == DialogResult.No)
                for (int i = 0; i < cards.Length; i++)
                    SingleCardOperation(type, i);

            LoadCards(true);
            updateBuffer = true;
        }

        public void SingleCardOperation(byte type, int cardIndex)
        {
            //Master
            if (type == 0)
                cards[cardIndex].SetAttributes(CardAttackType.A, 255, 255, 255, cards[cardIndex].Type, 255);

            //Delete
            else if (type == 1)
                cards[cardIndex].SetAttributes(CardAttackType.P, 0, 0, 0, 255, 0);

            else if (type < 14)
            {

                //Set Type
                if (type == 2 && cardIndex < 100)
                    cards[cardIndex].Type = (byte)cardIndex;

                //Add Attack
                else if (type == 3)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack + (byte)nupCardsAllAttack.Value, 255, 0);

                //Sub Attack
                else if (type == 4)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack - (byte)nupCardsAllAttack.Value, 255, 0);

                //Set Attack
                else if (type == 7)
                    cards[cardIndex].Attack = (byte)nupCardsAllAttack.Value;

                //Multiply Attack
                else if (type == 5)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin((int)(cards[cardIndex].Attack * nupCardsAllAttack.Value), 255, 0);

                //Divide Attack
                else if (type == 6)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin((int)(cards[cardIndex].Attack / nupCardsAllAttack.Value), 255, 0);

                //Mod Attack
                else if (type == 8)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack % (byte)nupCardsAllAttack.Value, 255, 0);

                //AND Attack
                else if (type == 9)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack & (byte)nupCardsAllAttack.Value, 255, 0);

                //OR Attack
                else if (type == 10)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack | (byte)nupCardsAllAttack.Value, 255, 0);

                //XOR Attack
                else if (type == 11)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack ^ (byte)nupCardsAllAttack.Value, 255, 0);

                //<< Attack
                else if (type == 12)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack << (byte)nupCardsAllAttack.Value, 255, 0);

                //>> Attack
                else if (type == 13)
                    cards[cardIndex].Attack = (byte)Numbers.MaxMin(cards[cardIndex].Attack >> (byte)nupCardsAllAttack.Value, 255, 0);
            }

            else if (type < 25)
            {
                //Add PDef
                if (type == 14)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence + (byte)nupCardsAllPDef.Value, 255, 0);

                //Sub PDef
                else if (type == 15)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence - (byte)nupCardsAllPDef.Value, 255, 0);

                //Set PDef
                else if (type == 18)
                    cards[cardIndex].PhysicalDefence = (byte)nupCardsAllPDef.Value;

                //Multiply Attack
                else if (type == 16)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin((int)(cards[cardIndex].PhysicalDefence * nupCardsAllPDef.Value), 255, 0);

                //Divide Attack
                else if (type == 17)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin((int)(cards[cardIndex].PhysicalDefence / nupCardsAllPDef.Value), 255, 0);

                //Mod Attack
                else if (type == 19)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence % (byte)nupCardsAllPDef.Value, 255, 0);

                //AND Attack
                else if (type == 20)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence & (byte)nupCardsAllPDef.Value, 255, 0);

                //OR Attack
                else if (type == 21)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence | (byte)nupCardsAllPDef.Value, 255, 0);

                //XOR Attack
                else if (type == 22)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence ^ (byte)nupCardsAllPDef.Value, 255, 0);

                //<< Attack
                else if (type == 23)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence << (byte)nupCardsAllPDef.Value, 255, 0);

                //>> Attack
                else if (type == 24)
                    cards[cardIndex].PhysicalDefence = (byte)Numbers.MaxMin(cards[cardIndex].PhysicalDefence >> (byte)nupCardsAllPDef.Value, 255, 0);
            }

            else if (type < 36)
            {

                //Add MDef
                if (type == 25)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence + (byte)nupCardsAllMDef.Value, 255, 0);

                //Sub MDef
                else if (type == 26)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence - (byte)nupCardsAllMDef.Value, 255, 0);

                //Set MDef
                else if (type == 29)
                    cards[cardIndex].MagicDefence = (byte)nupCardsAllMDef.Value;

                //Multiply Attack
                else if (type == 27)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin((int)(cards[cardIndex].MagicDefence * nupCardsAllMDef.Value), 255, 0);

                //Divide Attack
                else if (type == 28)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin((int)(cards[cardIndex].MagicDefence / nupCardsAllMDef.Value), 255, 0);

                //Mod Attack
                else if (type == 30)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence % (byte)nupCardsAllMDef.Value, 255, 0);

                //AND Attack
                else if (type == 31)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence & (byte)nupCardsAllMDef.Value, 255, 0);

                //OR Attack
                else if (type == 32)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence | (byte)nupCardsAllMDef.Value, 255, 0);

                //XOR Attack
                else if (type == 33)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence ^ (byte)nupCardsAllMDef.Value, 255, 0);

                //<< Attack
                else if (type == 34)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence << (byte)nupCardsAllMDef.Value, 255, 0);

                //>> Attack
                else if (type == 35)
                    cards[cardIndex].MagicDefence = (byte)Numbers.MaxMin(cards[cardIndex].MagicDefence >> (byte)nupCardsAllMDef.Value, 255, 0);
            }

            //Set Type by type control
            else if (type == 36)
                cards[cardIndex].Type = (byte)cbCardsAllType.SelectedIndex;

            //Set AType by type control
            else if (type == 37)
                cards[cardIndex].AttackType = (CardAttackType)cbCardsAllAType.SelectedItem;

            //Randomize arrows
            else if (type == 38)
                cards[cardIndex].Arrows ^= RandomNumber();

            else return;

            cards[cardIndex].WriteAttributesToBuffer(ref editBuffer, CurrentOffset, master.info);

        }

        private byte RandomNumber()
        {
            Random random = new Random();
            byte b = (byte)(random.Next(102, 25600) / 100 - 1);
            System.Threading.Thread.Sleep(Numbers.MaxMin((1 + b / 10), 20, 10));
            return b;
        }

        private void btnCardAddAllOp_Click(object sender, EventArgs e)
        {
            try
            {
                byte type = Convert.ToByte(((Control)sender).Tag);
                if (type > 38)
                    return;
                if (type > 2 && type < 36)
                {
                    int i; decimal value;
                    if (type == 3)
                    {
                        i = cbCardsAllAttckOp.SelectedIndex;
                        value = nupCardsAllAttack.Value;
                        type = (byte)(i + 3);
                    }
                    else if (type == 4)
                    {
                        i = cbCardsAllPDefOp.SelectedIndex;
                        value = nupCardsAllPDef.Value;
                        type = (byte)(i + 14);
                    }
                    else if (type == 5)
                    {
                        i = cbCardsAllMDefOp.SelectedIndex;
                        value = nupCardsAllMDef.Value;
                        type = (byte)(i + 25);
                    }
                    else
                        return;
                    if (!validateNUP(value, i)) return;
                }
                AllCardsOperation(type);
                EnableApply();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace); }

        }

        private void btnSingleOp_Click(object sender, EventArgs e)
        {
            try
            {
                byte type = Convert.ToByte(((Control)sender).Tag);
                if (type > 13)
                    return;
                int i = (int)numericUpDown2.Value;
                SingleCardOperation(type, i);
                UpdateCardFlowLayoutPanel(cards[i]);
                ChangeEditCard(i);
                EnableApply();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace); }
        }

        /*
        private void DragDrop_Cards(object sender, DragEventArgs e)
        {

        }

        private void DragDrop_Items(object sender, DragEventArgs e)
        {

        }

        private void DragDrop_Character(object sender, DragEventArgs e)
        {

        }

        private void DragDrop_AllCharacters(object sender, DragEventArgs e)
        {

        }
        */

        private void ChangeCell(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                previousQuantity =
                    Convert.ToByte(gwItems["Quantity", e.RowIndex].Value);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace); }
            finally
            {
                if (updateBuffer)
                    ChangeEditItem((byte)e.RowIndex, false);
            }
        }

        private void ChangeEditItem(byte i, bool changeGridViewRow)
        {
            updateBuffer = false;
            try
            {
                selectedItemIndex = i;
                nupItemSelected.Value = i;
                nupItemQuantity.Value = Convert.ToByte(gwItems["Quantity", i].Value);
                cbItemName.SelectedIndex = Array.IndexOf(items, gwItems["ItemName", i].Value);
                pbItemImage.Image = (Image)gwItems["ItemIcon", i].Value;
                cbItemName.Tag = string.Format("0;1;3;{0};1", i);
                nupItemQuantity.Tag = string.Format("1;1;3;{0};0", i);
                if (changeGridViewRow && gwItems.Rows.Count > i)
                {
                    gwItems.Rows[i].Selected = true;
                    gwItems.CurrentCell = gwItems[1, i];
                }
            }
            catch (Exception ex) { ex.ToString(); }
            finally { updateBuffer = true; }
        }

        private void nupItemSelected_ValueChanged(object sender, EventArgs e)
        {
            if (updateBuffer)
                ChangeEditItem((byte)nupItemSelected.Value, true);
        }

        private void CellEdited(object sender, DataGridViewCellEventArgs e)
        {
            updateBuffer = false;
            byte q = 0;

            try
            {
                q = Convert.ToByte(gwItems[2, e.RowIndex].Value);
            }
            catch (Exception ex)
            {
                gwItems[2, e.RowIndex].Value = previousQuantity;
                ex.ToString(); updateBuffer = true; return;
            }

            updateBuffer = true;
            if (cbItemName.SelectedItem == null)
                return;
            UpdateItem((string)gwItems["ItemName", e.RowIndex].Value, q);
            ControlValueChange(gwItems["ItemName", e.RowIndex], null);
            ControlValueChange(gwItems["Quantity", e.RowIndex], null);
        }

        private void UpdateItem(string name, byte quantity)
        {
            //Before we update the buffer we change the display correctly.
            updateBuffer = false; Image img = Resources.GoldCoin;
            /*if (quantity > 0 && name == "EMPTY")
                  name = "Potion";
            else */
            if (name == "EMPTY" || name == "Nothing")
            { quantity = 0; name = "EMPTY"; }
            else if (name != "EMPTY" && quantity == 0)
                quantity = 1;

            //Ensure to save values correctly if quantity still is 0 or name = Empty.
            if (quantity == 0 || name == "EMPTY" || name == "Nothing")
            { quantity = 0; name = "EMPTY"; img = new Bitmap(24, 24); }

            if (quantity > 99) quantity = 99;

            //Update selectedrow.
            UpdateItemRowInGrid(selectedItemIndex, img, name, quantity);

            //Update item editing controls.
            pbItemImage.Image = img;
            cbItemName.SelectedIndex = Array.IndexOf(items, name);
            nupItemQuantity.Value = quantity;

            updateBuffer = true;
        }

        private void ItemControlChange(object sender, EventArgs e)
        {
            if (updateBuffer)
            {
                if (cbItemName.SelectedItem == null)
                    return;
                UpdateItem((string)cbItemName.SelectedItem, (byte)nupItemQuantity.Value);
                ControlValueChange(cbItemName, e);
                ControlValueChange(nupItemQuantity, e);
            }
        }

        private void btnItemSingleEmpty_Click(object sender, EventArgs e)
        {
            cbItemName.SelectedIndex = Array.IndexOf(items, "EMPTY");
        }

        private void btnItemSingle99_Click(object sender, EventArgs e)
        {
            if (cbItemName.SelectedIndex == Array.IndexOf(items, "EMPTY"))
            {
                updateBuffer = false;
                cbItemName.SelectedIndex = Array.IndexOf(items, "Potion");
                updateBuffer = true;
            }
            nupItemQuantity.Value = 99;
        }

        private void btnItemsAllEmpty_Click(object sender, EventArgs e)
        {
            AllItemsOperation(0);
        }

        private void btnItemsAllFill_Click(object sender, EventArgs e)
        {
            AllItemsOperation(1);
        }

        private bool AllItemsOperation(byte type)
        {
            try
            {
                if (type > 12) return false;
                int start = CurrentOffset + (!IsNormalMemCard ? SaveMap.ITEM_SECTION_START_RR : SaveMap.ITEM_START_OFFSET);

                for (int i = start, j = 0; i < start + 512; i += 2, j++)
                {
                    //Empty
                    if (type == 0)
                    {
                        editBuffer[i + Item_typeOffset] = 0; editBuffer[i + itemQuantityOffset] = 0;
                    }
                    //Fill
                    else if (type == 1)
                    {
                        editBuffer[i + Item_typeOffset] = (byte)j; editBuffer[i + itemQuantityOffset] = 99;
                    }
                    //Add
                    else if (type == 2 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] + nupItemsAllOp.Value), 99, 1);
                    //Set
                    else if (type == 6 && editBuffer[i + itemQuantityOffset] > 0)
                    {
                        //if(nupItemsAllOp.Value == 0)
                        //    editBuffer[i] = 0;
                        editBuffer[i + itemQuantityOffset] = (byte)nupItemsAllOp.Value;
                    }
                    ////Set (if old is equal 0)
                    //else if (type == 6 && editBuffer[i + 1] == 0)
                    //{
                    //    if (nupItemsAllOp.Value == 0)
                    //    {
                    //        editBuffer[i] = 0;
                    //        continue;
                    //    }
                    //    editBuffer[i] = (byte)j;
                    //    editBuffer[i + 1] = (byte)nupItemsAllOp.Value;
                    //}
                    //Sub
                    else if (type == 3 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] - nupItemsAllOp.Value), 99, 1);
                    //Mult
                    else if (type == 4 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] * nupItemsAllOp.Value), 99, 1);
                    //Div
                    else if (type == 5 && editBuffer[i + itemQuantityOffset] > 0 && nupItemsAllOp.Value != 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] / nupItemsAllOp.Value), 99, 1);
                    //Modul
                    else if (type == 7 && editBuffer[i + itemQuantityOffset] > 0 && nupItemsAllOp.Value != 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] % nupItemsAllOp.Value), 99, 1);
                    //AND
                    else if (type == 8 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] & Convert.ToByte(nupItemsAllOp.Value)), 99, 1);
                    //OR
                    else if (type == 9 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] | Convert.ToByte(nupItemsAllOp.Value)), 99, 1);
                    //XOR
                    else if (type == 10 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] ^ Convert.ToByte(nupItemsAllOp.Value)), 99, 1);
                    //<<
                    else if (type == 11 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] << Convert.ToByte(nupItemsAllOp.Value)), 99, 1);
                    //>>
                    else if (type == 12 && editBuffer[i + itemQuantityOffset] > 0)
                        editBuffer[i + itemQuantityOffset] = (byte)Numbers.MaxMin((int)(editBuffer[i + itemQuantityOffset] >> Convert.ToByte(nupItemsAllOp.Value)), 99, 1);
                }
                LoadItems(false);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return false; }
            EnableApply();
            return true;
        }

        private void btnItemsAllEquals_Click(object sender, EventArgs e)
        {
            if (!validateNUP(nupItemsAllOp.Value, cbItemOp.SelectedIndex))
                return;
            AllItemsOperation((byte)(cbItemOp.SelectedIndex + 2));
        }

        private bool validateNUP(Decimal nupValue, int i)
        {
            if (nupValue == 0)
            {
                if (i == 0 || i == 1 || i == 3 || i == 4 ||
                   i == 5 || i == 9 || i == 10)
                    return false;
            }
            return true;
        }

        private void ExportImportClick(object sender, EventArgs e)
        {
            try
            {
                if (!IsNormalMemCard)
                {
                    MessageBox.Show("Sorry, not yet for 2016 version. \"/", "nope", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                byte[] tempBuffer = new byte[editBuffer.Length];
                master.CopyBuffers(editBuffer, tempBuffer);
                PortableType type = (PortableType)Convert.ToByte(((Button)sender).Tag);
                FormLoader.OpenExportImport(ref editBuffer, type, currentBlock, characterID, master.regKey, null);
                int i = 0;
                foreach (byte b in editBuffer)
                {
                    if (b != tempBuffer[i])
                    {
                        Load_Variables(type == PortableType.ImportAllCharacters |
                                       type == PortableType.ImportCharacter |
                                       type == PortableType.ExportAllCharacters |
                                       type == PortableType.ExportCharacter);
                        EnableApply();
                        return;
                    }
                    i++;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void AllAPOperationClick(object sender, EventArgs e)
        {
            try
            {
                updateBuffer = false;
                //0 = empty, 1 = master;
                byte type = Convert.ToByte(((Button)sender).Tag);

                int start = CurrentOffset +
                    ((characterID - 1) * (IsNormalMemCard ? SaveMap.CHARACTER_BLOCK_SIZE : SaveMap.CHARACTER_SECTION_LENGTH_RR)) +
                    (IsNormalMemCard ? SaveMap.CHARACTER_AP_LIST_OFFSET : 185) +
                    (IsNormalMemCard ? SaveMap.CHARACTER_SECTION_START_OFFSET : SaveMap.CHARACTER_SECTION_START_RR);
                for (int i = start, j = 0; i < start + 48; i++, j += 2)
                {
                    switch (type)
                    {

                        case 0:
                            editBuffer[i] = 0;
                            ((NumericUpDown)tabPageAbilities.Controls[j]).Value = 0;
                            break;
                        case 1:
                            editBuffer[i] = 255;
                            ((NumericUpDown)tabPageAbilities.Controls[j]).Value = 255;
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { updateBuffer = true; EnableApply(); }
        }

        private void Master_Wimp_Click(object sender, EventArgs e)
        {
            try
            {
                bool max = Convert.ToBoolean(((Button)sender).Tag);
                SetGBNUPValues(max, gpLevel.Controls);
                SetGBNUPValues(max, gpHPMP.Controls);
                SetGBNUPValues(max, gpStats.Controls);
                if (max) nupStatusBits.Value = 0;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void SetGBNUPValues(bool max, Control.ControlCollection gb)
        {
            foreach (Control c in gb)
            {
                if (c is NumericUpDown)
                    (c as NumericUpDown).Value = max ? (c as NumericUpDown).Maximum : (c as NumericUpDown).Minimum;
            }
        }

        private void MagicStone_Click(object sender, EventArgs e)
        {
            try
            {
                updateBuffer = false;
                int start = CurrentOffset +
                (IsNormalMemCard ? SaveMap.CHARACTER_SECTION_START_OFFSET : SaveMap.CHARACTER_SECTION_START_RR) +
                ((characterID - 1) *
                (IsNormalMemCard ? SaveMap.CHARACTER_BLOCK_SIZE : SaveMap.CHARACTER_SECTION_LENGTH_RR)) +
                (IsNormalMemCard ? 136 : 233);

                byte offset = Convert.ToByte(Strings.GetSeprStr((string)(sender as PictureBox).Tag, 0, ';'));
                byte bit = Convert.ToByte(Strings.GetSeprStr((string)(sender as PictureBox).Tag, 1, ';'));
                if ((editBuffer[offset + start] & bit) > 0)
                {
                    (sender as PictureBox).Image = Resources.SocketG;
                    editBuffer[offset + start] -= bit;
                }
                else
                {
                    (sender as PictureBox).Image = Resources.MS;
                    editBuffer[offset + start] += bit;
                }
                EnableApply();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { updateBuffer = true; }
        }

        private void FillEmptyMSClick(object sender, EventArgs e)
        {
            try
            {
                /*int start = CurrentOffset +
                SaveMap.CHARACTER_SECTION_START_OFFSET + ((characterID - 1) * SaveMap.CHARACTER_BLOCK_SIZE) + 0x88;*/
                int start = CurrentOffset +
                (IsNormalMemCard ? SaveMap.CHARACTER_SECTION_START_OFFSET : SaveMap.CHARACTER_SECTION_START_RR) +
                ((characterID - 1) * (IsNormalMemCard ? SaveMap.CHARACTER_BLOCK_SIZE : SaveMap.CHARACTER_SECTION_LENGTH_RR)) +
                (IsNormalMemCard ? 136 : 233);
                bool master = Convert.ToBoolean((sender as Control).Tag);
                SetGBNUPValues(master, tabPagemagicStones.Controls);
                foreach (Control c in tabPagemagicStones.Controls)
                {
                    if (c is PictureBox)
                    {
                        byte offset = Convert.ToByte(Strings.GetSeprStr((string)(c as PictureBox).Tag, 0, ';'));
                        if (master)
                        {
                            (c as PictureBox).Image = Resources.MS;
                            editBuffer[offset + start] = 255;
                        }
                        if (!master)
                        {
                            (c as PictureBox).Image = Resources.SocketG;
                            editBuffer[offset + start] = 0;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void EmptyEquip(object sender, EventArgs e)
        {
            cbEquipArms.SelectedIndex = cbEquipArms.Items.Count - 1;
            cbEquipBody.SelectedIndex = cbEquipBody.Items.Count - 1;
            cbEquipedAddOn.SelectedIndex = cbEquipedAddOn.Items.Count - 1;
            cbEquipHead.SelectedIndex = cbEquipHead.Items.Count - 1;
            cbEquipWeapon.SelectedIndex = cbEquipWeapon.Items.Count - 1;
        }

        private void btnAllCharacterOp_Click(object sender, EventArgs e)
        {
            byte tempCharID = characterID;
            try
            {
                int i = cbAllCharOp.SelectedIndex; bool master;
                if (i == 0 || i == 2 || i == 5 || i == 7)
                {
                    (sender as Button).Tag = true; master = true;
                }
                else
                {
                    (sender as Button).Tag = false; master = false;
                }
                for (characterID = 1; characterID < 10; characterID++)
                {
                    //change the control values to allow the buffer to update.

                    if (i < 4)
                    {
                        updateBuffer = false;
                        SetGBNUPValues(!master, gpLevel.Controls);
                        SetGBNUPValues(!master, gpHPMP.Controls);
                        SetGBNUPValues(!master, gpStats.Controls);
                        updateBuffer = true;
                        Master_Wimp_Click(sender, e);
                    }
                    if (i > 6 || i == 1 || i == 0)
                    {
                        updateBuffer = false;
                        nupMagStonesMax.Value = 1; nupMagStonesCurrent.Value = 1;
                        updateBuffer = true;
                        FillEmptyMSClick(sender, e);
                    }
                    if (i == 1 || i == 4)
                    {
                        updateBuffer = false;
                        cbEquipArms.SelectedIndex = 1;
                        cbEquipBody.SelectedIndex = 1;
                        cbEquipedAddOn.SelectedIndex = 1;
                        cbEquipHead.SelectedIndex = 1;
                        cbEquipWeapon.SelectedIndex = 1;
                        updateBuffer = true;
                        EmptyEquip(sender, e);
                    }
                    if (i == 1 || i == 0 || i == 5 || i == 6)
                        AllAPOperationClick(sender, e);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { updateBuffer = true; characterID = tempCharID; }
        }

        private void toolStripDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (btnAPPLY.Enabled)
                {
                    UpdateOffset(SaveMap.PREVIEW_GIL_OFFSET, (uint)numericUpDown1.Value, 3);
                    master.SetFileNameTime(currentBlock, editBuffer);
                    master.SetChecksum(currentBlock, ref editBuffer);
                }
                currentBlock = Convert.ToByte(e.ClickedItem.Text);
                Load_Variables(false);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { updateBuffer = true; }

        }

        private void btnUnknown_Changed(object sender, EventArgs e)
        {
            try
            {
                int start = 1, stop = 0;
                if (IsNormalMemCard)
                {
                    //REMEMBER: The tags are indexes, NOT offsets.
                    string tags = (sender as Control).Tag.ToString();
                    start = (int)UnknownValues.UNKNOWN_START_OFFSETS[Convert.ToInt32(tags)];
                    stop = (int)UnknownValues.UNKNOWN_END_OFFSETS[Convert.ToInt32(tags)];
                }
                else
                {
                    start = 0; stop = SaveFile_RR.SAVEFILE_DECRYPTED_SLOT_SIZE;
                }
                FormLoader.OpenHexEditor(start, stop, currentBlock, master);
                Load_Variables(false);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void nupTime_Changed(object sender, EventArgs e)
        {
            try
            {
                if (!updateBuffer) return;
                object gameTime = GetGameTime();
                if (!IsNormalMemCard)
                {
                    double dNew_value = (double)(Convert.ToDouble(nupTH.Value) * 3600.0 +
                        Convert.ToDouble(nupTM.Value) * 60 +
                        Convert.ToDouble(nupTS.Value)) +
                        (Convert.ToDouble(gameTime) % 1.0);
                    int position = SaveMap.PLAYTIME_AS_DOUBLE_OFFSET_RR;
                    foreach (byte b in BitConverter.GetBytes(dNew_value))
                        editBuffer[position++] = b;
                }
                else
                {
                    uint new_value = (uint)(nupTH.Value * 3600 * Framerate +
                        nupTM.Value * 60 * Framerate +
                        nupTS.Value * Framerate) +
                        (Convert.ToUInt32(gameTime) % Framerate);
                    UpdateOffset(SaveMap.GAME_TIME_OFFSET, new_value, 4);
                    UpdateOffset(SaveMap.PREVIEW_TIME_OFFSET, new_value, 4);
                }
                EnableApply();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void UpdateOffset(int Offset, uint value, ushort bytes)
        {
            int i = CurrentOffset + Offset;
            foreach (byte b in OffsetManager.Flap_Values(value, bytes))
                editBuffer[i++] = b;
        }

        private void Save_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                updateBuffer = false;
                e.Cancel = true;
                this.Hide();
                master.BringToFront();
                master.Focus();
            }
        }

        private void Save_Resize(object sender, EventArgs e)
        {
            panel3.Height = this.Height - panel4.Height - statusStrip1.Height - 48;
            foreach (TabPage t in tabControl.TabPages)
            {
                t.Height = tabControl.Height;
                t.Width = tabControl.Width;
            }
            groupBox1.Width = gbCardOpperations.Width = 169;
            gpCardMap.Width = this.Width - 215;
            tabControlCharacter.Height = tabControl.Height - 118;
            flowLayoutPanel3.Height = tabControlCharacter.Height - (32 + label52.Height * 2);

            if (master != null)
                SendScreenToReg(master.regKey);
        }

        private void Save_KeyDown(object sender, KeyEventArgs e)
        {
            if (blocksNrs == null) return;

            if (e.Alt && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down))
            {
                int i = Array.IndexOf(blocksNrs, currentBlock);
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
                {
                    if (i == 0) i = blocksNrs.Length - 1; else i--;
                }
                else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
                {
                    if (i == blocksNrs.Length - 1) i = 0; else i++;
                }
                if (currentBlock == blocksNrs[i]) return;
                currentBlock = blocksNrs[i];
                e.Handled = true;
                Load_Variables(false);
            }
        }

        private void gwItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                    btnItemSingleEmpty_Click(null, null);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        /// <summary>
        /// 0 = atck, 1 = phDef, 2 = MDef
        /// </summary>
        readonly byte[,] MAX_CARD_STATS =
                {
                    { 7, 9, 4 },
                    { 9, 10, 4 },
                    { 11, 12, 10 },
                    { 13, 6, 19 },
                    { 15, 13, 13 },
                    { 17  ,    15,      8},
                    { 19  ,     19,      11},
                    {        21  ,     12 ,     21},
                    {     23  ,    23   ,   13},
                    {     25  ,    18    ,  4},
                    {        27  ,     6     ,  26},
                    {       29  ,     20     , 27},
                    {       31  ,     9      , 33},
                    {  33  ,     15,      39},
                    {     35  ,    36  ,    8},
                    {   37  ,    37,      17},
                    {       39  ,     12    ,  38},
                    {  41  ,    38,      16},
                    {     43  ,    11     , 34},
                    {   45  ,    40 ,     19},
                    {    47  ,     29,      25},
                    {    49  ,    45   ,   4},
                    {     51  ,    48    ,  27},
                    {     53  ,    195    , 4},
                    {   55  ,     33     , 29},
                    {    57  ,     34      ,30},
                    {   59  ,     22,      40},
                    { 61  ,    68     , 12},
                    {        63  ,    37    ,  18},
                    {       65  ,    62     , 34},
                    {   67  ,    91  ,    18},
                    { 69  ,    59    ,  58},
                    {   71  ,     32    ,  96},
                    {     73  ,    64     , 8},
                    {   75  ,     43     , 39},
                    {    77  ,    65   ,   71},
                    {  79  ,     45   ,   41},
                    {    81  ,     10   ,   70},
                    {        83  ,    80    ,  29},
                    {   85  ,     36    ,  75},
                    {    87  ,     50    ,  50},
                    {      89  ,     80 ,     17},
                    {    91  ,     51  ,    47},
                    {       93  ,     52 ,     48},
                    {    84  ,     37  ,    54},
                    {    41  ,    54    ,  50},
                    {     90  ,     30 ,     145},
                    {      98  ,     72  ,    29},
                    {     86  ,     57   ,   99},
                    {       102 ,     250   ,  8},
                    {      125 ,     105   ,  45},
                    {    189 ,    71   ,   106},
                    {    197 ,    110  ,   12},
                    { 236 ,    125 ,    194},
                    {        221 ,    6    ,   199},
                    {       250 ,     200  ,   20},
                    {        134 ,     40   ,   63},
                    {      190 ,     162   ,  2},
                    {       208 ,     17     , 17},
                    {       83  ,     6     ,  95},
                    {       100 ,     150  ,   17},
                    {       74  ,     29   ,   103},
                    {      66  ,     100  ,   100},
                    {        205 ,     136  ,   72},
                    {   183 ,     100  ,   22},
                    {     200 ,     145  ,   83},
                    {     226 ,     96    ,  90},
                    {      139 ,     36 ,     22},
                    {      162 ,     22  ,    100},
                    {   225 ,     183  ,   86},
                    { 255 ,    180   ,  6},
                    {   248 ,    24 ,     102},
                    {    202 ,    180   ,  56},
                    {      100 ,     100  ,   100},
                    { 199 ,     56    ,  195},
                    {      12  ,     200  ,   255},
                    {    12  ,    5  ,     19},
                    {  112 ,    60  ,    10},
                    {       10  ,    105    , 175},
                    {  32  ,    4   ,    6},
                    {    143 ,    144  ,   20},
                    {    98  ,    62   ,   16},
                    {  185 ,     145   ,  201},
                    {  45  ,    100   ,  10},
                    {    99  ,    75 ,     2},
                    {    143 ,    20    ,  144},
                    {    33  ,    106 ,    19},
                    {   228 ,    145    , 32},
                    {     3   ,    5    ,   12},
                    { 2  ,    30   ,   30},
                    {     3   ,     5     ,  12},
                    {        2   ,    2   ,    2},
                    {       40  ,    33  ,    6},
                    {  4   ,    178  ,   100},
                    {    6   ,    100  ,   178},
                    {   113 ,     88 ,     88},
                    {     46  ,    17  ,    56 },
                    {   127,     127 ,    127},
                    { 128, 127, 127 },
                    { 129, 127, 127 }
                };
    }
}