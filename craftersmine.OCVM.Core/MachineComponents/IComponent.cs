using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.MachineComponents
{
    public interface IComponent
    {
        string Address { get; set; }
        bool IsPrimary { get; set; }
        int Slot { get; set; }
        DeviceInfo DeviceInfo { get; set; }

        Dictionary<string, LuaMethodInfo> GetDeviceMethods();
        string GetDeviceMethodDoc(string method);
        OpenComputersComponentAttribute GetComponentAttribute();
    }
}
