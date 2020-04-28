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
    public partial class IconContext : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);
        public void SetBorderCurve(int radius)
        {
            foreach(Control c in Controls)
            {
                if(c.Region != null)
                {
                    c.Region.Dispose();
                }
                c.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, c.Width, c.Height, radius, radius));
            }
        }
        bool closing;
        public int buttonIndex;
        Timer t;
        public IconContext()
        {
            InitializeComponent();

            buttonIndex = -1;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Deactivate += IconContext_Deactivate;
            Opacity = 0;
            t = new Timer();
            t.Interval = 16;
            t.Start();
            t.Tick += T_Tick;

            SetBorderCurve(10);
        }

        private void IconContext_Deactivate(object sender, EventArgs e)
        {
            StartClose();
        }
        private void T_Tick(object sender, EventArgs e)
        {
            if (!closing)
            {
                Opacity += 0.1;
                if (Opacity >= 1)
                {
                    t.Stop();
                    Opacity = 1;
                }
            }
            else
            {
                Opacity -= 0.1;
                if (Opacity == 0)
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


        private void runAsAdmin_Click(object sender, EventArgs e)
        {
            buttonIndex = 1;
            StartClose();
        }
        private void rename_Click(object sender, EventArgs e)
        {
            buttonIndex = 2;
            StartClose();
        }
        private void delete_Click(object sender, EventArgs e)
        {
            buttonIndex = 3;
            StartClose();
        }
    }
}
