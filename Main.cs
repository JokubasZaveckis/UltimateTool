using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltimateTool
{
    public partial class Main : Form
    {
        private Screenshot[] ScreenshotOverlays;
        private PictureToText[] PictureToTextOverlays;
        public Main()
        {
            InitializeComponent();
            InitializeOverlayForms();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeOverlayForms();
            foreach (var overlay in ScreenshotOverlays)
            {
                overlay.ShowOverlay();
            }
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AutoClicker a1 = new AutoClicker(this);
            a1.Show();
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InitializePTTOverlayForms();
            foreach (var overlay in PictureToTextOverlays)
            {
                overlay.ShowOverlay();
            }
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// For screenshot app START
        /// </summary>
        private void InitializeOverlayForms()
        {
            Screen[] screens = Screen.AllScreens;
            ScreenshotOverlays = new Screenshot[screens.Length];
            for (int i = 0; i < screens.Length; i++)
            {
                ScreenshotOverlays[i] = new Screenshot(screens[i], this);
            }
        }

        public void CloseAllOverlays()
        {
            foreach (var overlay in ScreenshotOverlays)
            {
                overlay.Close();
            }
        }

        public void ResetAllOverlays()
        {
            foreach (var overlay in ScreenshotOverlays)
            {
                overlay.ResetCurrentRect();
            }
        }

        /// END
        /// 
        /// <summary>
        /// For PictureToText app START
        /// </summary>
        private void InitializePTTOverlayForms()
        {
            Screen[] screens = Screen.AllScreens;
            PictureToTextOverlays = new PictureToText[screens.Length];
            for (int i = 0; i < screens.Length; i++)
            {
                PictureToTextOverlays[i] = new PictureToText(screens[i], this);
            }
        }

        public void CloseAllPTTOverlays()
        {
            foreach (var overlay in PictureToTextOverlays)
            {
                overlay.Close();
            }
        }

        public void ResetAllPTTOverlays()
        {
            foreach (var overlay in PictureToTextOverlays)
            {
                overlay.ResetCurrentRect();
            }
        }

        /// END
    }
}
