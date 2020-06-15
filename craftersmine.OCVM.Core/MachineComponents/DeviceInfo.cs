using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.MachineComponents
{
    public sealed class DeviceInfo
    {
        public string Class { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public string Product { get; set; }
        public string Version { get; set; }
        public string Serial { get; set; }
        public string Capacity { get; set; }
        public string Size { get; set; }
        public string Clock { get; set; }
        public string Width { get; set; }

        public static readonly string DefaultVendor = "SibStar Computer Hardware Corp.";

        public DeviceInfo()
        {
            Class = "class";
            Description = "description";
            Vendor = "vendor";
            Product = "product";
            Version = "version";
            Serial = "serial";
            Capacity = "capacity";
            Size = "size";
            Clock = "clock";
            Width = "width";
        }
    }

    public static class DeviceClass
    {
        public const string System = "system";
        public const string Bridge = "bridge";
        public const string Memory = "memory";
        public const string Processor = "processor";
        public const string Address = "address";
        public const string Storage = "storage";
        public const string Disk = "disk";
        public const string Tape = "tape";
        public const string Bus = "bus";
        public const string Network = "network";
        public const string Display = "display";
        public const string Input = "input";
        public const string Printer = "printer";
        public const string Multimedia = "multimedia";
        public const string Communication = "communication";
        public const string Power = "power";
        public const string Volume = "volume";
        public const string Generic = "generic";
    }
}
