using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.OCVM.Core.MachineComponents;
using craftersmine.OCVM.Core.Extensions;
using NLua;

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
            return null;
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

        public static void beep(string pattern)
        {
            try
            {
                SoundGenerator.BeepMorse(pattern);
            }
            catch
            { }
        }

        public static string getBootAddress()
        {
            MachineComponents.Computer computer = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer") as MachineComponents.Computer;
            string bootableAddress = null;
            if (!computer.PrimaryBootDevice.IsNullEmptyOrWhitespace())
            {
                var bootableFs = VM.RunningVM.DeviceBus.GetDevice(computer.PrimaryBootDevice);
                if (bootableFs != null)
                    bootableAddress = bootableFs.Address;
                else
                {
                    var primaryFs = VM.RunningVM.DeviceBus.GetPrimaryComponent("filesystem");
                    if (primaryFs != null)
                        bootableAddress = primaryFs.Address;
                }
            }
            else
            {
                var primaryFs = VM.RunningVM.DeviceBus.GetPrimaryComponent("filesystem");
                if (primaryFs != null)
                    bootableAddress = primaryFs.Address;
            }
            return bootableAddress;
        }

        public static void setBootAddress()
        {
            setBootAddress("");
        }

        public static void setBootAddress(string address)
        {
            MachineComponents.Computer computer = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer") as MachineComponents.Computer;
            computer.PrimaryBootDevice = address;
        }

        public static int uptime()
        {
            int uptime = DateTime.Now.Second - VM.RunningVM.LaunchTime.Second;
            return uptime;
        }

        public static long getFreeMemory()
        {
            return getTotalMemory() - 1024 * 1024;
        }

        public static long getTotalMemory()
        {
            return 16 * 1024 * 1024;
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

        public static int maxEnergy()
        {
            return int.MaxValue;
        }

        public static int energy()
        {
            return int.MaxValue;
        }

        public static LuaTable users()
        {
            LuaTable users = VM.RunningVM.ExecModule.CreateTable();
            MachineComponents.Computer computer = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer") as MachineComponents.Computer;
            int count = 1;
            foreach (var usr in computer.GetUsers())
            {
                users[count] = usr;
                count++;
            }
            return users;
        }

        public static string getArchitecture()
        {
            return "Lua 5.3";
        }

        public static LuaTable getArchitectures()
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();

            table[1] = "Lua 5.3";
            table[2] = "x86";
            table["n"] = 2;

            return table;
        }

        public static void setArchitecture(string arch)
        { }

        public static bool addUser(string user, out string errorMsg)
        {
            MachineComponents.Computer computer = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer") as MachineComponents.Computer;
            bool result = computer.AddUser(user, out errorMsg);
            return result;
        }

        public static bool removeUser(string user)
        {
            MachineComponents.Computer computer = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer") as MachineComponents.Computer;
            return computer.RemoveUser(user);
        }

        public static void pushSignal(string name, LuaTable data)
        {
            MachineComponents.Computer computer = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer") as MachineComponents.Computer;
            computer.PushSignal(name, data);
        }

        public static string pullSignal(int timeout, out LuaTable data)
        {
            MachineComponents.Computer computer = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer") as MachineComponents.Computer;
            Signal sig = computer.PullSignal();
            if (sig != null)
            {
                data = sig.Data;
                return sig.Name;
            }
            else
            {
                data = null;
                return null;
            }
        } 

        public static void shutdown(bool reboot)
        { }
    }
}
