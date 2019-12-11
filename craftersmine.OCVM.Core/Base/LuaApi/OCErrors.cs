using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public static class OCErrors
    {
        public static readonly DynValue NoSuchComponent = DynValue.NewString("no such component");
    }
}
