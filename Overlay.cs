using System;
using System.Drawing;
using System.Windows.Forms;

namespace UltimateTool
{
    public partial class Overlay : Form
    {
        private Screenshot parentForm;
        private Rectangle selectedRegion;

        public Overlay(Screenshot parentForm)
        {
            this.parentForm = parentForm;
            InitializeComponent();

            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0.3;
            this.WindowState = FormWindowState.Maximized;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            this.Cursor = Cursors.Cross;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Create a region that covers the entire screen
            Region overlayRegion = new Region(this.ClientRectangle);

            // If there's a selection, exclude it from the overlay region
            if (parentForm.IsSelecting && !parentForm.SelectionRect.IsEmpty)
            {
                overlayRegion.Exclude(parentForm.SelectionRect);
            }

            // Apply semi-transparent overlay to only the non-excluded parts of the screen
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
            {
                e.Graphics.FillRegion(brush, overlayRegion);
            }

            // If there's a selection, clear the inside of the rectangle
            if (parentForm.IsSelecting && !parentForm.SelectionRect.IsEmpty)
            {
                e.Graphics.SetClip(parentForm.SelectionRect);
                e.Graphics.Clear(Color.Transparent); // This will clear the inside area to be transparent
                e.Graphics.ResetClip(); // Reset the clip after clearing
            }
        }

        private bool isDragging = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!parentForm.SelectionRect.Contains(e.Location))
            {
                isDragging = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                this.Invalidate(); // This will call OnPaint to redraw the overlay
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isDragging)
            {
                isDragging = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                // Check if both Ctrl and 'C' keys are pressed
                if (parentForm.MyImage != null)
                {
                    // Copy the selected image to the clipboard
                    Clipboard.SetImage(parentForm.MyImage);

                    // Turn off the selection mode and hide the overlay
                    parentForm.IsSelecting = false;
                    this.Hide();
                    parentForm.Show();
                    return true; // Mark the key as handled
                }
            }
            else if (keyData == (Keys.Control | Keys.S))
            {
                // Check if both Ctrl and 'C' keys are pressed
                if (parentForm.MyImage != null)
                {
                    // Get the path to the user's desktop folder
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    // Save the screenshot to the desktop with a specific file name and format (e.g., PNG)
                    string filePath = System.IO.Path.Combine(desktopPath, "screenshot.png");
                    parentForm.MyImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                    // Turn off the selection mode and hide the overlay
                    parentForm.IsSelecting = false;
                    this.Hide();
                    parentForm.Show();
                    return true; // Mark the key as handled
                }
            }
            else if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                parentForm.Show();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
