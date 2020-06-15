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

        public static object invoke(string address, string method, LuaTable args, out object result1, out object result2, out object result3, out object result4, out object result5)
        {
            result1 = null;
            result2 = null;
            result3 = null;
            result4 = null;
            result5 = null;
            object invocationResult;
            var device = VM.RunningVM.DeviceBus.GetDevice(address);
            invocationResult = ((BaseComponent)device).InvokeMethod(method, args.GetValuesAsArray());
            if (invocationResult.GetType().BaseType == typeof(Array))
            {
                var arr = ((Array)invocationResult);
                switch (arr.Length)
                {
                    case 1:
                        return arr.GetValue(0);
                    case 2:
                        result1 = arr.GetValue(1);
                        return arr.GetValue(0);
                    case 3:
                        result1 = arr.GetValue(1);
                        result2 = arr.GetValue(2);
                        return arr.GetValue(0);
                    case 4:
                        result1 = arr.GetValue(1);
                        result2 = arr.GetValue(2);
                        result3 = arr.GetValue(3);
                        return arr.GetValue(0);
                    case 5:
                        result1 = arr.GetValue(1);
                        result2 = arr.GetValue(2);
                        result3 = arr.GetValue(3);
                        result4 = arr.GetValue(4);
                        return arr.GetValue(0);
                    case 6:
                    default:
                        result1 = arr.GetValue(1);
                        result2 = arr.GetValue(2);
                        result3 = arr.GetValue(3);
                        result4 = arr.GetValue(4);
                        result5 = arr.GetValue(5);
                        return arr.GetValue(0);
                }
            }
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
            LuaTable tableMethods = VM.RunningVM.ExecModule.CreateTable();

            var device = VM.RunningVM.DeviceBus.GetDevice(address);

            if (device != null)
            {
                Dictionary<string, LuaMethodInfo> methods = device.GetDeviceMethods();

                foreach (var method in methods)
                {
                    LuaTable methodInfo = VM.RunningVM.ExecModule.CreateTable();
                    methodInfo["direct"] = method.Value.IsDirect;
                    methodInfo["setter"] = method.Value.IsSetter;
                    methodInfo["getter"] = method.Value.IsGetter;
                    methodInfo["doc"] = method.Value.Doc;
                    tableMethods[method.Key] = methodInfo;
                }
            }
            else tableMethods[null] = OCErrors.NoSuchComponent;

            return tableMethods;
        }

        public static LuaTable fields(string address)
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();

            var device = VM.RunningVM.DeviceBus.GetDevice(address);
            if (device != null)
                table[1] = true;
            else
            {
                table[1] = null;
                table[2] = OCErrors.NoSuchComponent;
            }
            return table;
        }

        public static LuaTable get(string address, string componentType)
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();

            if (!componentType.IsNullEmptyOrWhitespace())
            {
                var device = VM.RunningVM.DeviceBus.GetDevice(address);
                if (device != null)
                    table[1] = device.Address;
                else
                {
                    table[1] = null;
                    table[2] = OCErrors.NoSuchComponent;
                }
            }
            else
            {
                bool isDeviceFound = false;
                var devices = VM.RunningVM.DeviceBus.GetDevicesByType(componentType, false);
                foreach (var dev in devices)
                {
                    var uuid = GetUUID(address);
                    if (dev.Address == uuid)
                    {
                        isDeviceFound = true;
                        table[1] = uuid;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (!isDeviceFound)
                {
                    table[1] = null;
                    table[2] = OCErrors.NoSuchComponent;
                }
            }

            return table;
        }

        public static bool isAvailable(string componentType)
        {
            var devices = VM.RunningVM.DeviceBus.GetDevicesByType(componentType, false);
            IComponent primaryDevice = null;
            foreach (var dev in devices)
            {
                if (dev.IsPrimary)
                {
                    primaryDevice = dev;
                    break;
                }
                else continue;
            }
            if (primaryDevice != null)
                return true;
            return false;
        }

        public static LuaTable getPrimary(string componentType)
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();
            if (isAvailable(componentType))
            {
                var devices = VM.RunningVM.DeviceBus.GetDevicesByType(componentType, false);
                IComponent primaryDevice = null;
                foreach (var dev in devices)
                {
                    if (dev.IsPrimary)
                    {
                        primaryDevice = dev;
                        break;
                    }
                    else continue;
                }
                if (primaryDevice != null)
                {

                }
            }
            return table;
        }
    }
}
