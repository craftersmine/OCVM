using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public static class OCErrors
    {
        public const string NoSuchComponent = "no such component";
        public const string FileNotFound = "file not found";
        public const string BadFileDescriptor = "bad file descriptor";
        public const string UnsupportedMode = "unsupported mode";
        public const string PermissionDenied = "access denied";
        public const string UserExists = "user exists";
        public const string NoBootableMediumFound = "no bootable medium found";
        public const string MachineHalted = "computer halted";
        public const string InvalidFillValue = "invalid fill value";
        public const string InvalidAddress = "invalid address";
        public const string NotAScreen = "not a screen";
        public const string InvalidPaletteIndex = "invalid palette index";
        public const string UnsupportedDepth = "unsupported depth";
        public const string InvalidBufferIndex = "invalid buffer index";
        public const string UnsupportedViewportSize = "unsupported viewport size";
    }
}
