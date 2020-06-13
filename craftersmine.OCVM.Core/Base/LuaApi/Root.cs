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
        public static void print(params object[] data)
        {
            string combined = "";

            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0)
                    combined = data[i].ToString();
                else {
                    if (data[i] == null)
                        combined += "\t" + "nil";
                    else combined += "\t" + data[i].ToString();
                }
            }

            if (combined == null)
            {
                LuaApi.InvokeDisplayOutput(new DisplayOutputEventArgs() { UseDefaultColors = true, StringValue = "nil", Position = VM.RunningVM.Display.CursorPosition });
            }
            else
            {
                LuaApi.InvokeDisplayOutput(new DisplayOutputEventArgs() { UseDefaultColors = true, StringValue = combined, Position = VM.RunningVM.Display.CursorPosition });
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

        public static void breakpoint(object data)
        {
            print("OCVM_INTERNAL_LUA_BREAKPOINT_HIT: " + data);
        }
    }
}
