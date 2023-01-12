using System;
using System.Drawing;
using System.Windows.Forms;
using Memoria.BaseOperations;
using Microsoft.Win32;

namespace Memoria
{
    public partial class BaseForm : Form
    {
        protected float fontSize = 8.25f;
        protected const float BASE_FONT_SIZE = 8.25f;
        protected const float MIN_FONT_SIZE = 6.75f;

        /// <summary>
        /// Operative system the application runs on. 
        /// </summary>
        public OperatingSystem os;

        /// <summary>
        /// OS flags. index 0 = windows, 1 = macosx, 2 = unix 
        /// </summary>
        public bool[] oss = new bool[3];

        protected int percentageDiff
        {
            get
            {
                if (fontSize < BASE_FONT_SIZE) return 100;
                return 100 +
                (((int)(fontSize < MIN_FONT_SIZE ? MIN_FONT_SIZE : fontSize - BASE_FONT_SIZE)) * 10);
            }
        }

        public BaseForm()
        {
            InitializeComponent();
            Setup();
        }

        protected virtual void Setup()
        {
            os = Environment.OSVersion;
            oss[0] = os.Platform == PlatformID.Win32Windows | os.Platform == PlatformID.Win32NT;
            oss[2] = os.Platform == PlatformID.Unix;
            oss[1] = os.Platform == PlatformID.MacOSX;
            
            this.AutoScaleMode = AutoScaleMode.None;
            Font font = new System.Drawing.Font(FontFamily.GenericSansSerif, BASE_FONT_SIZE);
            this.Font = font;

        }

        protected virtual int CalcNewValue(int value)
        {
            return Convert.ToInt32(Numbers.AdjustValue(value, percentageDiff));
        }

        protected virtual void SendScreenToReg(RegistryKey key)
        {
            try
            {
                if (this.WindowState != FormWindowState.Maximized)
                {
                    GRegistry.SetRegValue(key, this.Name + "_Width_" + (int)fontSize,
                        this.Width, RegistryValueKind.DWord);
                    GRegistry.SetRegValue(key, this.Name + "_Height_" + (int)fontSize,
                        this.Height, RegistryValueKind.DWord);
                }
                GRegistry.SetRegValue(key, this.Name + "wstate", this.WindowState, RegistryValueKind.DWord);
            }
            catch (Exception) { /*silent*/ }
        }

        protected virtual void SetFormSizeFromReg(RegistryKey key)
        {
            try
            {
                if ((FormWindowState)GRegistry.GetRegValue(key, this.Name + "wstate", FormWindowState.Normal) == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Maximized;
                    return;
                }
                int w = Int32.Parse(GRegistry.GetRegValue(key,
                    this.Name + "_Width_" + (int)fontSize, this.Width).ToString());
                int h = Int32.Parse(GRegistry.GetRegValue(key,
                    this.Name + "_Height_" + (int)fontSize, this.Height).ToString());
                this.Size = new Size(w, h);
            }
            catch (Exception) { /*silent*/ }
        }
    }
}
