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
    public sealed class EEPROM : BaseComponent
    {
        public string EEPROMCode { get; private set; }

        private EEPROM(string data) : base()
        {
            EEPROMCode = data;
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
