using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnamelDesktop_Injector
{
    public static class Win32
    {
        // strcts
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

#if DEBUG
            public override string ToString()
            {
                return "{x=" + x + ", y=" + y + "}";
            }
#endif
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r)
            {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(
                    x,
                    y,
                    x + width,
                    y + height);
            }

            public Size Size
            {
                get
                {
                    return new Size(this.right - this.left, this.bottom - this.top);
                }
            }

            public System.Drawing.Rectangle ToRectangle()
            {
                return new Rectangle(
                    left,
                    top,
                    right - left,
                    bottom - top);
            }
#if Microsoft_PUBLIC_GRAPHICS_LIBRARY
            public override string ToString()
            {
                Size size = this.Size;
                return string.Format("{0}=[left={1}, top={2}, width={3}, height={4}]", this.GetType().Name, left, top, size.Width, size.Height);
            }
#endif
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {

            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;

        }
        public static class SetWindowPosFlags
        {
            public static readonly int
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }
        private const int CS_DROPSHADOW = 0x00020000;

        // privates
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, RECT lParam);

        // publics
        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        public static IntPtr LPARAM(int low, int high)
        {
            // no idea how this makes an lParam but thanks
            return (IntPtr)(((short)high << 16) | (low & 0xffff));
        }
        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        public static IntPtr _progMan;
        public static IntPtr _shell;
        public static IntPtr _listView;

        public static void Attach()
        {
            _progMan = FindWindow("Progman", "Program Manager");
            _shell = FindWindowEx(_progMan, IntPtr.Zero, "SHELLDLL_DefView", null);
            _listView = FindWindowEx(_shell, IntPtr.Zero, "SysListView32", "FolderView");
        }
        public static DEVMODE[] GetMonitorSettings()
        {
            List<DEVMODE> modes = new List<DEVMODE>();
            DEVMODE dev = new DEVMODE();
            int i = 0;
            while(EnumDisplaySettings(null, i, ref dev))
            {
                modes.Add(dev);
                i++;
            }
            return modes.ToArray();
        }
        public static int GetIconCount()
        {
            return (int)SendMessage(_listView, (int)LVM.GETITEMCOUNT, 0, IntPtr.Zero);
        }
        public static void SetIconLocation(int index, int x, int y)
        {
            IntPtr loc = LPARAM(x, y);
            SendMessage(_listView, (int)LVM.SETITEMPOSITION, index, loc);
        }
        public static Icon GetIconFromDLL(string dll, int index)
        {
            ExtractIconEx(dll, index, out IntPtr icon, out _, 1);
            return Icon.FromHandle(icon);
        }
        public static void SetLocationAndSize(Form target, Point loc, Size sz)
        {
            target.Location = loc;
            target.Size = sz;
        }
        public static int GetGifFramerateMs(Bitmap gif)
        {
            PropertyItem item = gif.GetPropertyItem(0x5100);
            return (item.Value[0] + item.Value[1] * 256) * 10;
        }
    }
}
