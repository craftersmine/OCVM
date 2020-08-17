using craftersmine.OCVM.Core.MachineComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using craftersmine.OCVM.Core.Base.LuaApi;
using System.Reflection;
using System.Threading;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Exceptions;
using NLua;

namespace craftersmine.OCVM.Core
{
    public sealed class VM
    {
        private Thread ExecutionThread { get; set; }

        public static VM RunningVM { get; private set; }
        public static Version CurrentVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public DeviceBus DeviceBus { get; set; }
        public DisplayControl Display { get; set; }
        public LuaExecutionModule ExecModule { get; set; }
        public DateTime LaunchTime { get; private set; }
        public VMState State { get; private set; }
        public ScreenBuffer ScreenBuffer { get; set; }
        public Computer ComputerInstance { get; private set; }
        public Keyboard KeyboardInstance { get; private set; }
        public bool IsKeyboardAttached { get; internal set; } = false;

        public void Initialize(DisplayControl display)
        {
            Logger.Instance.Log(LogEntryType.Info, "Initializing VM with display @" + display.GetHashCode() + " ...");
            ExecutionThread = new Thread(new ThreadStart(ExecuteLuaCode));
            ExecModule = new LuaExecutionModule(0);
            DeviceBus = new DeviceBus(8);
            Display = display;
            ComputerInstance = new Computer(Guid.NewGuid().ToString());
            KeyboardInstance = new Keyboard();
            DeviceBus.ConnectDevice(ComputerInstance);
            DeviceBus.ConnectDevice(FileSystem.MountFileSystem("D:\\OCVMDrives\\c\\"));
            //DeviceBus.ConnectDevice(FileSystem.MountFileSystem("D:\\OCVMDrives\\d\\"));
            DeviceBus.ConnectDevice(new Screen(display.Tier));
            DeviceBus.ConnectDevice(new GPU());
            DeviceBus.ConnectDevice(KeyboardInstance);
            IsKeyboardAttached = true;
            if (EEPROM.LoadEEPROMFromFile("LuaBios.lua", out EEPROM eeprom))
            {
                DeviceBus.ConnectDevice(eeprom);
            }
            else throw new EEPROMFailedToLoadException();
            RunningVM = this;
            LaunchTime = DateTime.Now;
            VMEvents.OnVMReady();
        }

        private void ExecuteLuaCode()
        {
            VM.RunningVM.ExecModule.ExecuteString(((EEPROM)VM.RunningVM.DeviceBus.GetPrimaryComponent("eeprom")).EEPROMCode);
        }

        public void Stop(bool reboot)
        {
            if (!reboot)
                SetState(VMState.Stopping);
            else SetState(VMState.Rebooting);
            var fsms = DeviceBus.GetDevicesByType("filesystem", false);
            foreach (var fs in fsms)
            {
                ((FileSystem)fs).CloseHandles();
            }
            ((Keyboard)DeviceBus.GetPrimaryComponent("keyboard")).UninstallHook();
        }

        public void SetState(VMState state)
        {
            State = state;
            VMEvents.OnVMStateChanged(state);
        }

        public void Run()
        {
            ExecutionThread.Start();
        }
    }

    public enum VMState
    {
        Running, Stopping, Rebooting, Stopped
    }
}
