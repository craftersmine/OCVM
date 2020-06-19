using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public static class OCErrors
    {
        public static readonly string NoSuchComponent = "no such component";
        public static readonly string FileNotFound = "file not found";
        public static readonly string BadFileDescriptor = "bad file descriptor";
        public static readonly string UnsupportedMode = "unsupported mode";
        public static readonly string PermissionDenied = "access denied";
        public static readonly string UserExists = "user exists";
        public static readonly string NoBootableMediumFound = "no bootable medium found";
    }
}
