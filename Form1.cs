using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnamelDesktop_Injector
{
    public partial class Form1 : Form
    {
        bool overrideWindowProc = false;
        /*protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0046 && overrideWindowProc) // windowposchanging
            {
                SendToBack();
                //m.Result = (IntPtr)0;
                //base.WndProc(ref m);
                return;
            }
            base.WndProc(ref m);
        }*/

        public static int FRAME_CAP = -1;
        public static int FRAMES_LOGGED = 0;
        public static float FRAME_DELAY;

        public string ToBase64(string input)
        {
            if (input == null) return null;

            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
        public string FromBase64(string input)
        {
            if (input == null) return null;

            var bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }

        internal Rectangle GetRectangleFromIcon(DesktopIcon i)
        {
            int size = DesktopIcon.ICON_SIZE;
            return new Rectangle((int)i.x, (int)i.y, size, size);
        }
        internal PointF GetSnapToGridPoint(DesktopIcon ico)
        {
            float pad = DesktopIcon.ICON_WITH_PADDING(); // All this is is the padding between icons. Not relevant.
            float x = ico.target.X / pad; // So we're saying "x = (icon's x / padding)"
            float y = ico.target.Y / pad; // So we're saying "y = (icon's y / padding)"
            x = (float)Math.Round(x); // Now we round x to the nearest whole number.
            y = (float)Math.Round(y); // Now we round y to the nearest whole number.
            return new PointF(x * pad, y * pad); // Return a "PointF" of the x and y locations multiplied by the padding.
        }
        internal bool PointIntersectsWithRectangle(Point _a, Rectangle b)
        {
            Rectangle a = new Rectangle(_a.X, _a.Y, 1, 1);
            return a.IntersectsWith(b);
        }

        Point lastHold;
        Point curHold;

        bool mbDown = false;
        int capturedIconID = -1;
        PointF captureOffset = PointF.Empty;

        int shadowOffset = 1;
        int fadeLevel = -1;

        bool needsPainting = true;
        public DesktopManager dm;
        public Stopwatch sw;

        public int monitorIndex;
        public Screen runOnMonitor = null;

        Wallpaper wp;

        public Form1()
        {
            InitializeComponent();
            GotFocus += Form1_GotFocus;
            Application.Idle += OnFrameCalled;
            sw = new Stopwatch();
            sw.Start();

            wp = new Wallpaper();
            SendToBack();
        }
        private void Form1_GotFocus(object sender, EventArgs e)
        {
            SendToBack();
            wp.ResetSW();
        }
        private bool Decide_CanFrame()
        {
            if(!needsPainting)
            {
                return false;
            }
            if(ActiveForm != this)
            {
                return false;
            }

            long ms = sw.ElapsedMilliseconds;
            return (FRAMES_LOGGED * FRAME_DELAY) < ms;
        }
        private List<int> GetAvailableRefreshRates(Win32.DEVMODE[] toscan)
        {
            List<int> ints = new List<int>();
            foreach(Win32.DEVMODE dv in toscan)
            {
                ints.Add(dv.dmDisplayFrequency);
            }
            ints.Sort();
            return ints;
        }
        private void OnFrameCalled(object sender, EventArgs e)
        {
            if (!Decide_CanFrame())
                return;
            FRAMES_LOGGED++;

            /*if (WindowState != FormWindowState.Normal)
            {
                WindowState = FormWindowState.Normal;
            }*/
            for (int i = 0; i < dm.IconCount(); i++)
            {
                var ico = dm.icons[i];
                if(ico.ID == capturedIconID
                && capturedIconID != -1)
                {
                    Point pos = GetCorrectedCursorPosition();
                    if (!pos.Equals(lastHold))
                    {
                        PointF n = new PointF(pos.X - captureOffset.X,
                            pos.Y - captureOffset.Y);
                        ico.target = n;
                        dm.icons[i] = ico;
                    }
                }
            }
            if(needsPainting)
                Invalidate();
        }
        private Point GetCorrectedCursorPosition()
        {
            // Compensate for other monitors.
            Point pos = Cursor.Position;
            pos.X -= runOnMonitor.Bounds.X;
            pos.Y -= runOnMonitor.Bounds.Y;
            return pos;
        }

        public int attemptToUseMonitor;
        public bool fromEnamel = false;
        public int[] leftOutMonitors;
        public List<Process> externEnamelInstances = new List<Process>();
        private void Form1_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            DoubleBuffered = true;

            Screen[] screens = Screen.AllScreens;
            if (runOnMonitor == null)
            {
                monitorIndex = attemptToUseMonitor;
                runOnMonitor = screens[monitorIndex];
            }
            /*if(screens.Length > 1)
            {
                List<int> leftOut = new List<int>();
                // Get remainder monitor(s)
                for(int i = 0; i < screens.Length; i++)
                {
                    if (i == attemptToUseMonitor) continue;
                    leftOut.Add(i);
                }
                leftOutMonitors = leftOut.ToArray();
            } else
                leftOutMonitors = new int[0];

            if (!fromEnamel) {
                foreach (int toShow in leftOutMonitors)
                {
                    string curPath = Assembly.GetExecutingAssembly().Location;
                    Process newEnamel = Process.Start(curPath, toShow.ToString());
                    extraEnamelInstances.Add(newEnamel);
                }
            }*/

            Rectangle rct = runOnMonitor.WorkingArea;
            // Use working area.
            Win32.SetLocationAndSize(this, rct.Location, rct.Size);

            ShowInTaskbar = false;

            Win32.DEVMODE[] dvmodes;
            dvmodes = Win32.GetMonitorSettings();

            List<int> rf = GetAvailableRefreshRates(dvmodes);
            FRAME_CAP = rf.Max();
            FRAME_DELAY = 1000.0f / FRAME_CAP;

            // Load as the last operation.
            if (!TryLoadLayout())
            {
                dm = new DesktopManager();
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!overrideWindowProc)
                overrideWindowProc = true;

            int iconExpand = 5;

            if (e.Graphics.CompositingQuality != CompositingQuality.HighSpeed)
                e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;

            if (e.Graphics.SmoothingMode != SmoothingMode.HighSpeed)
                e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;

            if (e.Graphics.PixelOffsetMode != PixelOffsetMode.HighSpeed)
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            if (e.Graphics.InterpolationMode != InterpolationMode.NearestNeighbor)
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            if (wp.HasWallpaper())
            {
                Bitmap b = wp.StepGifFrame();
                e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                e.Graphics.DrawImageUnscaled(b, Point.Empty);
                e.Graphics.CompositingMode = CompositingMode.SourceOver;
            }

            DesktopIcon[] icons = dm.icons;
            for(int xx = 0; xx < dm.IconCount(); xx++)
            {
                DesktopIcon i = icons[xx];
                Bitmap bm = i.image;

                i.TryPhysicsTick();

                // code
                if (i.selected)
                {
                    using (Brush br = new SolidBrush(Color.FromArgb(100, 80, 80, 255)))
                    {
                        var rec = GetRectangleFromIcon(i);

                        rec.X -= iconExpand;
                        rec.Y -= iconExpand;
                        rec.Width += iconExpand * 2;
                        rec.Height += iconExpand * 2;

                        e.Graphics.FillRectangle(br, rec);
                    }
                }
                using (Font f = new Font(FontFamily.GenericSansSerif, DesktopIcon.ICON_SIZE / 5))
                {
                    int offset = 0;
                    SizeF ss = e.Graphics.MeasureString(i.name, f);
                    int width = (int)ss.Width;
                    if(width > DesktopIcon.ICON_SIZE)
                    {
                        int dif = DesktopIcon.ICON_SIZE - width;
                        offset = dif / 2;
                    }
                    if (width < DesktopIcon.ICON_SIZE)
                    {
                        int dif = DesktopIcon.ICON_SIZE - width;
                        offset = dif / 2;
                    }
                    e.Graphics.DrawString(i.name, f, Brushes.Black, i.x + offset + shadowOffset, i.y + DesktopIcon.ICON_SIZE + shadowOffset);
                    e.Graphics.DrawString(i.name, f, Brushes.White, i.x + offset, i.y + DesktopIcon.ICON_SIZE);
                }
                e.Graphics.DrawImage(bm, new PointF(i.x, i.y));
            }

            int alphaStep = 15; // 15(10) = 150

            if (mbDown && capturedIconID == -1)
            {
                using (Pen p = new Pen(new SolidBrush(Color.FromArgb(255, 20, 20, 255)), 1))
                using (Brush b = new SolidBrush(Color.FromArgb(alphaStep*10, 100, 100, 255)))
                {
                    curHold = GetCorrectedCursorPosition();
                    Point cur = curHold;
                    Point last = lastHold;

                    int minX = Math.Min(cur.X, last.X);
                    int minY = Math.Min(cur.Y, last.Y);

                    int width = Math.Abs(cur.X - last.X);
                    int height = Math.Abs(cur.Y - last.Y);

                    Rectangle rect = new Rectangle(minX, minY, width, height);
                    for (int i = 0; i < dm.IconCount(); i++)
                    {
                        DesktopIcon ico = icons[i];
                        if(rect.IntersectsWith(GetRectangleFromIcon(ico)))
                        {
                            ico.selected = true;
                            icons[i] = ico;
                            continue;
                        }
                    }
                    dm.icons = icons;

                    e.Graphics.FillRectangle(b, rect);
                    e.Graphics.DrawRectangle(p, rect);
                }
            } else if(!mbDown && fadeLevel != -1 && capturedIconID == -1)
            {
                using (Brush b = new SolidBrush(Color.FromArgb(alphaStep * fadeLevel, 100, 100, 255)))
                {
                    fadeLevel--;

                    Point cur = curHold;
                    Point last = lastHold;

                    int minX = Math.Min(cur.X, last.X);
                    int minY = Math.Min(cur.Y, last.Y);

                    int width = Math.Abs(cur.X - last.X);
                    int height = Math.Abs(cur.Y - last.Y);

                    e.Graphics.FillRectangle(b,
                        new Rectangle(minX, minY, width, height));
                }
            }
        }

        // INPUT
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            DesktopIcon[] icons = dm.icons;
            Point pos = GetCorrectedCursorPosition();
            for (int i = 0; i < dm.IconCount(); i++)
            {
                DesktopIcon ico = icons[i];
                ico.selected = false;

                Rectangle rect = GetRectangleFromIcon(ico);
                Rectangle mouse = new Rectangle(pos, new Size(1, 1));
                if (mouse.IntersectsWith(rect))
                {
                    ico.selected = true;
                    capturedIconID = ico.ID;
                    captureOffset = new PointF(
                      pos.X-ico.x, pos.Y-ico.y);
                }
                icons[i] = ico;
            }
            lastHold = e.Location;
            mbDown = true;
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if(capturedIconID != -1)
            {
                for(int i = 0; i < dm.IconCount(); i++)
                {
                    var ico = dm.icons[i];
                    if(ico.ID == capturedIconID)
                    {
                        if(snap)
                        {
                            ico.target = GetSnapToGridPoint(ico);
                            dm.icons[i] = ico;
                        }
                    }
                }
                capturedIconID = -1;
            } else
            {
                fadeLevel = 9;
            }
            mbDown = false;
        }
        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            DesktopIcon[] icons = dm.icons;
            Point pos = GetCorrectedCursorPosition();
            bool opened = false;
            for (int i = 0; i < dm.IconCount(); i++)
            {
                DesktopIcon ico = icons[i];

                Rectangle rect = GetRectangleFromIcon(ico);
                Rectangle mouse = new Rectangle(pos, new Size(1, 1));
                if(mouse.IntersectsWith(rect)) {
                    opened = true;
                    //Process.Start(ico.loc); // start the program
					Process p = new Process();
                    p.StartInfo.FileName = ico.loc;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(ico.loc);
                    p.Start();
                    p.Exited += (object _, EventArgs _ev) =>
                    {
                        p.Dispose();
                    };
                }

                ico.selected = false;
                icons[i] = ico;
                dm.icons = icons;
            }
            if(!opened)
            {
                Random r = new Random();
                for (int i = 0; i < dm.IconCount(); i++)
                {
                    DesktopIcon ico = icons[i];
                    float a = (float)r.NextDouble() * 200;
                    a -= (a / 2);
                    float b = (float)r.NextDouble() * 200;
                    b -= (b / 2);
                    ico.velocity_x = a;
                    ico.velocity_y = b;
                    icons[i] = ico;
                }
                dm.icons = icons;
            }
        }

        bool snap = false;
        private void ProcessDesktopContext(MouseEventArgs e)
        {
            DesktopContext dct = new DesktopContext(snap);
            void handler(object snd, FormClosedEventArgs args)
            {
                int pick = dct.buttonIndex;
                dct.Dispose();
                if (pick == 1)
                {
                    if (!snap)
                    {
                        snap = true;
                        // Snap all icons to grid.
                        for (int i = 0; i < dm.IconCount(); i++)
                        {
                            DesktopIcon ico = dm.icons[i];
                            ico.target = GetSnapToGridPoint(ico);
                            dm.icons[i] = ico;
                        }
                    }
                    else
                    {
                        snap = false;
                    }
                }
                else if (pick == 2)
                {
                    // Create new icon.
                    CreateIconWindow ciw = new CreateIconWindow();
                    ciw.Location = Cursor.Position;
                    ciw.ShowDialog();
                    if (!ciw.IsConfirmed())
                    {
                        return;
                    }
                    if(ciw.appdir.ToLower().EndsWith(".lnk")) {
                        Shell32.Shell sh = new Shell32.Shell();
                        Shell32.Folder scf = sh.NameSpace(Path.GetDirectoryName(ciw.appdir));
                        Shell32.FolderItem item = scf.ParseName(Path.GetFileName(ciw.appdir));
                        if(item==null)
                        {
                            MessageBox.Show("Invalid .lnk file? null");
                            return;
                        }
                        if (!item.IsLink)
                        {
                            MessageBox.Show("Invalid .lnk file? nonlink");
                            //MessageBox.Show(item.Name + "\n" + item.IsLink + "\n" + item.Path + "\n" + item.Type);
                            return;
                        }
                        Shell32.ShellLinkObject lnk = (Shell32.ShellLinkObject)item.GetLink;
                        DesktopIcon ico = new DesktopIcon(e.X, e.Y, lnk.Target.Path, dm)
                        {
                            name = ciw.name
                        };
                        dm.AddIcon(ico);
                    } else
                    {
                        DesktopIcon ico = new DesktopIcon(e.X, e.Y, ciw.appdir, dm)
                        {
                            name = ciw.name
                        };
                        dm.AddIcon(ico);
                    }
                    ciw.Dispose();
                }
                else if (pick == 3)
                {
                    // Change wallpaper.
                    OpenFileDialog ofd = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        Multiselect = false,
                        Filter = "Image/Video Files (*.jpg, *.jpeg, *.png, *.gif, *.mp4)|*.jpg;*.jpeg;*.png;*.gif;*.mp4;"
                    };

                    var res = ofd.ShowDialog();

                    if (res != DialogResult.OK)
                    {
                        ofd.Dispose();
                        return;
                    }
                    wp.SetWallpaper(runOnMonitor, ofd);
                    return;
                }
                else if (pick == 4)
                {
                    // Save
                    SaveLayout();
                    return;
                }
                else if (pick == 5)
                {
                    // Load
                    if (!TryLoadLayout())
                    {
                        MessageBox.Show("Couldn't load any existing layout files. Try saving first.");
                        return;
                    }
                    return;
                }
            }
            dct.FormClosed += handler;
            dct.Show();
            dct.Location = Cursor.Position;
        }
        private void ProcessIconContext(MouseEventArgs e, DesktopIcon ico, int index)
        {
            IconContext ict = new IconContext();
            void handler(object snd, FormClosedEventArgs args)
            {
                int pick = ict.buttonIndex;
                ict.Dispose();
                if(pick == 1) {
                    // Admin
                    Process p = new Process();
                    p.StartInfo.FileName = ico.loc;
                    p.StartInfo.Verb = "runas";
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(ico.loc);
                    try {
                        p.Start();
                    } catch (Exception) { }
                    p.Exited += (object _, EventArgs _ev) =>
                    {
                        p.Dispose();
                    };
                } else if(pick == 2) {
                    // Rename
                    RenameIconWindow riw = new RenameIconWindow(ico.name);
                    riw.Location = Cursor.Position;
                    riw.ShowDialog();
                    string name = riw.name;
                    riw.Dispose();

                    ico.name = name;
                    dm.icons[index] = ico;
                } else if (pick == 3) {
                    // Delete
                    List<DesktopIcon> ii = dm.icons.ToList();
                    ii.Remove(ico);
                    dm.icons = ii.ToArray();
                }
            }
            ict.FormClosed += handler;
            ict.Show();
            ict.Location = Cursor.Position;
        }
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                for(int i = 0; i < dm.IconCount(); i++)
                {
                    DesktopIcon ico = dm.icons[i];
                    Rectangle r = GetRectangleFromIcon(ico);
                    if (PointIntersectsWithRectangle(e.Location, r))
                    {
                        ProcessIconContext(e, ico, i);
                        return;
                    }
                }
                ProcessDesktopContext(e);
                return;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            wp.Dispose();
            foreach(Process enamel in externEnamelInstances)
            {
                enamel.Kill();
            }
        }

        /// <summary>
        /// Saves down the current desktop layout.
        /// </summary>
        private void SaveLayout()
        {
            string s = this.Serialize();
            File.WriteAllText("desktopSave.ser", ToBase64(s));
            System.Media.SystemSounds.Exclamation.Play();
        }

        /// <summary>
        /// Returns if succeeded to load.
        /// </summary>
        /// <returns></returns>
        private bool TryLoadLayout()
        {
            if(!File.Exists("desktopSave.ser"))
            {
                return false;
            }
            string rd = FromBase64(File.ReadAllText("desktopSave.ser"));
            string[] array = rd.Split('|');
            bool hasWallpaper = bool.Parse(array[0]);
            if(hasWallpaper)
            {
                string path = array[1];
                wp.SetWallpaper(runOnMonitor, path);
            }
            dm = new DesktopManager(rd.Split(new[] { "||||" }, StringSplitOptions.None)[1]);
            return true;
        }


        private string Serialize()
        {
            string s = "";
            bool hwp = wp.HasWallpaper();
            if(!hwp)
                s += hwp.ToString() + "||||";
            else
            {
                s += hwp.ToString() + "|";
                s += wp.wallpaperPath + "||||";
            }
                
            s += dm.Serialize();
            return s;
        }
    }
}
