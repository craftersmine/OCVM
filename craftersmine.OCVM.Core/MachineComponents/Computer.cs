using craftersmine.OCVM.Core.Attributes;
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
        public Computer(string address) : base(address)
        {
            DeviceInfo.Class = DeviceClass.Processor;
            DeviceInfo.Description = "Computer";
            DeviceInfo.Product = "craftersmine OpenComputers Virtual Machine";
            DeviceInfo.Vendor = "craftersmine";
            DeviceInfo.Version = VM.CurrentVersion.ToString();
        }
    }
}
