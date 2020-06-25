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
        public Tier Tier { get; private set; }
        public ScreenBuffer ScreenBuffer { get { return ScreenBufferManager.Instance.GetBuffer(0); } }

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
                    this.Tier = Tier.Base;
                    break;
                case Tier.Medium:
                    SetDisplaySize(80, 25);
                    this.Tier = Tier.Medium;
                    break;
                case Tier.Advanced:
                    SetDisplaySize(160, 50);
                    this.Tier = Tier.Advanced;
                    break;
            }
        }

        private void VMEvents_VMReady(object sender, EventArgs e)
        {
            //ScreenBuffer.CreateScreenBufferInstance();
            ScreenBufferManager.Instance.GetBuffer(0).ScreenBufferChanged += Instance_ScreenBufferChanged;
            ScreenBufferManager.Instance.GetBuffer(0).ScreenBufferCleared += Instance_ScreenBufferCleared;
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
            if (ScreenBufferManager.Instance.GetBuffer(0) == null)
            {
                ScreenBufferManager.Instance.CreateBuffer(0, DisplayWidth, DisplayHeight);
            }
            ScreenBufferManager.Instance.GetBuffer(0).Initialize(DisplayWidth, DisplayHeight);
            CharSize = RazorGFX.MeasureString(" ", Font);
            CharSize = new SizeF(8f, 16f);
            //CharSize = RazorGFX.MeasureString(" ", Font, 16, new StringFormat(StringFormatFlags.NoFontFallback | StringFormatFlags.DisplayFormatControl));
            Size measuredSize = new Size((int)CharSize.Width * ScreenBuffer.Width, (int)CharSize.Height * ScreenBuffer.Height);
            Padding = Padding.Empty;
            Margin = Padding.Empty;
            ClientSize = measuredSize;
            Redraw();
        }

        public void Redraw()
        {
            if (ScreenBuffer != null)
            {
                lock (RazorLock)
                {
                    using (var bmp = new Bitmap(ClientSize.Width, ClientSize.Height))
                    {
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.RenderingOrigin = new Point(0, 0);
                            for (int x = 0; x < ScreenBuffer.Width; x++)
                            {
                                for (int y = 0; y < ScreenBuffer.Height; y++)
                                {
                                    var chr = ScreenBuffer.Get(x, y);
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
}