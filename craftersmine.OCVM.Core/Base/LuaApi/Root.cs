using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
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
                {
                    if (data[i] == null)
                        combined = "nil";
                    else combined = data[i].ToString();
                }
                else
                {
                    if (data[i] == null)
                        combined += "\t" + "nil";
                    else combined += "\t" + data[i].ToString();
                }
            }

            if (combined == null)
            {
                Console.WriteLine("nil");
                //LuaApi.InvokeDisplayOutput(new DisplayOutputEventArgs() { UseDefaultColors = true, StringValue = "nil", Position = VM.RunningVM.Display.CursorPosition });
                for (int i = 0; i < 3; i++)
                {
                    ScreenBuffer.Instance.Begin();
                    ScreenBuffer.Instance.Set(i, VM.RunningVM.Display.CursorPosition.Y, "nil"[i], BaseColors.White, BaseColors.Black);
                    ScreenBuffer.Instance.End();
                }
            }
            else
            {
                Console.WriteLine(combined);
                ScreenBuffer.Instance.Begin();
                //LuaApi.InvokeDisplayOutput(new DisplayOutputEventArgs() { UseDefaultColors = true, StringValue = combined, Position = VM.RunningVM.Display.CursorPosition });
                if (combined.Length > ScreenBuffer.Instance.Width)
                {
                    for (int i = 0; i < combined.Length / ScreenBuffer.Instance.Width; i++)
                    {
                        print(combined.Substring(i * ScreenBuffer.Instance.Width, ScreenBuffer.Instance.Width));
                    }
                }
                else
                {
                    for (int i = 0; i < combined.Length; i++)
                    {
                        ScreenBuffer.Instance.Set(i, VM.RunningVM.Display.CursorPosition.Y, combined[i], BaseColors.White, BaseColors.Black);
                    }
                }
                ScreenBuffer.Instance.End();
                if (displayCursorY == VM.RunningVM.Display.DisplayHeight - 1)
                {
                    ScreenBuffer.Instance.Scroll();
                }
                else
                {
                    displayCursorY++;
                    VM.RunningVM.Display.CursorPosition = new System.Drawing.Point(0, displayCursorY);
                }
            }
        }

        public static bool checkArgType(string argLuaType, params string[] checkTypes)
        {
            try
            {
                foreach (var type in checkTypes)
                {
                    if (argLuaType.ToLower() == type.ToLower())
                        return true;
                    else continue;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void breakpoint(object data)
        {
            print("OCVM_INTERNAL_LUA_BREAKPOINT_HIT: " + data);
        }
    }
}
