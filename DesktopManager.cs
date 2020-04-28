using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnamelDesktop_Injector
{
    public class DesktopIcon
    {
        // status
        public bool selected = false;

        public static readonly int ICON_SIZE = 48;
        public static readonly int ICON_PADDING = 15;
        public static float ICON_WITH_PADDING() { return (float)ICON_SIZE + (float)ICON_PADDING; }

        public int ID;
        public string loc;
        public Bitmap image;
        public string name;

        public float lastx, lasty;
        public float x = -1;
        public float y = -1;

        public float velocity_x = 0;
        public float velocity_y = 0;

        private bool specifiedIcon = false;
        private string icoPath = null;
        private int icoIndex = -1;
        private readonly int shadowOffset = 3;
        private readonly int shadowSize = 1;
        private readonly int shadowOpacity = 70;
        public DesktopIcon(float x, float y, string loc, DesktopManager sender)
        {
            this.loc = loc;
            this.x = x;
            this.y = y;
            this.lastx = x;
            this.lasty = y;
            this.target = new PointF(x, y);
            
            name = Path.GetFileNameWithoutExtension(loc);
            Icon ic = Icon.ExtractAssociatedIcon(loc);
            if (ic.Width != ICON_SIZE
            && ic.Height != ICON_SIZE)
            {
                Bitmap bm = ic.ToBitmap();
                Bitmap toPaste = (Bitmap)bm.Clone();
                bm.Dispose();
                bm = new Bitmap(toPaste, new Size(ICON_SIZE, ICON_SIZE));
                Graphics gr = Graphics.FromImage(bm);
                Rectangle r = new Rectangle(new Point(0, 0), new Size(ICON_SIZE, ICON_SIZE));
                gr.Clear(Color.FromArgb(0, 0, 0, 0));

                using (Bitmap shadow = DropShadow.CreateShadow(toPaste, shadowSize, shadowOpacity))
                    gr.DrawImage(shadow, r);

                gr.DrawImage(toPaste, r);
                ic.Dispose();

                image = bm;

                toPaste.Dispose();
                gr.Dispose();
            }
            else
            {
                this.image = ic.ToBitmap();
                ic.Dispose();
            }

            IEnumerable<DesktopIcon> enu = sender.icons.Where(el => el != null);
            if (enu.Count() == 0)
                ID = 1;
            else
                ID = enu.OrderByDescending(el => el.ID).First().ID + 1;
        }
        public DesktopIcon(float x, float y, string loc, string name, DesktopManager sender)
        {
            this.name = name;
            this.loc = loc;
            this.x = x;
            this.y = y;
            this.lastx = x;
            this.lasty = y;
            this.target = new PointF(x, y);

            Icon ic = Icon.ExtractAssociatedIcon(loc);
            if (ic.Width != ICON_SIZE
            && ic.Height != ICON_SIZE)
            {
                Bitmap bm = ic.ToBitmap();
                Bitmap toPaste = (Bitmap)bm.Clone();
                bm.Dispose();
                bm = new Bitmap(toPaste, new Size(ICON_SIZE, ICON_SIZE));
                Graphics gr = Graphics.FromImage(bm);
                Rectangle r = new Rectangle(new Point(0, 0), new Size(ICON_SIZE, ICON_SIZE));
                Rectangle sr = new Rectangle(new Point(shadowOffset, shadowOffset), new Size(ICON_SIZE, ICON_SIZE));
                gr.Clear(Color.FromArgb(0, 0, 0, 0));

                using (Bitmap shadow = DropShadow.CreateShadow(toPaste, shadowSize, shadowOpacity))
                    gr.DrawImage(shadow, sr);

                gr.DrawImage(toPaste, r);
                ic.Dispose();

                image = bm;

                toPaste.Dispose();
                gr.Dispose();
            }
            else
            {
                this.image = ic.ToBitmap();
                ic.Dispose();
            }

            IEnumerable<DesktopIcon> enu = sender.icons.Where(el => el != null);
            if (enu.Count() == 0)
                ID = 1;
            else
                ID = enu.OrderByDescending(el => el.ID).First().ID + 1;
        }
        public DesktopIcon(float x, float y, string loc, string name, string iconPath, int iconIndex, DesktopManager sender)
        {
            specifiedIcon = true;
            icoPath = iconPath;
            icoIndex = iconIndex;

            this.loc = loc;
            this.x = x;
            this.y = y;
            this.name = name;
            this.lastx = x;
            this.lasty = y;
            target = new PointF(x, y);

            Icon ic = Win32.GetIconFromDLL(iconPath, iconIndex);

            if(ic.Width != ICON_SIZE
            && ic.Height != ICON_SIZE)
            {
                Bitmap bm = ic.ToBitmap();
                Bitmap toPaste = (Bitmap)bm.Clone();
                bm.Dispose();
                bm = new Bitmap(toPaste, new Size(ICON_SIZE, ICON_SIZE));
                Graphics gr = Graphics.FromImage(bm);
                Rectangle r = new Rectangle(new Point(0, 0), new Size(ICON_SIZE, ICON_SIZE));
                Rectangle sr = new Rectangle(new Point(shadowOffset, shadowOffset), new Size(ICON_SIZE, ICON_SIZE));
                gr.Clear(Color.FromArgb(0, 0, 0, 0));

                using (Bitmap shadow = DropShadow.CreateShadow(toPaste, shadowSize, shadowOpacity))
                    gr.DrawImage(shadow, sr);

                gr.DrawImage(toPaste, r);
                ic.Dispose();

                image = bm;

                toPaste.Dispose();
                gr.Dispose();
            } else
            {
                this.image = ic.ToBitmap();
                ic.Dispose();
            }
            IEnumerable<DesktopIcon> enu = sender.icons.Where(el => el != null);
            if(enu.Count()==0)
                ID = 1;
            else
                ID = enu.OrderByDescending(el => el.ID).First().ID + 1;
        }

        public void SetLocation(int x, int y)
        {
            this.x = x; this.y = y;
            return;
        }
        public void SetLocation(Point p)
        {
            SetLocation(p.X, p.Y);
            return;
        }

        public PointF target = PointF.Empty;
        public static readonly float PHYSICS_FORCE = 0.2f; // pps
        public static readonly float PHYSICS_SPEED = 5f; // pps
        public static readonly float PHYSICS_DRAG = 1.04f; // pps
        public bool HasPhysicsPoint()
        {
            return !target.Equals(PointF.Empty);
        }
        public void TryPhysicsTick()
        {
            if(!HasPhysicsPoint())
            {
                return;
            }

            // Velocity to target point.
            float diffX, diffY;
            diffX = target.X - x;
            diffY = target.Y - y;
            velocity_x += diffX * PHYSICS_FORCE;
            velocity_y += diffY * PHYSICS_FORCE;

            velocity_x /= PHYSICS_DRAG;
            velocity_y /= PHYSICS_DRAG;
            if(velocity_x < 0.01 && velocity_x > -0.01)
                velocity_x = 0;
            if (velocity_y < 0.01 && velocity_y > -0.01)
                velocity_y = 0;

            lastx = x;
            lasty = y;

            x += velocity_x / 100f * PHYSICS_SPEED;
            y += velocity_y / 100f * PHYSICS_SPEED;
        }
        // -----------------------------
        public string Serialize()
        {
            string s = target.X + "~~" + target.Y + "~~" + name + "~~" + loc + "~~" + specifiedIcon;
            if(specifiedIcon)
            {
                s += "~~" + icoPath + "~~" + icoIndex;
            }
            return s;
        }
        public static DesktopIcon Deserialize(string serial, DesktopManager sender)
        {
            string[] fields = serial.Split(new string[] { "~~" },
                StringSplitOptions.RemoveEmptyEntries);
            string _x = fields[0];
            float x = float.Parse(_x);
            string _y = fields[1];
            float y = float.Parse(_y);
            string name = fields[2];
            string loc = fields[3];
            string _spec = fields[4];
            bool spec = bool.Parse(_spec);

            if(spec) {
                string icoloc = fields[5];
                string _icoindex = fields[6];
                int icoindex = int.Parse(_icoindex);
                return new DesktopIcon(x, y, loc, name, icoloc, icoindex, sender);
            } else {
                return new DesktopIcon(x, y, loc, name, sender);
            }
        }
    }
    public class DesktopManager
    {
        public DesktopIcon[] icons; // need to resize at some point
        // EDIT changed to array, it'll perform resizing automatically.

        public DesktopManager()
        {
            icons = new DesktopIcon[10];

            string winpath = Environment.GetEnvironmentVariable("windir");
            string sys32 = winpath += "\\system32\\";
            string imageres = sys32 + "\\imageres.dll";

            AddIcon(new DesktopIcon(0, 0, @"C:\$Recycle.Bin",
                "Recycle Bin", imageres, 50, this)); // 50 is the icon offset in the DLL.
            /*AddIcon(new DesktopIcon(1, 0, @"C:\$Recycle.Bin",
                "Recycle Bin", imageres, 50, this));
            AddIcon(new DesktopIcon(0, 1, @"C:\$Recycle.Bin",
                "Recycle Bin", imageres, 50, this));*/
        }
        public DesktopManager(string deserialize)
        {
            icons = new DesktopIcon[10];
            string[] serIcons = deserialize.Split(new[] { "@@" }, StringSplitOptions.None);
            foreach(string icon in serIcons)
            {
                AddIcon(DesktopIcon.Deserialize(icon, this));
            }
        }
        public int IconCount()
        {
            return icons.Count(i => i != null);
        }
        public void AddIcon(DesktopIcon ico)
        {
            // example: count = 9,
            //     length = 10
            //
            // insert at index 9
            int count = IconCount();
            if(count < icons.Length)
            {
                icons[count] = ico;
                return;
            } else
            {
                // requires resizing
                Array.Resize(ref icons, icons.Length+10);
                icons[count] = ico;
            }
        }
        // -----------------------------------
        public string Serialize()
        {
            int len = IconCount();
            string[] ser = new string[len];
            for(int i = 0; i < len; i++)
            {
                ser[i] = icons[i].Serialize();
            }
            return string.Join("@@", ser);
        }
    }
}
