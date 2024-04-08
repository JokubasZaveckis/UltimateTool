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
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox()
        {
            InitializeComponent();
        }

        public string MessageText { get; set; }

        public CustomMessageBox(string message)
        {
            InitializeComponent();
            MessageText = message;
            richTextBox1.Text = message;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MessageText);
            button2.Enabled = false;
        }


    }
}
