using craftersmine.OCVM.Core.Base;
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
    public sealed class Display : UserControl
    {
        private Graphics graphics;
        private Tier tier;
        private bool isCursorShown;
        private TimeSpan lastCursorTime;
        private readonly System.Diagnostics.Stopwatch frameTimeCounter = new System.Diagnostics.Stopwatch();
        private Timer timer = new Timer();
        private Size charSize = Size.Empty;
        private bool charSizeCalculated = false;

        public event EventHandler<DisplayRedrawnEventArgs> DisplayRedrawn;
        private DisplayRedrawnEventArgs drea;

        public Tier Tier { get { return tier; } }
        public bool ShowCharactersBounds { get; set; }
        public int DisplayWidth { get; set; }
        public int DisplayHeight { get; set; }
        public bool EnableCursor { get; set; } = true;
        public Point CursorPosition { get; set; }
        public Size CharSize { get; private set; }
        public ScreenBuffer ScreenBuffer { get; private set; }

        public Display()
        {
            drea = new DisplayRedrawnEventArgs();
            timer.Interval = 16;
            timer.Tick += Timer_Tick;
            this.Load += Display_Load;
            CursorPosition = new Point(0, 0);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            PrivateFontCollection pfc = new PrivateFontCollection();
            if (DesignMode)
                Font = new Font("Lucida Console", 12f);
            else
            {
                pfc.AddFontFile(Path.Combine(Application.StartupPath, "unscii-16-full.ttf"));
                Font = new Font(pfc.Families[0], 8f, FontStyle.Regular);
                //Font = new Font("Lucida Console", 8f);
            }
            SetTier(Tier.Base);
            SetColor(BaseColors.Black, BaseColors.White);
            ScreenBuffer = new ScreenBuffer(DisplayWidth, DisplayHeight);
            ScreenBuffer.ScreenBufferChanged += ScreenBuffer_ScreenBufferChanged;
        }

        private void ScreenBuffer_ScreenBufferChanged(object sender, EventArgs e)
        {
            DrawScreenBuffer();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (lastCursorTime.TotalSeconds >= .5f)
            {
                isCursorShown = !isCursorShown;
                lastCursorTime = TimeSpan.Zero;
            }
        }

        private void Display_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;
            timer.Start();
        }

        public void SetColor(Color background, Color foreground)
        {
            BackColor = background;
            ForeColor = foreground;
        }

        public void DrawScreenBuffer()
        {
            for (int x = 0; x < DisplayWidth; x++)
                for (int y = 0; y < DisplayHeight; y++)
                {
                    var chr = ScreenBuffer.Instance.Get(x, y);
                    DrawChar(x, y, chr);
                }
        }

        public void DrawChar(int posX, int posY, DisplayChar chr)
        {
            if (!charSizeCalculated)
            {
                charSize = TextRenderer.MeasureText(graphics, chr.ToString(), Font, Size.Empty, TextFormatFlags.NoPadding);
                charSizeCalculated = true;
            }
            Point charPos = new Point(charSize.Width * posX, charSize.Height * posY);
            if (ShowCharactersBounds)
                graphics.DrawRectangle(new Pen(Color.Red, 2), new Rectangle(charPos, charSize));
            graphics.FillRectangle(new SolidBrush(chr.BackgroundColor), charPos.X, charPos.Y, charSize.Width, charSize.Height);
            TextRenderer.DrawText(graphics, chr.ToString(), Font, charPos, chr.ForegroundColor, TextFormatFlags.NoPadding);
        }

        public void PlaceChar(int posX, int posY, char chr, Color foreground, Color background)
        {
            //if (screenBuffer[posX, posY].CheckInvalidation(chr, foreground, background))
            ScreenBuffer.Instance.Set(posX, posY, chr, foreground, background);
        }

        public void PlaceString(int posX, int posY, string str, Color foreground, Color background)
        {
            string remainingBuffer = "";
            if (str.Length > DisplayWidth)
            {
                remainingBuffer = str.Substring(DisplayWidth);
                str = str.Substring(0, DisplayWidth);
            }
            for (int i = 0; i < str.Length; i++)
            {
                try
                {
                    ScreenBuffer.Instance.Set(posX + i, posY, str[i], foreground, background);
                }
                catch { }
            }
            if (remainingBuffer != "" && remainingBuffer != null)
            {
                SetCursorPosition(0, posY + 1);
                craftersmine.OCVM.Core.Base.LuaApi.Root.print(remainingBuffer);  // TODO: Fix text overlap after new line
            }
        }

        public void SetCursorPosition(int posX, int posY)
        {
            CursorPosition = new Point(posX, posY);
        }

        public void ScrollScreenBuffer()
        {
            VM.RunningVM.ScreenBuffer.Scroll();
        }

        public void SetTier(Tier tier)
        {
            switch (tier)
            {
                case Tier.Base:
                    SetDisplaySize(50, 16);
                    this.tier = Tier.Base;
                    break;
                case Tier.Medium:
                    SetDisplaySize(80, 25);
                    this.tier = Tier.Medium;
                    break;
                case Tier.Advanced:
                    SetDisplaySize(160, 50);
                    this.tier = Tier.Advanced;
                    break;
            }
        }

        public void SetDisplaySize(int width, int height)
        {
            DisplayWidth = width;
            DisplayHeight = height;
            if (ScreenBuffer.Instance != null)
            {
                ScreenBuffer.Instance.Initialize(DisplayWidth, DisplayHeight);
                string[] lines = new string[height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        lines[y] += ' ';
                    }
                }
                Size measuredSize = TextRenderer.MeasureText(string.Join("\r\n", lines), Font);
                ClientSize = measuredSize;
                Redraw();
            }
        }

        public void ClearScreenBuffer()
        {
            ScreenBuffer.Instance.ClearColor = BackColor;
            ScreenBuffer.Instance.Clear();
            Redraw();
        }

        public void SetScreenBufferData(int x, int y, DisplayChar chr)
        {
            ScreenBuffer.Instance.Set(x, y, chr);
        }

        public DisplayChar GetScreenBufferData(int x, int y)
        {
            return ScreenBuffer.Instance.Get(x, y);
        }

        public void Redraw()
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            graphics = e.Graphics;
            frameTimeCounter.Start();
            if (DesignMode)
            {
                PlaceString(0, 0, "Display is running in design mode!", BaseColors.Red, BackColor);
                DrawScreenBuffer();
                return;
            }

            //if (isCursorShown)
            //    graphics.FillRectangle(new SolidBrush(ForeColor), CursorPosition.X, CursorPosition.Y, CharSize.Width, CharSize.Height);
            //else
            //    graphics.FillRectangle(new SolidBrush(BackColor), CursorPosition.X, CursorPosition.Y, CharSize.Width, CharSize.Height);

            lastCursorTime += frameTimeCounter.Elapsed;
            drea.DrawTime = frameTimeCounter.Elapsed;

            DisplayRedrawn?.Invoke(this, drea);
            frameTimeCounter.Restart();
        }
    }

    public class DisplayRedrawnEventArgs : EventArgs
    {
        public TimeSpan DrawTime { get; set; }
    }
}
