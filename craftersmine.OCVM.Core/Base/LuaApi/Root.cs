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

            var buffer = ScreenBufferManager.Instance.GetBuffer(0);

            if (combined == null)
            {
                Console.WriteLine("nil");
                //LuaApi.InvokeDisplayOutput(new DisplayOutputEventArgs() { UseDefaultColors = true, StringValue = "nil", Position = VM.RunningVM.Display.CursorPosition });
                for (int i = 0; i < 3; i++)
                {
                    buffer.Begin();
                    buffer.Set(i, VM.RunningVM.Display.CursorPosition.Y, "nil"[i], BaseColors.White, BaseColors.Black);
                    buffer.End();
                }
            }
            else
            {
                Console.WriteLine(combined);
                buffer.Begin();
                //LuaApi.InvokeDisplayOutput(new DisplayOutputEventArgs() { UseDefaultColors = true, StringValue = combined, Position = VM.RunningVM.Display.CursorPosition });
                if (combined.Length > buffer.Width)
                {
                    for (int i = 0; i < combined.Length / buffer.Width; i++)
                    {
                        print(combined.Substring(i * buffer.Width, buffer.Width));
                    }
                }
                else
                {
                    for (int i = 0; i < combined.Length; i++)
                    {
                        buffer.Set(i, VM.RunningVM.Display.CursorPosition.Y, combined[i], BaseColors.White, BaseColors.Black);
                    }
                }
                buffer.End();
                if (displayCursorY == VM.RunningVM.Display.DisplayHeight - 1)
                {
                    buffer.Scroll();
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

        public static void LogConsole(string data)
        {
            Console.WriteLine(data);
        }
    }
}
