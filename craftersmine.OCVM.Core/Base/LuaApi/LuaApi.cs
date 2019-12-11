
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public static class LuaApi
    {
        public static event EventHandler<DisplayOutputEventArgs> DisplayOutput;
        public static event EventHandler DisplayScroll;
        public static event EventHandler<DisplayCursorPositionEventArgs> DisplayCursorPositionChange;

        public static void InvokeDisplayOutput(DisplayOutputEventArgs e)
        {
            DisplayOutput?.Invoke(null, e);
        }

        public static void InvokeDisplayScroll()
        {
            DisplayScroll?.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeDisplayCursorPositionChange(DisplayCursorPositionEventArgs e)
        {
            DisplayCursorPositionChange?.Invoke(null, e);
        }
    }

    public sealed class DisplayOutputEventArgs : EventArgs
    {
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public string StringValue { get; set; }
        public bool UseDefaultColors { get; set; }
        public Point Position { get; set; }
    }

    public sealed class DisplayCursorPositionEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
