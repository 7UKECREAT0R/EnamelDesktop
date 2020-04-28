using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnamelDesktop_Injector
{
    public partial class DesktopContext : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect,
        int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);
        public void SetBorderCurve(int radius)
        {
            Region r = this.Region;
            if (r != null) { r.Dispose(); }
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, radius, radius));

            r = snap.Region;
            if (r != null) { r.Dispose(); }
            snap.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, snap.Width, snap.Height, radius, radius));

            r = createIcon.Region;
            if (r != null) { r.Dispose(); }
            createIcon.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, createIcon.Width, createIcon.Height, radius, radius));

            r = setWallpaper.Region;
            if (r != null) { r.Dispose(); }
            setWallpaper.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, setWallpaper.Width, setWallpaper.Height, radius, radius));

            r = saveButton.Region;
            if (r != null) { r.Dispose(); }
            saveButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, saveButton.Width, saveButton.Height, radius, radius));

            r = loadButton.Region;
            if (r != null) { r.Dispose(); }
            loadButton.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, loadButton.Width, loadButton.Height, radius, radius));
        }

        bool closing;
        public int buttonIndex;
        Timer t;
        public DesktopContext(bool snap)
        {
            InitializeComponent();

            if(snap)
            {
                this.snap.Text = "Disable Snap To Grid";
            } else
            {
                this.snap.Text = "Enable Snap To Grid";
            }

            buttonIndex = -1;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Deactivate += DesktopContext_Deactivate;
            Opacity = 0;
            t = new Timer();
            t.Interval = 16;
            t.Start();
            t.Tick += T_Tick;

            SetBorderCurve(10);
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if(!closing)
            {
                Opacity += 0.1;
                if (Opacity >= 1)
                {
                    t.Stop();
                    Opacity = 1;
                }
            } else
            {
                Opacity -= 0.1;
                if(Opacity == 0)
                {
                    t.Stop();
                    t.Dispose();
                    Close();
                }
            }
        }
        public void StartClose()
        {
            closing = true;
            t.Start();
        }

        private void DesktopContext_Deactivate(object sender, EventArgs e)
        {
            StartClose();
        }

        private void snap_Click(object sender, EventArgs e)
        {
            buttonIndex = 1;
            StartClose();
        }
        private void createIcon_Click(object sender, EventArgs e)
        {
            buttonIndex = 2;
            StartClose();
        }
        private void setWallpaper_Click(object sender, EventArgs e)
        {
            buttonIndex = 3;
            StartClose();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            buttonIndex = 4;
            StartClose();
        }
        private void loadButton_Click(object sender, EventArgs e)
        {
            buttonIndex = 5;
            StartClose();
        }
    }
}
