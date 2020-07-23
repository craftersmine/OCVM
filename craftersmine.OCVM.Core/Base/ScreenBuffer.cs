using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base
{
    public sealed class ScreenBufferManager
    {
        private Dictionary<int, ScreenBuffer> buffers = new Dictionary<int, ScreenBuffer>();

        public static ScreenBufferManager Instance { get; private set; }

        public int AllocatedBuffersCount { get { return buffers.Count; } }

        public ScreenBufferManager()
        {
            Instance = this;
        }

        public void CreateBuffer(int index, int width, int height)
        {
            Logger.Instance.Log(LogEntryType.Info, "[SCRBUFMGR] Creating new screen buffer instance (" + width + "x" + height + ") with index " + index + "...");
            ScreenBuffer buffer = new ScreenBuffer(width, height);
            Logger.Instance.Log(LogEntryType.Info, "[SCRBUFMGR] Created new screen buffer instance (" + width + "x" + height + ") with index " + index);
            buffers.Add(index, buffer);
        }

        public bool FreeBuffer(int index)
        {
            if (buffers.ContainsKey(index))
            {
                if (buffers[index].IsChanging)
                {
                    Logger.Instance.Log(LogEntryType.Info, " [SCRBUFMGR] Ending buffer " + index + " operations before freeing...");
                    buffers[index].End();
                }
                Logger.Instance.Log(LogEntryType.Info, " [SCRBUFMGR] Freed buffer " + index);
                buffers.Remove(index);
                return true;
            }
            else return false;
        }

        public int[] GetAllocatedBuffers()
        {
            return buffers.Keys.ToArray();
        }

        public void FreeAllBuffers()
        {
            foreach (var buffer in buffers)
            {
                if (buffer.Key != 0)
                    FreeBuffer(buffer.Key);
            }
        }

        public ScreenBuffer GetBuffer(int index)
        {
            if (buffers.ContainsKey(index))
                return buffers[index];
            else return null;
        }
    }

    public sealed class ScreenBuffer
    {
        public DisplayChar[,] Buffer { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public bool IsChanging { get; set; }
        public static ScreenBuffer Instance { get; private set; }

        public event EventHandler ScreenBufferChanged;
        public event EventHandler ScreenBufferCleared;
        public event EventHandler ScreenBufferInitialized;

        private ScreenBuffer()
        {
            Instance = this;
        }

        public ScreenBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            Instance = this;
            Initialize(Width, Height);
        }

        public static void CreateScreenBufferInstance()
        {
            new ScreenBuffer();
        }

        public void Initialize(int width, int height)
        {
            Logger.Instance.Log(LogEntryType.Info, "Initializing new screen buffer instance (" + width + "x" + height + ")...");
            Width = width;
            Height = height;
            Buffer = new DisplayChar[Width, Height];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Buffer[x, y] = new DisplayChar(' ', BaseColors.White, BaseColors.Black);
                }
            }
            ScreenBufferInitialized?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            bool beginCalled = IsChanging;
            if (!beginCalled)
                Begin();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, ' ', ForegroundColor, BackgroundColor);
            if (!beginCalled)
                End();
        }

        public void Begin()
        {
            if (IsChanging == false)
                IsChanging = true;
        }

        public void Set(int x, int y, DisplayChar chr)
        {
            if (IsChanging)
                Buffer[x, y] = chr;
        }

        public void Set(int x, int y, char chr, Color foreground, Color background)
        {
            if (IsChanging)
            {
                if ((x >= 0 || x <= Width) && (y >= 0 || y <= Height))
                    Buffer[x, y].SetChar(chr, foreground, background);
            }
        }

        public void Set(int x, int y, char chr)
        {
            Set(x, y, chr, ForegroundColor, BackgroundColor);
        }

        public void End()
        {
            IsChanging = false;
            ScreenBufferChanged?.Invoke(this, EventArgs.Empty);
        }

        public DisplayChar Get(int x, int y)
        {
            return Buffer[x, y];
        }

        public void Copy(int x, int y, int width, int height, int tx, int ty)
        {
            if (IsChanging)
            {
                for (int w = 0; w < width; w++)
                    for (int h = 0; h < height; h++)
                    {
                        if ((w >= 0 || w < Width) && (h >= height || h < Height))
                        {
                            if ((tx + x >= 0 || tx + x < Width) && (ty + y >= 0 || ty + y < Height))
                                Buffer[tx + x + w, ty + y + h] = Buffer[x + w, y + h];
                        }
                    }
            }
        }

        public void Scroll()
        {
            Begin();
            for (int y = 1; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Buffer[x, y - 1] = Buffer[x, y];
                    if (y == Height - 1)
                        Buffer[x, y] = new DisplayChar(' ', Buffer[x, y - 1].ForegroundColor, Buffer[x, y - 1].BackgroundColor);
                }
            }
            End();
        }

        public override string ToString()
        {
            string str = "";
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    str += Buffer[x, y].Character;
                }
                str += "\r\n";
            }
            return str;
        }
    }
}
