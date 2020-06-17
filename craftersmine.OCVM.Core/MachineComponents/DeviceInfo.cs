using craftersmine.OCVM.Core.Extensions;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Class = null;
            Description = null;
            Vendor = null;
            Product = null;
            Version = null;
            Serial = null;
            Capacity = null;
            Size = null;
            Clock = null;
            Width = null;
        }

        public LuaTable GetDeviceInfoTable()
        {
            var table = VM.RunningVM.ExecModule.CreateTable();

            if (!Class.IsNullEmptyOrWhitespace())
                table["class"] = Class;
            if (!Description.IsNullEmptyOrWhitespace())
                table["description"] = Description;
            if (!Vendor.IsNullEmptyOrWhitespace())
                table["vendor"] = Vendor;
            if (!Product.IsNullEmptyOrWhitespace())
                table["product"] = Product;
            if (!Version.IsNullEmptyOrWhitespace())
                table["version"] = Version;
            if (!Serial.IsNullEmptyOrWhitespace())
                table["serial"] = Serial;
            if (!Capacity.IsNullEmptyOrWhitespace())
                table["capacity"] = Capacity;
            if (!Size.IsNullEmptyOrWhitespace())
                table["size"] = Size;
            if (!Clock.IsNullEmptyOrWhitespace())
                table["clock"] = Clock;
            if (!Width.IsNullEmptyOrWhitespace())
                table["width"] = Width;

            return table;
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
