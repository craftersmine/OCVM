using craftersmine.OCVM.Core.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private DisplayChar[,] screenBuffer;
        private bool isCursorShown;
        private TimeSpan lastCursorTime;
        private readonly System.Diagnostics.Stopwatch frameTimeCounter = new System.Diagnostics.Stopwatch();

        public event EventHandler<DisplayRedrawnEventArgs> DisplayRedrawn;
        private DisplayRedrawnEventArgs drea;

        public Tier Tier { get { return tier; } }
        public bool ShowCharactersBounds { get; set; }
        public int DisplayWidth { get; set; }
        public int DisplayHeight { get; set; }
        public bool EnableCursor { get; set; } = true;
        public Point CursorPosition { get; set; }
        public Size CharSize { get; private set; }

        public Display()
        {
            drea = new DisplayRedrawnEventArgs();
            CursorPosition = new Point(0, 0);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;
            Font = new Font("Lucida Console", 12f);
            SetTier(Tier.Base);

            SetColor(BaseColors.Black, BaseColors.White);
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
                    PlaceChar(x, y, screenBuffer[x, y].Character, screenBuffer[x, y].ForegroundColor, screenBuffer[x, y].BackgroundColor);
                }
        }

        public void PlaceChar(int posX, int posY, char chr, Color foreground, Color background)
        {
            Size charSize = TextRenderer.MeasureText(graphics, chr.ToString(), Font, Size.Empty, TextFormatFlags.NoPadding);
            Point charPos = new Point(charSize.Width * posX, charSize.Height * posY);
            if (ShowCharactersBounds)
                graphics.DrawRectangle(new Pen(Color.Red, 2), new Rectangle(charPos, charSize));
            graphics.FillRectangle(new SolidBrush(background), charPos.X, charPos.Y, charSize.Width, charSize.Height);
            TextRenderer.DrawText(graphics, chr.ToString(), Font, charPos, foreground, TextFormatFlags.NoPadding);
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
                screenBuffer[posX + i, posY].Character = str[i];
                screenBuffer[posX + i, posY].ForegroundColor = foreground;
                screenBuffer[posX + i, posY].BackgroundColor = background;
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
            for (int y = 1; y < DisplayHeight; y++)
            {
                for (int x = 0; x < DisplayWidth; x++)
                {
                    screenBuffer[x, y - 1] = screenBuffer[x, y];
                    if (y == DisplayHeight - 1)
                        screenBuffer[x, y] = new DisplayChar(' ', ForeColor, BackColor);
                }
            }
            Redraw();
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
            screenBuffer = new DisplayChar[width, height];
            string[] lines = new string[height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    screenBuffer[x, y] = new DisplayChar(' ', ForeColor, BackColor);
                    lines[y] += screenBuffer[x, y].Character;
                }
            }
            Size measuredSize = TextRenderer.MeasureText(string.Join("\r\n", lines), Font);
            ClientSize = measuredSize;
            Redraw();
        }

        public void ClearScreenBuffer()
        {
            for (int y = 0; y < DisplayHeight; y++)
                for (int x = 0; x < DisplayWidth; x++)
                    screenBuffer[x, y] = new DisplayChar(' ', ForeColor, BackColor);
            Redraw();
        }

        public void SetScreenBufferData(int x, int y, DisplayChar chr)
        {
            screenBuffer[x, y] = chr;
        }

        public DisplayChar GetScreenBufferData(int x, int y)
        {
            return screenBuffer[x, y];
        }

        public void Redraw()
        {
            //SuspendLayout();
            Invalidate();
            //ResumeLayout();
            //PerformLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            graphics = e.Graphics;
            CharSize = TextRenderer.MeasureText(graphics, ' '.ToString(), Font, Size.Empty, TextFormatFlags.NoPadding);
            frameTimeCounter.Start();
            if (DesignMode)
            {
                PlaceString(0, 0, "Display is running in design mode!", BaseColors.Red, BackColor);
                DrawScreenBuffer();
                return;
            }

            if (lastCursorTime.TotalSeconds >= .5f)
            {
                isCursorShown = !isCursorShown;
                lastCursorTime = TimeSpan.Zero;
            }
            if (isCursorShown)
                graphics.FillRectangle(new SolidBrush(ForeColor), CursorPosition.X, CursorPosition.Y, CharSize.Width, CharSize.Height);
            else
                graphics.FillRectangle(new SolidBrush(BackColor), CursorPosition.X, CursorPosition.Y, CharSize.Width, CharSize.Height);

            DrawScreenBuffer();
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
