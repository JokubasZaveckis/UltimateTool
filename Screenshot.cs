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
            overlay = new Overlay(this); // Pass a reference to Form1 to Form2
            this.KeyPreview = true; // Enable key preview
        }

        private void button1_Click(object sender, EventArgs e) // take a screenshot
        {
            overlay = new Overlay(this);
            overlay.StartPosition = FormStartPosition.Manual;
            overlay.Bounds = SystemInformation.VirtualScreen;

            overlay.Show();


            overlay.MouseDown += OverlayForm_MouseDown;
            overlay.MouseMove += OverlayForm_MouseMove;
            overlay.MouseUp += OverlayForm_MouseUp;

            //this.WindowState = FormWindowState.Minimized;
            this.Hide();
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
                    // Start selecting
                    isSelecting = true;
                    startLocation = e.Location; // Store the starting location
                }
            }
        }

        private void OverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                // Update the rectangle coordinates based on the mouse cursor position
                int left = Math.Min(startLocation.X, e.X);
                int top = Math.Min(startLocation.Y, e.Y);
                int width = Math.Abs(e.X - startLocation.X);
                int height = Math.Abs(e.Y - startLocation.Y);

                // Update the selection rectangle
                selectionRect = new Rectangle(left, top, width, height);

                // Redraw overlayForm with the updated rectangle
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

                    // Set the captured image in Form1
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

        // Provide a public property to access overlay
        public Overlay OverlayForm
        {
            get { return overlay; }
        }

        // Add a public property to access selectionRect
        public Rectangle SelectionRect
        {
            get { return selectionRect; }
        }

        // Add a public property to access myImage
        public Image MyImage
        {
            get { return myImage; }
        }
    }
}
