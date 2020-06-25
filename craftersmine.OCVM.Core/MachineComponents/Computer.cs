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
                users.Add(user);
                errorMsg = null;
                return true;
            }
        }

        public bool RemoveUser(string user)
        {
            if (users.Contains(user))
            {
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
        }

        public Signal PullSignal()
        {
            if (signalQueue.Count > 0)
                return signalQueue.Dequeue();
            else return null;
        }
    }

    public sealed class Signal
    {
        public string Name { get; set; }
        public LuaTable Data { get; set; }

        private Signal() { }
        public Signal(string name, LuaTable data)
        {
            Name = name; Data = data;
        }
    }
}
