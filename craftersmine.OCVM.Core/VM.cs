using craftersmine.OCVM.Core.MachineComponents;
using craftersmine.OCVM.Core.Base.LuaApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core
{
    public sealed class VM
    {
        public static VM RunningVM { get; private set; }

        public DeviceBus DeviceBus { get; set; }
        public Display Display { get; set; }
        public LuaExecutionModule ExecModule { get; set; }

        public void Launch(Display display)
        {
            ExecModule = new LuaExecutionModule(0);
            DeviceBus = new DeviceBus(8);
            Display = display;
            if (EEPROM.LoadEEPROMFromFile("LuaBios.lua" , out EEPROM eeprom))
            {
                eeprom.Address = Guid.Empty.ToString();
                DeviceBus.ConnectDevice(eeprom);
            }
            RunningVM = this;
            //ExecModule.ExecuteString(((EEPROM)(DeviceBus.GetDevicesByType("eeprom", true)[0])).EEPROMCode);
        }

        public void Stop()
        {

        }
    }
}
