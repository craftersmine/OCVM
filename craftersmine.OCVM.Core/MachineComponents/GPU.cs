using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "gpu")]
    public sealed class GPU : BaseComponent
    {
        public GPU() : base()
        {
            
        }

        public object[] get(int x, int y)
        {
            var data = new object[5];
            var v = ScreenBufferManager.Instance.GetBuffer(0).Get(x, y);
            data[0] = v.Character.ToString();
            data[1] = v.ForegroundColor.ToArgb();
            data[2] = v.BackgroundColor.ToArgb();
            data[3] = null;
            data[4] = null;
            return data;
        }
    }
}
