using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Base.LuaApi;
using System;
using System.Collections.Generic;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "gpu")]
    public sealed class GPU : BaseComponent
    {
        public int AvailableVram { get; set; }
        public int TotalVram { get; set; }
        public string ScreenAddress { get; set; } = null;
        public Screen ScreenInstance { get; set; } = null;

        public GPU() : base()
        {
            DeviceInfo.Class = DeviceClass.Display;
            DeviceInfo.Capacity = "0";
            DeviceInfo.Vendor = DeviceInfo.DefaultVendor;
            DeviceInfo.Description = "Virtual Graphics Adapter";
            DeviceInfo.Product = "craftersmine Virtual Graphics Adapter";
            DeviceInfo.Width = "8/24/32";
            DeviceInfo.Clock = "2000/2000/2000/2000/2000/2000";
            if (ScreenBufferManager.Instance.GetBuffer(0) == null)
            {
                ScreenBufferManager.Instance.CreateBuffer(0, Settings.GpuMaxWidth, Settings.GpuMaxHeight);
            }
            ScreenBufferManager.Instance.GetBuffer(0).Initialize(Settings.GpuMaxWidth, Settings.GpuMaxHeight);
        }

        [LuaCallback(IsDirect = true)]
        public object[] get(int x, int y)
        {
            var data = new object[5];
            var v = ScreenBufferManager.Instance.GetBuffer(0).Get(x, y);
            data[0] = v.Character.ToString();
            data[1] = v.ForegroundColor.ToArgb();
            data[2] = v.BackgroundColor.ToArgb();
            data[3] = null;
            data[4] = null;
            return data;
        }

        [LuaCallback(IsDirect = true)]
        public bool set(int x, int y, string value, bool vertical)
        {
            var buffer = ScreenBufferManager.Instance.GetBuffer(0);
            if (buffer != null)
            {
                buffer.Begin();
                for (int i = 0; i < value.Length; i++)
                {
                    buffer.Set(x + i, y, value[i]);
                }
                buffer.End();
                return true;
            }
            else return false;
        }

        [LuaCallback(IsDirect = true)]
        public bool set(int x, int y, string value)
        {
            return set(x, y, value, false);
        }

        [LuaCallback(IsDirect = true)]
        public bool copy(int x, int y, int width, int height, int tx, int ty)
        {
            var buffer = ScreenBufferManager.Instance.GetBuffer(0);
            if (buffer != null)
            {
                buffer.Begin();
                buffer.Copy(x, y, width, height, tx, ty);
                buffer.End();
                return true;
            }
            else return false;
        }

        [LuaCallback(IsDirect = true)]
        public object[] fill(int x, int y, int width, int height, string chr)
        {
            if (chr.Length > 1)
                return new object[] { false, OCErrors.InvalidFillValue };
            var buffer = ScreenBufferManager.Instance.GetBuffer(0);
            if (buffer != null)
            {
                var c = chr[0];
                buffer.Begin();
                for (int x1 = 0; x1 < width; x1++)
                    for (int y1 = 0; y1 < height; y1++)
                        buffer.Set(x + x1, y + y1, c);
                buffer.End();
                return new object[] { true };
            }
            else return new object[] { false };
        }

        [LuaCallback(IsDirect = false)]
        public object[] bind(string address, bool reset = true)
        {
            if (address == null)
                return new object[] { false, OCErrors.InvalidAddress };

            var scr = VM.RunningVM.DeviceBus.GetDevice<Screen>(address);
            if (scr != null || scr.GetDeviceTypeName() == "screen")
            {
                if (reset)
                    scr.Reset();
                ScreenAddress = scr.Address;
                ScreenInstance = scr;
                return new object[] { true };
            }
            else
                return new object[] { false, OCErrors.NotAScreen };
        }

        [LuaCallback(IsDirect = false)]
        public object[] bind(string address)
        {
            return bind(address, true);
        }

        [LuaCallback(IsDirect = true)]
        public string getScreen()
        {
            return ScreenAddress;
        }

        [LuaCallback(IsDirect = true)]
        public object[] getBackground()
        {
            return new object[] { ScreenBufferManager.Instance.GetBuffer(0).BackgroundColor.ToArgb(), false };
        }

        [LuaCallback(IsDirect = true)]
        public object[] setBackground()
        {
            return null;
        }
    }
}
