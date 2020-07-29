using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Base.LuaApi;
using craftersmine.OCVM.Core.Exceptions;
using NLua;
using System.Diagnostics;
using System.Linq;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "gpu")]
    public sealed class GPU : BaseComponent
    {
        public int AvailableVram { get; set; }
        public int TotalVram { get; set; }
        public string ScreenAddress { get; set; } = null;
        public Screen ScreenInstance { get; set; } = null;
        public int ActiveBufferIndex { get; set; } = 0;

        public GPU() : base()
        {
            DeviceInfo.Class = DeviceClass.Display;
            DeviceInfo.Capacity = "0";
            DeviceInfo.Vendor = DeviceInfo.DefaultVendor;
            DeviceInfo.Description = "Virtual Graphics Adapter";
            DeviceInfo.Product = "craftersmine Virtual Graphics Adapter";
            DeviceInfo.Width = "1/4/8/24/32";
            DeviceInfo.Clock = "2000/2000/2000/2000/2000/2000";
            if (ScreenBufferManager.Instance.GetBuffer(0) == null)
            {
                ScreenBufferManager.Instance.CreateBuffer(0, Settings.GpuMaxWidth, Settings.GpuMaxHeight);
            }
            ScreenBufferManager.Instance.GetBuffer(0).Initialize(Settings.GpuMaxWidth, Settings.GpuMaxHeight);
        }

        [LuaCallback(IsDirect = true)]
        public object[] get(long x, long y)
        {
            var data = new object[5];
            var v = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex).Get((int)x, (int)y);
            data[0] = v.Character.ToString();
            data[1] = v.ForegroundColor.ToArgb();
            data[2] = v.BackgroundColor.ToArgb();
            data[3] = null;
            data[4] = null;
            return data;
        }

        [LuaCallback(IsDirect = true)]
        public bool set(long x, long y, string value, bool vertical)
        {
            var buffer = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex);
            x -= 1;
            y -= 1;
            if (buffer != null)
            {
                buffer.Begin();
                if (!vertical)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        buffer.Set((int)x + i, (int)y, value[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        buffer.Set((int)x, (int)y + i, value[i]);
                    }
                }
                buffer.End();
                return true;
            }
            else return false;
        }

        [LuaCallback(IsDirect = true)]
        public bool set(long x, long y, string value)
        {
            return set(x, y, value, false);
        }

        [LuaCallback(IsDirect = true)]
        public bool copy(long x, long y, long width, long height, long tx, long ty)
        {
            var buffer = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex);
            x -= 1;
            y -= 1;
            width -= 1;
            height -= 1;
            if (buffer != null)
            {
                buffer.Begin();
                buffer.Copy((int)x, (int)y, (int)width, (int)height, (int)tx, (int)ty);
                buffer.End();
                return true;
            }
            else return false;
        }

        [LuaCallback(IsDirect = true)]
        public object[] fill(long x, long y, long width, long height, string chr)
        {
            if (chr.Length > 1)
                throw new OpenComputersException(OCErrors.InvalidFillValue);
            var buffer = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex);
            x -= 1;
            y -= 1;
            width -= 1;
            height -= 1;
            if (buffer != null)
            {
                var c = chr[0];
                buffer.Begin();
                for (int x1 = 0; x1 < width; x1++)
                    for (int y1 = 0; y1 < height; y1++)
                        buffer.Set((int)x + x1, (int)y + y1, c);
                buffer.End();
                return new object[] { true };
            }
            else return new object[] { false };
        }

        [LuaCallback(IsDirect = false)]
        public bool bind(string address, bool reset = true)
        {
            if (address == null)
                throw new OpenComputersException(OCErrors.InvalidAddress);

            var scr = VM.RunningVM.DeviceBus.GetDevice<Screen>(address);
            if (scr != null || scr.GetDeviceTypeName() == "screen")
            {
                if (reset)
                    scr.Reset();
                ScreenAddress = scr.Address;
                ScreenInstance = scr;
                VM.RunningVM.Display.SetViewport(Settings.GpuMaxWidth, Settings.GpuMaxHeight);
                VM.RunningVM.Display.SetDisplaySize(Settings.GpuMaxWidth, Settings.GpuMaxHeight);
                return true;
            }
            else
                throw new OpenComputersException(OCErrors.NotAScreen);
        }

        [LuaCallback(IsDirect = true)]
        public string getScreen()
        {
            return ScreenAddress;
        }

        [LuaCallback(IsDirect = true)]
        public object[] getBackground()
        {
            return new object[] { ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex).BackgroundColor.ToArgb(), false };
        }

        [LuaCallback(IsDirect = true)]
        public object[] setBackground(long color, bool isPaletteIndex)
        {
            var old = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex).BackgroundColor;
            var col = EightBitColorPalette.ExtractColorFromRgb((int)color);
            ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex).BackgroundColor = col;
            return new object[] { old.ToArgb(), false };
        }

        [LuaCallback(IsDirect = true)]
        public object[] getForeground()
        {
            return new object[] { ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex).ForegroundColor.ToArgb(), false };
        }

        [LuaCallback(IsDirect = true)]
        public object[] setForeground(long color, bool isPaletteIndex)
        {
            var old = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex).ForegroundColor;
            var col = EightBitColorPalette.ExtractColorFromRgb((int)color);
            ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex).ForegroundColor = col;
            return new object[] { old.ToArgb(), false };
        }

        [LuaCallback(IsDirect = true)]
        public object getPaletteColor(long index)
        {
            var val = EightBitColorPalette.GetColor((int)index);
            if (val == -1)
                return 0x0;
            else return OCErrors.InvalidPaletteIndex;
        }

        [LuaCallback(IsDirect = true)]
        public object setPaletteColor(long index, long color)
        {
            if (index >= 0 || index < 256)
            {
                EightBitColorPalette.SetColor((int)index, (int)color);
                return EightBitColorPalette.GetColor((int)index);
            }
            else
            {
                return OCErrors.InvalidPaletteIndex;
            }
        }

        [LuaCallback(IsDirect = true)]
        public int maxDepth()
        {
            if (Settings.CapScreenDepth)
                return 8;
            return 32;
        }

        [LuaCallback(IsDirect = true)]
        public int getDepth()
        {
            if (ScreenInstance == null)
                return 1;
            if (Settings.CapScreenDepth)
                if (ScreenInstance.Depth > 8)
                    return 8;
            return ScreenInstance.Depth;
        }

        [LuaCallback(IsDirect = true)]
        public bool setDepth(long depth)
        {
            switch (depth)
            {
                case 1:
                    ScreenInstance.Depth = 1;
                    return true;
                case 4:
                    ScreenInstance.Depth = 4;
                    return true;
                case 8:
                    ScreenInstance.Depth = 8;
                    return true;
                case 16:
                    ScreenInstance.Depth = 16;
                    return true;
                case 24:
                    ScreenInstance.Depth = 24;
                    return true;
                case 32:
                    ScreenInstance.Depth = 32;
                    return true;
            }
            return false;
        }

        [LuaCallback(IsDirect = true)]
        public object[] maxResolution()
        {
            var buff = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex);
            return new object[] { Settings.GpuMaxWidth, Settings.GpuMaxHeight };
        }

        [LuaCallback(IsDirect = true)]
        public object[] getResolution()
        {
            var buff = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex);
            return new object[] { buff.Width, buff.Height };
        }

        [LuaCallback(IsDirect = true)]
        public bool setResolution(long width, long height)
        {
            var buff = ScreenBufferManager.Instance.GetBuffer(ActiveBufferIndex);
            if (buff.Width == width || buff.Height == height)
                return false;
            else if (width > Settings.GpuMaxWidth || height > Settings.GpuMaxHeight)
                throw new OpenComputersException();
            else
            {
                buff.End();
                buff.Initialize((int)width, (int)height);
                return true;
            }
        }

        [LuaCallback(IsDirect = true)]
        public object[] getViewport()
        {
            return new object[] { VM.RunningVM.Display.Viewport.Width, VM.RunningVM.Display.Viewport.Height };
        }

        [LuaCallback(IsDirect = true)]
        public bool setViewport(long width, long height)
        {
            int wL = VM.RunningVM.Display.DisplayWidth;
            int hL = VM.RunningVM.Display.DisplayHeight;
            if (wL == width || hL == height)
                return false;
            else
            {
                VM.RunningVM.Display.SetDisplaySize((int)width, (int)height);
                return true;
            }
        }

        [LuaCallback(IsDirect = true)]
        public int getActiveBuffer()
        {
            return ActiveBufferIndex;
        }

        [LuaCallback(IsDirect = true)]
        public object[] setActiveBuffer(int newIndex)
        {
            int prevIdx = ActiveBufferIndex;
            if (ScreenBufferManager.Instance.GetAllocatedBuffers().Contains(newIndex))
            {
                ActiveBufferIndex = newIndex;
                return new object[] { prevIdx };
            }
            else return new object[] { null, OCErrors.InvalidBufferIndex };
        }

        [LuaCallback(IsDirect = true)]
        public LuaTable buffers()
        {
            var table = VM.RunningVM.ExecModule.CreateTable();
            var buffs = ScreenBufferManager.Instance.GetAllocatedBuffers();
            int i = 1;
            foreach (var buff in buffs)
            {
                if (buff == 0)
                    continue;
                else
                {
                    table[i] = buff;
                    i++;
                }
            }
            return table;
        }

        [LuaCallback(IsDirect = true)]
        public int allocateBuffer(int width = int.MinValue, int height = int.MinValue)
        {
            if (width == int.MinValue)
                width = Settings.GpuMaxWidth;
            if (height == int.MinValue)
                height = Settings.GpuMaxHeight;

            int newBuffIdx = ScreenBufferManager.Instance.AllocatedBuffersCount + 1;
            ScreenBufferManager.Instance.CreateBuffer(newBuffIdx, width, height);
            return newBuffIdx;
        }

        [LuaCallback(IsDirect = true)]
        public bool freeBuffer(int index = int.MinValue)
        {
            if (index == int.MinValue)
                index = ActiveBufferIndex;

            ActiveBufferIndex = 0;
            return ScreenBufferManager.Instance.FreeBuffer(index);
        }

        [LuaCallback(IsDirect = true)]
        public void freeAllBuffers()
        {
            ActiveBufferIndex = 0;
            ScreenBufferManager.Instance.FreeAllBuffers();
        }
    }
}
