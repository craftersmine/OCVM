using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.OCVM.Core.MachineComponents;
using craftersmine.OCVM.Core.Extentions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.CoreLib;

namespace craftersmine.OCVM.Core.Base.LuaApi.OpenComputers
{
    [MoonSharpModule(Namespace = "component")]
    public class Component
    {
        [MoonSharpModuleMethod]
        public static DynValue doc(ScriptExecutionContext context, CallbackArguments args)
        {
            return DynValue.NewString(args[0].CastToString(), args[1].CastToString());
        }

        [MoonSharpModuleMethod]
        public static DynValue invoke(ScriptExecutionContext context, CallbackArguments args)
        {
            object invocationResult;
            string addr = Guid.Empty.ToString();
            string methodName = "";
            List<object> methodParams = new List<object>();

            for (int i = 0; i < args.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        addr = args[i].CastToString();
                        break;
                    case 1:
                        methodName = args[i].CastToString();
                        break;
                    default:
                        methodParams.Add(Converters.ConvertType(args[i]));
                        break;
                }
            }

            var device = VM.RunningVM.DeviceBus.GetDevice(addr);
            invocationResult = ((BaseComponent)device).InvokeMethod(methodName, methodParams.ToArray());
            return DynValue.FromObject(context.OwnerScript, invocationResult);
        }

        //[MoonSharpModuleMethod]
        public static DynValue __list_clr(ScriptExecutionContext context, CallbackArguments args)
        {
            DynValue devices = DynValue.NewTable(context.OwnerScript);
            //Dictionary<string, string> devices = new Dictionary<string, string>();
            string filter = args[0].CastToString();
            bool exact = args[1].CastToBool();
            IComponent[] busDevices;

            busDevices = VM.RunningVM.DeviceBus.GetDevicesByType(filter, exact);

            foreach (var entry in busDevices)
            {
                if (filter != null || filter != string.Empty || !string.IsNullOrWhiteSpace(filter))
                {
                    if (!exact)
                    {
                        if (entry.GetDeviceTypeName().Contains(filter))
                            devices.Table.Set(entry.Address, DynValue.NewString(entry.GetDeviceTypeName()));
                        if (entry.GetDeviceTypeName().StartsWith(filter))
                            devices.Table.Set(entry.Address, DynValue.NewString(entry.GetDeviceTypeName()));
                        if (entry.GetDeviceTypeName().EndsWith(filter))
                            devices.Table.Set(entry.Address, DynValue.NewString(entry.GetDeviceTypeName()));
                    }
                    else devices.Table.Set(entry.Address, DynValue.NewString(entry.GetDeviceTypeName()));
                }
                else devices.Table.Set(entry.Address, DynValue.NewString(entry.GetDeviceTypeName()));
            }

            return devices;
        }

        [MoonSharpModuleMethod]
        public static DynValue list(ScriptExecutionContext context, CallbackArguments args)
        {
            DynValue table = DynValue.NewTable(__list_clr(context, args).Table.MetaTable);
            context.GetMetamethodTailCall(table, "__call", )
            return table;
        }

        public static DynValue methods(ScriptExecutionContext context, CallbackArguments args)
        {
            //string addr = args[0].CastToString();
            //if (!string.IsNullOrEmpty(addr) || !string.IsNullOrWhiteSpace(addr))
            //{
            //    var dev = VM.RunningVM.DeviceBus.GetDevice(addr);

            //}
            Root.print("methods method is not implenented yet");
            return DynValue.NewTable(context.OwnerScript);
        }

        public static DynValue type(ScriptExecutionContext context, CallbackArguments args)
        {
            string addr = args[0].CastToString();
            if (!addr.IsNullEmptyOrWhitespace())
            {
                var dev = VM.RunningVM.DeviceBus.GetDevice(addr);
                if (dev == null)
                    return DynValue.NewTuple(DynValue.Nil, OCErrors.NoSuchComponent);
                return DynValue.NewString(dev.GetDeviceTypeName().ToLower());
            }
            return DynValue.NewTuple(DynValue.Nil, OCErrors.NoSuchComponent);
        }

        public static DynValue slot(ScriptExecutionContext context, CallbackArguments args)
        {
            string addr = args[0].CastToString();
            if (!addr.IsNullEmptyOrWhitespace())
            {
                var dev = VM.RunningVM.DeviceBus.GetDevice(addr);
                if (dev != null)
                    return DynValue.NewNumber(-1);
                else return DynValue.NewTuple(DynValue.Nil, OCErrors.NoSuchComponent);
            }
            else return DynValue.NewTuple(DynValue.Nil, OCErrors.NoSuchComponent);
        }

        public static DynValue get(ScriptExecutionContext context, CallbackArguments args)
        {
            string addr = args[0].CastToString();
            string compType = args[1].CastToString();
            string resolvedAddr = "";
            if (!addr.IsNullEmptyOrWhitespace())
            {
                resolvedAddr = VM.RunningVM.DeviceBus.GetDevice(addr).Address;
                if (!compType.IsNullEmptyOrWhitespace())
                {
                    if (VM.RunningVM.DeviceBus.GetDevice(addr).GetDeviceTypeName().ToLower() == compType.ToLower())
                        resolvedAddr = VM.RunningVM.DeviceBus.GetDevice(addr).Address;
                }
            }

            if (resolvedAddr.IsNullEmptyOrWhitespace())
                return DynValue.NewTuple(DynValue.Nil, OCErrors.NoSuchComponent);
            else return DynValue.NewString(resolvedAddr);
        }

        public static DynValue isAvailable(ScriptExecutionContext context, CallbackArguments args)
        {
            string compType = args[0].CastToString();
            bool isAvailableFound = false;
            if (!compType.IsNullEmptyOrWhitespace())
            {
                var primaryComps = VM.RunningVM.DeviceBus.GetPrimaryComponents();
                foreach (var dev in primaryComps)
                {
                    if (dev.GetDeviceTypeName().ToLower() == compType.ToLower() && dev.IsPrimary)
                    {
                        isAvailableFound = true;
                        break;
                    }
                    else continue;
                }
                return DynValue.NewBoolean(isAvailableFound);
            }
            else return DynValue.NewBoolean(false);
        }
    }
}
