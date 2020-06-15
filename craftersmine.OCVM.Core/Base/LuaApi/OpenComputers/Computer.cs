﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.OCVM.Core.MachineComponents;
using craftersmine.OCVM.Core.Extensions;
using NLua;
using System.Runtime.InteropServices.WindowsRuntime;

namespace craftersmine.OCVM.Core.Base.LuaApi.OpenComputers
{
    public class Computer
    {
        public static string address()
        {
            var dev = VM.RunningVM.DeviceBus.GetPrimaryComponent("computer");
            return dev.Address;
        }

        public static string tmpAddress()
        {
            return Guid.Empty.ToString();
        }
        
        public static void beep(int freq, float duration)
        { }

        public static string getBootAddress()
        {
            var primaryFs = VM.RunningVM.DeviceBus.GetPrimaryComponent("filesystem");
            if (primaryFs != null)
                return primaryFs.Address;
            else return null;
        }
    }
}