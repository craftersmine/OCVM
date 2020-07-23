using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "screen")]
    public sealed class Screen : BaseComponent
    {
        internal bool IsOn { get; set; }
        internal bool IsPresiseModeEnabled { get; set; }
        internal bool IsTouchModeInverted { get; set; }
        internal int Depth { get; set; } = 32;

        public Screen(Tier tier) : base()
        {

        }

        public void Reset()
        {
            VM.RunningVM.Display.SetDisplaySize(160, 50);
            var buff = ScreenBufferManager.Instance.GetBuffer(0);
            buff.BackgroundColor = BaseColors.Black;
            buff.ForegroundColor = BaseColors.White;
            ScreenBufferManager.Instance.FreeAllBuffers();
            Logger.Instance.Log(LogEntryType.Info, "Screen " + Address + " has been reset! (" + ScreenBufferManager.Instance.GetBuffer(0).Width + "x" + ScreenBufferManager.Instance.GetBuffer(0).Height + "x" + Depth + ")");
        }

        [LuaCallback(IsDirect = true)]
        public bool isOn()
        {
            return IsOn;
        }

        [LuaCallback(IsDirect = false)]
        public bool turnOn()
        {
            IsOn = true;
            VMEvents.OnScreenStateChanged(this);
            return IsOn;
        }

        [LuaCallback(IsDirect = false)]
        public bool turnOff()
        {
            IsOn = false;
            VMEvents.OnScreenStateChanged(this);
            return IsOn;
        }

        [LuaCallback(IsDirect = true)]
        public object[] getAspectRatio()
        {
            var buff = ScreenBufferManager.Instance.GetBuffer(0);
            if (buff != null)
                return new object[] { buff.Width, buff.Height };
            else return new object[] { 1, 1 };
        }

        [LuaCallback(IsDirect = false)]
        public LuaTable getKeyboards()
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();
            var kbds = VM.RunningVM.DeviceBus.GetDevicesByType("keyboard", false);
            for (int i = 1; i <= kbds.Length; i++)
            {
                table[i] = kbds[i - 1];
            }
            return table;
        }

        [LuaCallback(IsDirect = true)]
        public bool isPresise()
        {
            return IsPresiseModeEnabled;
        }

        [LuaCallback(IsDirect = false)]
        public bool setPresise(bool enabled)
        {
            bool old = IsPresiseModeEnabled;
            IsPresiseModeEnabled = enabled;
            return old;
        }

        [LuaCallback(IsDirect = true)]
        public bool isTouchModeInverted()
        {
            return IsTouchModeInverted;
        }

        public bool setTouchModeInverted(bool value)
        {
            bool old = IsTouchModeInverted;
            if (old != value)
            {
                IsTouchModeInverted = value;
                VMEvents.OnScreenStateChanged(this);
            }
            return old;
        }
    }
}
