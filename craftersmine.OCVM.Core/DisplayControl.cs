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
        public int DisplayWidth { get; private set; }
        public int DisplayHeight { get; private set; }
        public SizeF CharSize { get; private set; }
        public Point CursorPosition { get; set; }
        public bool DrawCharacterCells { get; set; } = false;
        public Tier Tier { get; private set; }
        public ScreenBuffer ScreenBuffer { get { return ScreenBufferManager.Instance.GetBuffer(0); } }
        public Size Viewport { get; set; } = new Size(50, 16);

        public DisplayControl()
        {
            //SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            VMEvents.VMReady += VMEvents_VMReady;
            VMEvents.GpuOperationRequested += DisplayControl_OnGpuOperation;
        }

        private void DisplayControl_OnGpuOperation(object sender, GpuOperationEventArgs e)
        {
            if (e.Operation == GpuOperation.SetResolution)
                SetDisplaySize(ScreenBuffer.Width, ScreenBuffer.Height);
            if (e.Operation == GpuOperation.SetViewport) 
            {
                Viewport = e.Viewport;
                SetDisplaySize(Viewport.Width, Viewport.Height);
            }
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

        public void SetViewport(int width, int height)
        {
            Viewport = new Size(width, height);
            SetDisplaySize(width, height);
        }

        private void VMEvents_VMReady(object sender, EventArgs e)
        {
            //ScreenBuffer.CreateScreenBufferInstance();
            ScreenBufferManager.Instance.GetBuffer(0).ScreenBufferChanged += Instance_ScreenBufferChanged;
            ScreenBufferManager.Instance.GetBuffer(0).ScreenBufferCleared += Instance_ScreenBufferCleared;
            Font = Settings.DisplayFont;
        }

        private void Instance_ScreenBufferCleared(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Instance_ScreenBufferChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        public void SetDisplaySize(int width, int height)
        {
            DisplayWidth = width;
            DisplayHeight = height;
            //if (ScreenBufferManager.Instance.GetBuffer(0) == null)
            //{
            //    ScreenBufferManager.Instance.CreateBuffer(0, DisplayWidth, DisplayHeight);
            //}
            //ScreenBufferManager.Instance.GetBuffer(0).Initialize(DisplayWidth, DisplayHeight);
            CharSize = RazorGFX.MeasureString(" ", Font);
            CharSize = new SizeF(8f, 16f);
            //CharSize = RazorGFX.MeasureString(" ", Font, 16, new StringFormat(StringFormatFlags.NoFontFallback | StringFormatFlags.DisplayFormatControl));
            Size measuredSize = new Size((int)CharSize.Width * Viewport.Width, (int)CharSize.Height * Viewport.Height);
            if (InvokeRequired)
                Invoke(new Action(() => {
                    Padding = Padding.Empty;
                    Margin = Padding.Empty;
                    ClientSize = measuredSize;
                }));
            else {
                Padding = Padding.Empty;
                Margin = Padding.Empty;
                ClientSize = measuredSize;
            }
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
                            for (int x = 0; x < Viewport.Width; x++)
                            {
                                for (int y = 0; y < Viewport.Height; y++)
                                {
                                    var chr = ScreenBuffer.Get(x, y);
                                    float xPos = CharSize.Width * (float)x;
                                    float yPos = CharSize.Height * (float)y;
                                    var chrRect = new RectangleF(xPos, yPos, CharSize.Width, CharSize.Height);
                                    var b = new SolidBrush(chr.BackgroundColor);
                                    g.FillRectangle(b, chrRect);
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