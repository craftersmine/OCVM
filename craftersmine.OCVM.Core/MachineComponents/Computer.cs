using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base.LuaApi;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "computer")]
    public sealed class Computer : BaseComponent
    {
        private List<string> users = new List<string>();
        private Queue<Signal> signalQueue = new Queue<Signal>();

        public int AllocatedVirtualMemory { get; set; }
        public int FreeMemory { get { return AllocatedVirtualMemory / 2; } }
        public string PrimaryBootDevice { get; set; }

        public Computer(string address) : base(address)
        {
            DeviceInfo.Class = DeviceClass.Processor;
            DeviceInfo.Description = "Computer";
            DeviceInfo.Product = "craftersmine OpenComputers Virtual Machine";
            DeviceInfo.Vendor = "craftersmine";
            DeviceInfo.Version = VM.CurrentVersion.ToString();
        }

        public bool AddUser(string user, out string errorMsg)
        {
            if (users.Contains(user))
            {
                errorMsg = OCErrors.UserExists;
                return false;
            }
            else
            {
                Logger.Instance.Log(LogEntryType.Info, "Added computer user: " + user);
                users.Add(user);
                errorMsg = null;
                return true;
            }
        }

        public bool RemoveUser(string user)
        {
            if (users.Contains(user))
            {
                Logger.Instance.Log(LogEntryType.Info, "Removed computer user: " + user);
                users.Remove(user);
                return true;
            }
            else return false;
        }
        
        public string[] GetUsers()
        {
            return users.ToArray();
        }

        public void PushSignal(string name, LuaTable data)
        {
            signalQueue.Enqueue(new Signal(name, data));
            if (Settings.EnableLoggingSignals)
            {
                Logger.Instance.Log(LogEntryType.Info, "Pushed computer signal: " + name, true);
                foreach (var val in data.Values)
                {
                    try
                    {
                        Logger.Instance.Log(LogEntryType.Info, "Signal data value: " + val, true);
                    }
                    catch
                    {
                        Logger.Instance.Log(LogEntryType.Warning, "Unable to convert signal data value to string!", true);
                    }
                }
            }
        }

        public void PushSignal(string name, params object[] data)
        {
            signalQueue.Enqueue(new Signal(name, data));
        }

        public Signal PullSignal()
        {
            if (signalQueue.Count > 0)
            {
                var signal = signalQueue.Dequeue();
                if (Settings.EnableLoggingSignals)
                {
                    Logger.Instance.Log(LogEntryType.Info, "Pulled computer signal: " + signal.Name);
                    foreach (var val in signal.Data)
                    {
                        try
                        {
                            Logger.Instance.Log(LogEntryType.Info, "Signal data value: " + val.ToString(), true);
                        }
                        catch
                        {
                            Logger.Instance.Log(LogEntryType.Warning, "Unable to convert signal data value to string!", true);
                        }
                    }
                }
                return signal;
            }
            else return null;
        }
    }

    public sealed class Signal
    {
        public string Name { get; set; }
        public object[] Data { get; set; }

        private Signal() { }
        public Signal(string name, params object[] data)
        {
            Name = name; Data = data;
        }
    }
}
