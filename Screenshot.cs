using System;
using System.Drawing;
using System.Windows.Forms;

namespace UltimateTool
{
    public partial class Screenshot : Form
    {
        private Main mainForm;
        private Overlay overlay;

        private System.Drawing.Image myImage;
        private Rectangle selectionRect = new Rectangle();
        private Point startLocation;
        private bool isSelecting = false;



        public Screenshot(Main mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
            overlay = new Overlay(this);
            this.KeyPreview = true;
        }

        private void button1_Click(object sender, EventArgs e) // take a screenshot button
        {
            // Check if overlay is null or disposed, and only then create a new instance
            if (overlay == null || overlay.IsDisposed)
            {
                overlay = new Overlay(this);
                overlay.StartPosition = FormStartPosition.Manual;
                overlay.Bounds = SystemInformation.VirtualScreen; // This should already span all monitors - doesnt work :)
            }

            overlay.Show();
            this.Hide();

            overlay.MouseDown -= OverlayForm_MouseDown;
            overlay.MouseDown += OverlayForm_MouseDown;
            overlay.MouseMove -= OverlayForm_MouseMove;
            overlay.MouseMove += OverlayForm_MouseMove;
            overlay.MouseUp -= OverlayForm_MouseUp;
            overlay.MouseUp += OverlayForm_MouseUp;
        }

        private void button2_Click(object sender, EventArgs e) // bind a button
        {

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            mainForm.WindowState = FormWindowState.Normal;
        }

        private void OverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!isSelecting)
                {
                    isSelecting = true;
                    startLocation = e.Location;
                }
            }
        }

        private void OverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                int left = Math.Min(startLocation.X, e.X);
                int top = Math.Min(startLocation.Y, e.Y);
                int width = Math.Abs(e.X - startLocation.X);
                int height = Math.Abs(e.Y - startLocation.Y);

                selectionRect = new Rectangle(left, top, width, height);

                overlay.Invalidate();
            }
        }

        private void OverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                // Check if the width and height of the selection rectangle are greater than zero
                if (selectionRect.Width > 0 && selectionRect.Height > 0)
                {
                    // Capture only the selected region
                    Bitmap screenshot = new Bitmap(selectionRect.Width, selectionRect.Height);
                    using (Graphics g = Graphics.FromImage(screenshot))
                    {
                        g.CopyFromScreen(selectionRect.Location, Point.Empty, screenshot.Size);
                    }

                    myImage = screenshot;

                    isSelecting = false;
                }
            }
        }

        public bool IsSelecting
        {
            get { return isSelecting; }
            set { isSelecting = value; }
        }

        public Overlay OverlayForm
        {
            get { return overlay; }
        }

        public Rectangle SelectionRect
        {
            get { return selectionRect; }
        }

        public Image MyImage
        {
            get { return myImage; }
        }
    }
}
