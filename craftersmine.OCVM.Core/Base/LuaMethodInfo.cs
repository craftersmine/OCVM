using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base
{
    public struct LuaMethodInfo
    {
        public bool IsDirect { get; set; }
        public bool IsGetter { get; set; }
        public bool IsSetter { get; set; }
        public string Doc { get; set; }

        public LuaMethodInfo(bool isDirect = false, bool isGetter = false, bool isSetter = false, string doc = "")
        {
            IsDirect = isDirect;
            IsGetter = isGetter;
            IsSetter = isSetter;
            Doc = doc;
        }
    }
}
