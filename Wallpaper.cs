using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnamelDesktop_Injector
{
    class Wallpaper
    {
        public string wallpaperPath { get; internal set; }

        Stopwatch frameRateCounter = new Stopwatch();
        int frames = 0;
        int frameRate = 30;
        int millisDelay = -1;

        System.IO.Stream loadStream;

        private bool hasWallpaper;
        private Bitmap wallpaper;

        private bool isGif;
        private bool isMP4;
        private int frameCount;
        private int currentFrame;
        private FrameDimension dim;
        private Bitmap[] gif;
        private VideoFileReader mp4;
        private int mp4frame;
        public void Dispose()
        {
            wallpaperPath = null;
            if (wallpaper != null) wallpaper.Dispose();
            if (gif != null)
            {
                foreach (Bitmap b in gif)
                {
                    if (b != null) b.Dispose();
                }
            }
            if (loadStream != null) loadStream.Dispose();
            frameRateCounter.Stop();

            if(mp4 != null)
            {
                mp4.Close();
                mp4.Dispose();
                mp4frame = 0;
            }
        }

        public void ResetSW()
        {
            frameRateCounter.Restart();
            frames = 0;
        }
        public void SetMillisDelay()
        {
            millisDelay = (int)Math.Round(1000.0 / frameRate);
        }
        public Wallpaper()
        {
            hasWallpaper = false;
            wallpaper = null;
            frames = 0;
            frameRate = 30;
            SetMillisDelay();
        }
        /// <summary>
        /// Returns if this wallpaper instance has a valid image attached.
        /// </summary>
        /// <returns></returns>
        public bool HasWallpaper()
        {
            return hasWallpaper;
        }
        /// <summary>
        /// Gets the current wallpaper bitmap, if it exists.
        /// </summary>
        /// <returns>The current wallpaper image, or null if doesn't exist.</returns>
        public Bitmap GetWallpaper()
        {
            if(wallpaper==null)
                return null;
            return wallpaper;
        }
        public void SetWallpaper(Screen scr, OpenFileDialog ofd)
        {
            SetWallpaper(scr, ofd.FileName);
            ofd.Dispose();
        }
        /// <summary>
        /// Sets/resizes the wallpaper according to the openfiledialog and screen size.
        /// </summary>
        /// <param name="scr">The screen to scale for.</param>
        /// <param name="txt">The path of the wallpaper.</param>
        public void SetWallpaper(Screen scr, string txt)
        {
            Dispose();

            this.wallpaperPath = txt;

            if (wallpaper != null) { wallpaper.Dispose(); }

            Rectangle rct = scr.Bounds;
            Size sz = new Size(rct.Width, rct.Height - 1);


            if (txt.EndsWith(".mp4"))
            {
                // MP4 FILE
                mp4 = new VideoFileReader();
                mp4.Open(txt);
                int frames = Convert.ToInt32(mp4.FrameCount);

                if(frames == 0 || frames == 1)
                {
                    MessageBox.Show("This video codec cannot be played with FFMPEG.", "Wallpaper Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hasWallpaper = false;
                    isGif = false;
                    Dispose();
                    return;
                }

                frameRate = (int)mp4.FrameRate.Value;
                SetMillisDelay();

                isGif = false;
                currentFrame = 0;
                frameCount = frames;
                hasWallpaper = true;
                isMP4 = true;
                mp4frame = 0;

                ResetSW();
            }
            else
            {
                isMP4 = false;
                Bitmap temp;
                loadStream = new FileStream(txt, FileMode.Open, FileAccess.Read);
                temp = (Bitmap)Image.FromStream(loadStream);

                dim = new FrameDimension(temp.FrameDimensionsList[0]);
                int frames = temp.GetFrameCount(dim);
                if (frames == 1)
                {
                    wallpaper = new Bitmap(sz.Width, sz.Height, PixelFormat.Format32bppPArgb);
                    using (Graphics gr = Graphics.FromImage(wallpaper))
                    { gr.DrawImage(temp, new Rectangle(new Point(0, 0), sz)); }
                    temp.Dispose();
                    hasWallpaper = true;
                    isGif = false;
                    frameCount = 1;
                    currentFrame = 0;
                    return;
                }
                else
                {
                    // GIF
                    gif = new Bitmap[frames];
                    int fps = Win32.GetGifFramerateMs(temp);
                    for (int i = 0; i < frames; i++)
                    {
                        temp.SelectActiveFrame(dim, i);
                        Rectangle bnd = scr.Bounds;
                        Bitmap b = new Bitmap(bnd.Width, bnd.Height, PixelFormat.Format32bppPArgb);
                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.CompositingMode = CompositingMode.SourceCopy;
                            g.CompositingQuality = CompositingQuality.HighSpeed;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.Half;
                            g.InterpolationMode = InterpolationMode.NearestNeighbor;
                            g.DrawImage(temp, bnd);
                        }
                        gif[i] = b;
                    }
                    temp.Dispose();
                    isGif = true;
                    currentFrame = 0;
                    frameCount = frames;
                    hasWallpaper = true;

                    millisDelay = fps;
                    ResetSW();
                }
            }
        }
        /// <summary>
        /// Releases and removes the current wallpaper image, if there is any.
        /// </summary>
        public void ClearWallpaper()
        {
            if(wallpaper != null)
            {
                wallpaper.Dispose();
            }
            wallpaper = null;
            hasWallpaper = false;
        }
        /// <summary>
        /// Returns whether the current wallpaper is a GIF file.
        /// </summary>
        /// <returns></returns>
        public bool WallpaperIsGif()
        {
            return isGif;
        }
        /// <summary>
        /// Steps the gif (if any) forward one frame, or loops back around if end of animation.
        /// </summary>
        public Bitmap StepGifFrame()
        {
            if (!WallpaperIsGif() && !isMP4)
                return wallpaper;
            else if(isMP4)
            {
                if (!(frameRateCounter.ElapsedMilliseconds < frames * millisDelay))
                {
                    frames++;
                    mp4frame++;
                    if (mp4frame >= frameCount)
                        mp4frame = 0;
                    return mp4.ReadVideoFrame();
                } else {

                }
            }

            if (frameRateCounter.ElapsedMilliseconds < frames * millisDelay)
                return GetCurrentFrame();

            frames++;
            currentFrame++;
            if(currentFrame >= frameCount)
                currentFrame = 0;
            return GetCurrentFrame();
        }


        // -----------------------------
        /// <summary>
        /// Applies the currentFrame to the wallpaper object.
        /// Can only be called if wallpaper is a Gif.
        /// </summary>
        private Bitmap GetCurrentFrame()
        {
            return gif[currentFrame];
        }
    }
}
