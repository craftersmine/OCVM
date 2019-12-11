using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public sealed class Root
    {
        private static int displayCursorY = 0;
        public static void print(object data)
        {
            LuaApi.InvokeDisplayOutput(new DisplayOutputEventArgs() { UseDefaultColors = true, StringValue = data.ToString(), Position = VM.RunningVM.Display.CursorPosition });
            if (displayCursorY == VM.RunningVM.Display.DisplayHeight - 1)
            {
                LuaApi.InvokeDisplayScroll();
            }
            else
            {
                displayCursorY++;
                VM.RunningVM.Display.CursorPosition = new System.Drawing.Point(0, displayCursorY);
            }
        }
    }
}
