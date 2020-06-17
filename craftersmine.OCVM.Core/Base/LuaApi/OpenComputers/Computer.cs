using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.OCVM.Core.MachineComponents;
using craftersmine.OCVM.Core.Extensions;
using NLua;
using System.Runtime.InteropServices.WindowsRuntime;

namespace craftersmine.OCVM.Core.Base.LuaApi.OpenComputers
{
    public class Computer
    {
        public static string address()
        {
            var dev = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer");
            return dev.Address;
        }

        public static string tmpAddress()
        {
            return Guid.Empty.ToString();
        }
        
        public static void beep(float freq, float duration)
        { 
            try
            {
                SoundGenerator.PlaySine(freq, duration);
            }
            catch
            { }
        }

        public static string getBootAddress()
        {
            var primaryFs = VM.RunningVM.DeviceBus.GetPrimaryComponent("filesystem");
            if (primaryFs != null)
                return primaryFs.Address;
            else return null;
        }

        public static int uptime()
        {
            int uptime = DateTime.Now.Second - VM.RunningVM.LaunchTime.Second;
            return uptime;
        }

        public static long getFreeMemory()
        {
            return getTotalMemory() - GC.GetTotalMemory(false);
        }

        public static long getTotalMemory()
        {
            return 1024 * 1024 * 1024;
        }

        public static LuaTable getDeviceInfo()
        {
            LuaTable infos = VM.RunningVM.ExecModule.CreateTable();
            var devices = VM.RunningVM.DeviceBus.GetDevices();
            foreach (var dev in devices)
            {
                if (dev.DeviceInfo != null)
                    infos[dev.Address] = dev.DeviceInfo.GetDeviceInfoTable();
            }
            return infos;
        }
    }
}
