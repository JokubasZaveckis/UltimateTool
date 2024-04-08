using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace UltimateTool
{
    public partial class Screenshot : Form
    {
        private Main parentForm;
        private Screen screen;
        private bool isDrawing = false;
        private Point startPoint;
        private Rectangle currentRect = Rectangle.Empty;
        private Bitmap screenshot;

        public Screenshot(Screen screen, Main parentForm)
        {
            this.screen = screen;
            this.parentForm = parentForm;
            InitializeOverlayForm(screen);
            this.MouseDown += Overlay_MouseDown;
            this.MouseMove += Overlay_MouseMove;
            this.MouseUp += Overlay_MouseUp;
            this.KeyPreview = true;
        }

        private void InitializeOverlayForm(Screen screen)
        {
            this.BackColor = Color.Gray; // Placeholder, actual color not visible due to opacity
            this.Opacity = 0.3; // Set to 30% opacity
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = screen.Bounds;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = screen.Bounds.Location;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.Cursor = Cursors.Cross;
            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        public void ShowOverlay()
        {
            this.Show();
        }

        private void Overlay_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Reset rectangles in all overlays before starting a new drawing.
                parentForm.ResetAllOverlays();

                // Now proceed to start drawing the new rectangle.
                isDrawing = true;
                startPoint = e.Location;
                currentRect = new Rectangle(e.Location, new Size(0, 0));
                this.Invalidate(); // Invalidate to ensure the overlay is redrawn.
            }
        }

        private void Overlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                currentRect = new Rectangle(Math.Min(startPoint.X, e.X), Math.Min(startPoint.Y, e.Y), Math.Abs(e.X - startPoint.X), Math.Abs(e.Y - startPoint.Y));
                this.Invalidate();
            }
        }

        private void Overlay_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                CaptureScreenshot();
            }
        }

        private void CaptureScreenshot()
        {
            if (currentRect.Width > 0 && currentRect.Height > 0)
            {
                screenshot = new Bitmap(currentRect.Width, currentRect.Height, PixelFormat.Format32bppArgb);
                this.Visible = false;
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(new Point(currentRect.Left + screen.Bounds.Left, currentRect.Top + screen.Bounds.Top), Point.Empty, currentRect.Size);
                }
                this.Visible = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Create a region that covers the entire screen
            Region overlayRegion = new Region(this.ClientRectangle);

            // If there's a selection, exclude it from the overlay region
            if (!currentRect.IsEmpty)
            {
                overlayRegion.Exclude(currentRect);
            }

            // Apply semi-transparent overlay to only the non-excluded parts of the screen
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(200, 0, 0, 0))) // Use semi-transparent gray color
            {
                e.Graphics.FillRegion(brush, overlayRegion);
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                if (screenshot != null)
                {
                    Clipboard.SetImage(screenshot);
                    parentForm.CloseAllOverlays(); // Close all overlays
                    return true; // Key press handled
                }
            }
            else if (keyData == (Keys.Control | Keys.S))
            {
                if (screenshot != null)
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    string filePath = System.IO.Path.Combine(desktopPath, fileName);
                    screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                    parentForm.CloseAllOverlays(); // Close all overlays
                    return true; // Key press handled
                }
            }
            else if (keyData == Keys.Escape)
            {
                parentForm.CloseAllOverlays(); // Close all overlays
                return true; // Key press handled
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void ResetCurrentRect()
        {
            currentRect = Rectangle.Empty;
            this.Invalidate(); // This ensures the overlay is redrawn without the previous rectangle.
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            parentForm.WindowState = FormWindowState.Normal;
        }
    }
}
