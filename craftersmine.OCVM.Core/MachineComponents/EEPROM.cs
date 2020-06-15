using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using craftersmine.OCVM.Core.Base.LuaApi;
using craftersmine.OCVM.Core.Attributes;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "eeprom")]
    public sealed class EEPROM : BaseComponent
    {
        public string EEPROMCode { get; private set; }

        private EEPROM() { }

        private EEPROM(string data, int capacity) : base()
        {
            EEPROMCode = data;
            if (DeviceInfo == null)
                DeviceInfo = new DeviceInfo();
            DeviceInfo.Class = DeviceClass.Memory;
            DeviceInfo.Description = "EEPROM";
            DeviceInfo.Vendor = DeviceInfo.DefaultVendor;
            DeviceInfo.Product = "Fast-Flash 2K";
            DeviceInfo.Capacity = capacity.ToString();
            DeviceInfo.Size = capacity.ToString();
        }

        public static bool LoadEEPROM(string eepromCode, out EEPROM eeprom)
        {
            if (eepromCode.Length > 4096)
            {
                eeprom = null;
                return false;
            }
            else
            {
                eeprom = new EEPROM(eepromCode);
                return true;
            }
        }

        public static bool LoadEEPROMFromFile(string eepromCodeFile, out EEPROM eeprom)
        {
            return LoadEEPROM(File.ReadAllText(eepromCodeFile), out eeprom);
        }

        [LuaCallback(IsDirect = true, Doc = "")]
        public string get()
        {
            return EEPROMCode;
        }
    }
}
