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

        public void Initialize(DisplayControl display)
        {
            ExecutionThread = new Thread(new ThreadStart(ExecuteLuaCode));
            ExecModule = new LuaExecutionModule(0);
            DeviceBus = new DeviceBus(8);
            Display = display;
            DeviceBus.ConnectDevice(new Computer(Guid.NewGuid().ToString()));
            DeviceBus.ConnectDevice(MachineComponents.FileSystem.MountFileSystem("D:\\OCVMDrives\\d\\"));
            if (EEPROM.LoadEEPROMFromFile("LuaBios.lua" , out EEPROM eeprom))
            {
                DeviceBus.ConnectDevice(eeprom);
            }
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
