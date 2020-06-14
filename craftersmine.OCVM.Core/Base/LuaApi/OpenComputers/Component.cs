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
    public class Component
    {
        public static string GetUUID(string uuid)
        {
            var dev = VM.RunningVM.DeviceBus.GetDevice(uuid);
            if (dev == null)
                return null;
            return dev.Address;
        }

        public static string doc(string address, string method)
        {
            var device = VM.RunningVM.DeviceBus.GetDevice(address);
            
            if (device != null)
            {
                return device.GetDeviceMethodDoc(method);
            }

            return "";
        }

        public static object invoke(string address, string method, LuaTable args)
        {
            object invocationResult;
            string addr = Guid.Empty.ToString();
            var device = VM.RunningVM.DeviceBus.GetDevice(addr);
            invocationResult = ((BaseComponent)device).InvokeMethod(method, args.GetValuesAsArray());
            return invocationResult;
        }

        public static LuaTable list(string filter, bool exact)
        {
            List<IComponent> devices = new List<IComponent>();

            IComponent[] busDevices;

            busDevices = VM.RunningVM.DeviceBus.GetDevicesByType(filter, exact);

            foreach (var entry in busDevices)
            {
                if (filter != null || filter != string.Empty || !string.IsNullOrWhiteSpace(filter))
                {
                    if (!exact)
                    {
                        if (entry.GetDeviceTypeName().StartsWith(filter))
                            devices.Add(entry);
                        if (entry.GetDeviceTypeName().Contains(filter))
                            devices.Add(entry);
                        if (entry.GetDeviceTypeName().EndsWith(filter))
                            devices.Add(entry);
                    }
                    else
                    {
                        devices.Add(entry);
                    }
                }
                else
                {
                    devices.Add(entry);
                }
            }

            LuaTable devs = VM.RunningVM.ExecModule.CreateTable();

            foreach (var dev in devices)
            {
                devs[dev.Address] = dev.GetDeviceTypeName().ToLower();
            }

            return devs;
        }

        public static LuaTable type(string address)
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();
            var device = VM.RunningVM.DeviceBus.GetDevice(address);
            if (device != null)
                table[1] = device.GetDeviceTypeName().ToLower();
            else
            {
                table[1] = null;
                table[2] = OCErrors.NoSuchComponent;
            }
            return table;
        }

        public static LuaTable slot(string address)
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();
            var device = VM.RunningVM.DeviceBus.GetDevice(address);
            if (device != null)
                table[1] = device.Slot;
            else
            {
                table[1] = null;
                table[2] = OCErrors.NoSuchComponent;
            }
            return table;
        }

        public static LuaTable methods(string address)
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();

            var device = VM.RunningVM.DeviceBus.GetDevice(address);

            if (device != null)
            {
                var methods = device.GetDeviceMethods();

                foreach (var method in methods)
                {
                    table[method.Key] = method.Value;
                }
            }
            else table[null] = OCErrors.NoSuchComponent;

            return table;
        }
    }
}
