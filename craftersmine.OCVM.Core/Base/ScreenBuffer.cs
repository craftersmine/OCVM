using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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

        public void BitBlt(int destinationBuffer, int x, int y, int width, int height, int sourceBuffer, int fromX, int fromY)
        {
            
        }
    }

    public sealed class ScreenBuffer
    {
        public DisplayChar[,] Buffer { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public bool IsChanging { get; private set; }
        public bool IsWaitingToEnd { get; private set; }
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
                Begin(true);
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, ' ', ForegroundColor, BackgroundColor);
            if (!beginCalled)
                End();
        }

        public void Begin(bool waitToEnd)
        {
            IsWaitingToEnd = waitToEnd;
            if (IsChanging == false)
                IsChanging = true;
        }

        public void Set(int x, int y, DisplayChar chr)
        {
            if (IsChanging)
                if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
                {
                    Buffer[x, y] = chr;
                    if (!IsWaitingToEnd)
                        VMEvents.OnScreenBufferUpdate(chr, x, y);
                }
        }

        public void Set(int x, int y, char chr, Color foreground, Color background)
        {
            if (IsChanging)
            {
                if ((x >= 0 && x < Width) && (y >= 0 && y < Height))
                {
                    Buffer[x, y].SetChar(chr, foreground, background);
                    if (!IsWaitingToEnd)
                        VMEvents.OnScreenBufferUpdate(Buffer[x, y], x, y);
                }
            }
        }

        public void Set(int x, int y, char chr)
        {
            Set(x, y, chr, ForegroundColor, BackgroundColor);
        }

        public void End()
        {
            IsChanging = false;
            if (IsWaitingToEnd)
                ScreenBufferChanged?.Invoke(this, EventArgs.Empty);
        }

        public DisplayChar Get(int x, int y)
        {
            if (x >= 0 || x < Width || y >= 0 || y < Height)
                return Buffer[x, y];
            return null;
        }

        public void Copy(int x, int y, int width, int height, int tx, int ty)
        {
            Begin(true);
            if (IsChanging)
            {
                if (width <= 0 || height <= 0) return;
                if (tx == 0 && ty == 0) return;

                DisplayChar[,] copiedData = new DisplayChar[width, height];

                for (int dx = 0; dx < width; dx++)
                    for (int dy = 0; dy < height; dy++)
                    {
                        if (x + dx < 0 || y + dy < 0 || x + dx >= Width || y + dy >= Height)
                            copiedData[dx, dy] = new DisplayChar(' ', ForegroundColor, BackgroundColor);
                        else
                            copiedData[dx, dy] = Get(x + dx, y + dy);
                    }

                for (int w = 0; w < width; w++)
                    for (int h = 0; h < height; h++)
                    {
                        if (x + tx + w >= 0 && x + tx + w < Width && y + ty + h >= 0 && y + ty + h < Height)
                        {
                            Buffer[x + tx + w, y + ty + h] = copiedData[w, h];
                        }
                    }
            }
            End();
        }

        public void Scroll()
        {
            Begin(true);
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
