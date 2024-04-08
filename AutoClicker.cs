using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace UltimateTool
{
    public partial class AutoClicker : Form
    {
        private Main mainForm;
        private bool isLeft, isClicking;
        private int miliseconds, seconds, minutes, hours;
        private Thread clickThread;

        // DLL imports and constants for mouse event and global hotkeys
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int HOTKEY_ID = 9000; // Unique identifier for your hotkey
        private const uint VK_TAB = 0x09; // Virtual-key code for the Tab key

        public AutoClicker(Main mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            this.comboBox1.Text = "Left";
            this.KeyPreview = true;
            isClicking = false;
            button1.Enabled = false;

            // Register the Tab key as a global hotkey
            bool hotKeyRegistered = RegisterHotKey(this.Handle, HOTKEY_ID, 0, VK_TAB);
            if (!hotKeyRegistered)
            {
                MessageBox.Show("Failed to register the Tab key as a global hotkey.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.WindowState = FormWindowState.Normal;
            isClicking = false;
            UnregisterHotKey(this.Handle, HOTKEY_ID); // Unregister the hotkey when the form is closed
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = comboBox1.Text;
            isLeft = (s == "Left") ? true : false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartAutoClicker();
            this.WindowState = FormWindowState.Minimized;
        }

        private void StartAutoClicker()
        {
            if (isClicking) return;
            isClicking = true;
            clickThread = new Thread(new ThreadStart(AutoClick));
            clickThread.Start();
            button1.Enabled = false;
        }

        private void AutoClick()
        {
            int totalMiliseconds = miliseconds + seconds * 1000 + minutes * 60000 + hours * 3600000;
            while (isClicking)
            {
                if (isLeft)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                }

                Thread.Sleep(totalMiliseconds);
            }
        }

        private void StopAutoClicker()
        {
            if (clickThread != null && clickThread.IsAlive)
            {
                isClicking = false;
                clickThread.Join(); // Wait for the thread to finish
                button1.Enabled = true;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) //miliseconds
        {
            miliseconds = (int)numericUpDown1.Value;
            CheckAndToggleStartButton();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e) // seconds
        {
            seconds = (int)numericUpDown2.Value;
            CheckAndToggleStartButton();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e) //minutes
        {
            minutes = (int)numericUpDown3.Value;
            CheckAndToggleStartButton();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e) //hours
        {
            hours = (int)numericUpDown4.Value;
            CheckAndToggleStartButton();
        }

        private void CheckAndToggleStartButton()
        {
            if (miliseconds == 0 && seconds == 0 && minutes == 0 && hours == 0)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            // Listen for WM_HOTKEY messages
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == HOTKEY_ID)
            {
                // Toggle the auto-clicking without displaying message boxes
                if (isClicking)
                {
                    StopAutoClicker();
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    StartAutoClicker();
                }
            }
        }
    }
}
