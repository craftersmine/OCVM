using craftersmine.OCVM.Core.Base;
using RazorGDI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace craftersmine.OCVM.Core
{
    public sealed class DisplayControl : RazorPainterControl
    {
        private PrivateFontCollection pfc = new PrivateFontCollection();

        public int DisplayWidth { get; private set; }
        public int DisplayHeight { get; private set; }
        public SizeF CharSize { get; private set; }
        public Point CursorPosition { get; set; }
        public bool DrawCharacterCells { get; set; } = false;

        public DisplayControl()
        {
            //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            VMEvents.VMReady += VMEvents_VMReady;
        }
        public void SetTier(Tier tier)
        {
            switch (tier)
            {
                case Tier.Base:
                    SetDisplaySize(50, 16);
                    //this.tier = Tier.Base;
                    break;
                case Tier.Medium:
                    SetDisplaySize(80, 25);
                    //this.tier = Tier.Medium;
                    break;
                case Tier.Advanced:
                    SetDisplaySize(160, 50);
                    //this.tier = Tier.Advanced;
                    break;
            }
        }

        private void VMEvents_VMReady(object sender, EventArgs e)
        {
            //ScreenBuffer.CreateScreenBufferInstance();
            ScreenBuffer.Instance.ScreenBufferChanged += Instance_ScreenBufferChanged;
            ScreenBuffer.Instance.ScreenBufferCleared += Instance_ScreenBufferCleared;
        }

        private void Instance_ScreenBufferCleared(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Instance_ScreenBufferChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        public void InitializeFont()
        {
            pfc.AddFontFile(Path.Combine(Application.StartupPath, "unscii-16-full.ttf"));
            Font = new Font(pfc.Families[0], 16f, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public void SetDisplaySize(int width, int height)
        {
            InitializeFont();
            DisplayWidth = width;
            DisplayHeight = height;
            if (ScreenBuffer.Instance == null)
            {
                ScreenBuffer.CreateScreenBufferInstance();
            }
            ScreenBuffer.Instance.Initialize(DisplayWidth, DisplayHeight);
            CharSize = RazorGFX.MeasureString(" ", Font);
            CharSize = new SizeF(8f, 16f);
            //CharSize = RazorGFX.MeasureString(" ", Font, 16, new StringFormat(StringFormatFlags.NoFontFallback | StringFormatFlags.DisplayFormatControl));
            Size measuredSize = new Size((int)CharSize.Width * ScreenBuffer.Instance.Width, (int)CharSize.Height * ScreenBuffer.Instance.Height);
            ClientSize = measuredSize;
            Redraw();
        }

        public void Redraw()
        {
            lock (RazorLock)
            {
                using (var bmp = new Bitmap(ClientSize.Width, ClientSize.Height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.RenderingOrigin = new Point(0, 0);
                        for (int x = 0; x < ScreenBuffer.Instance.Width; x++)
                        {
                            for (int y = 0; y < ScreenBuffer.Instance.Height; y++)
                            {
                                var chr = ScreenBuffer.Instance.Get(x, y);
                                float xPos = CharSize.Width * (float)x;
                                float yPos = CharSize.Height * (float)y;
                                var chrRect = new RectangleF(xPos, yPos, CharSize.Width, CharSize.Height);
                                g.FillRectangle(new SolidBrush(chr.BackgroundColor), chrRect);
                                g.DrawString(chr.ToString(), Font, new SolidBrush(chr.ForegroundColor), chrRect.X - 3, chrRect.Y);
                                if (DrawCharacterCells)
                                    g.DrawRectangle(Pens.Red, chrRect.X, chrRect.Y, chrRect.Width, chrRect.Height);
                            }
                        }
                    }
                    RazorGFX.DrawImage(bmp, Point.Empty);
                }
                RazorPaint();
            }
        }
    }
}