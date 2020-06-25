﻿using System;
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
            ScreenBuffer buffer = new ScreenBuffer(width, height);
            buffers.Add(index, buffer);
        }

        public bool FreeBuffer(int index)
        {
            if (buffers.ContainsKey(index))
            {
                if (buffers[index].IsChanging)
                {
                    buffers[index].End();
                }
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
        public Color ClearColor { get; set; }
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
            Begin();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Set(x, y, ' ', BaseColors.White, ClearColor);
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
                Buffer[x, y].SetChar(chr, foreground, background);
            }
        }

        public DisplayChar[,] End()
        {
            ScreenBufferChanged?.Invoke(this, EventArgs.Empty);
            return Buffer;
        }

        public DisplayChar Get(int x, int y)
        {
            return Buffer[x, y];
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
