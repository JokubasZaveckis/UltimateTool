using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Drawing2D;
using System.Net;

namespace UltimateTool
{
    public partial class Overlay : Form
    {
        private Screenshot parentForm;
        private Rectangle selectedRegion;
        private RectangleForm childForm;
        private Point startPoint, endPoint;

        public Overlay(Screenshot parentForm)
        {
            this.parentForm = parentForm;
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0.5;
            this.WindowState = FormWindowState.Maximized;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            this.Cursor = Cursors.Cross;

            this.MouseDown += Overlay_MouseDown;
            this.MouseMove += Overlay_MouseMove;
            this.MouseUp += Overlay_MouseUp;

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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            parentForm.WindowState = FormWindowState.Normal;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, Color.Black)))
            {
                e.Graphics.FillRectangle(brush, Bounds);
            }

            if (parentForm.IsSelecting)
            {
                using (Pen pen = new Pen(Color.Transparent, 2))
                {
                    e.Graphics.DrawRectangle(pen, parentForm.SelectionRect);
                }
                selectedRegion = parentForm.SelectionRect;
            }
            else
            {
                selectedRegion = Rectangle.Empty;
            }
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            if (selectedRegion != Rectangle.Empty)
            {
                Rectangle screenRect = new Rectangle(
                    selectedRegion.Left + Location.X,
                    selectedRegion.Top + Location.Y,
                    selectedRegion.Width,
                    selectedRegion.Height);

                Region formRegion = new Region(new Rectangle(0, 0, Width, Height));
                formRegion.Exclude(screenRect);

                Region = formRegion;
            }
        }

        private void Overlay_MouseDown(object sender, MouseEventArgs e)
        {
            // Check if the mouse is inside the selected region
            if (selectedRegion.Contains(e.Location))
            {
                // Prevent further propagation of mouse events to underlying windows
                this.Capture = true;
                startPoint = e.Location;
            }
        }
        private void Overlay_MouseMove(object sender, MouseEventArgs e)
        {
            // Check if the mouse is inside the selected region
            if (selectedRegion.Contains(e.Location))
            {
                // Change cursor to a different one if the mouse is inside the selected region
                this.Cursor = Cursors.Hand;
            }
            else
            {
                // Reset cursor to default if the mouse is outside the selected region
                this.Cursor = Cursors.Cross;
            }
        }
        private void Overlay_MouseUp(object sender, MouseEventArgs e)
        {
            // Check if the mouse is inside the selected region
            if (selectedRegion.Contains(e.Location))
            {
                // Prevent further propagation of mouse events to underlying windows
                this.Capture = true;
                endPoint = e.Location;
            }
        }

    }
}
