using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Native;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WinputManager;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "keyboard")]
    public class Keyboard : BaseComponent
    {
        private KeyboardHook kbdHook;
        private KeyboardInputProcessor kbdInpProcessor;

        public Keyboard() : base()
        {
            DeviceInfo.Class = DeviceClass.Input;
            DeviceInfo.Description = "Keyboard";
            DeviceInfo.Vendor = DeviceInfo.DefaultVendor;
            DeviceInfo.Product = "craftersmine Virtual Input Device";

            kbdHook = new KeyboardHook();
            kbdHook.OnKeyboardEvent += OnKeyboardEvent;
            Logger.Instance.Log(LogEntryType.Info, "Initialized WinputManager KeyboardHook instance");
            kbdInpProcessor = new KeyboardInputProcessor();
            Logger.Instance.Log(LogEntryType.Info, "Initialized KeyboardInputProcessor instance");

            //Logger.Instance.Log(LogEntryType.Info, "Hooking keyboard input...");
            //kbdHook.Install();
        }

        internal void UninstallHook()
        {
            //Logger.Instance.Log(LogEntryType.Info, "Unhooking keyboard input...");
            //kbdHook.Uninstall();
        }

        internal void ProcessKeyEvent(KeyboardSignalType type, Keys key, bool shift, bool control, bool alt)
        {
            KeyCodes keyCode = KeyCodes.NoKey;

            KeysConverter converter = new KeysConverter();
            var a = converter.ConvertToString(key);

            // Convert Windows keycodes to LWJGL 2 keycodes that used by Minecraft as well by OpenComputers
            // Many of case statements...
            switch(key)
            {
                // Latin alphabet keys
                case (Keys)0x41: keyCode = KeyCodes.A; break;
                case (Keys)0x42: keyCode = KeyCodes.B; break;
                case (Keys)0x43: keyCode = KeyCodes.C; break;
                case (Keys)0x44: keyCode = KeyCodes.D; break;
                case (Keys)0x45: keyCode = KeyCodes.E; break;
                case (Keys)0x46: keyCode = KeyCodes.F; break;
                case (Keys)0x47: keyCode = KeyCodes.G; break;
                case (Keys)0x48: keyCode = KeyCodes.H; break;
                case (Keys)0x49: keyCode = KeyCodes.I; break;
                case (Keys)0x4A: keyCode = KeyCodes.J; break;
                case (Keys)0x4B: keyCode = KeyCodes.K; break;
                case (Keys)0x4C: keyCode = KeyCodes.L; break;
                case (Keys)0x4D: keyCode = KeyCodes.M; break;
                case (Keys)0x4E: keyCode = KeyCodes.N; break;
                case (Keys)0x4F: keyCode = KeyCodes.O; break;
                case (Keys)0x50: keyCode = KeyCodes.P; break;
                case (Keys)0x51: keyCode = KeyCodes.Q; break;
                case (Keys)0x52: keyCode = KeyCodes.R; break;
                case (Keys)0x53: keyCode = KeyCodes.S; break;
                case (Keys)0x54: keyCode = KeyCodes.T; break;
                case (Keys)0x55: keyCode = KeyCodes.U; break;
                case (Keys)0x56: keyCode = KeyCodes.V; break;
                case (Keys)0x57: keyCode = KeyCodes.W; break;
                case (Keys)0x58: keyCode = KeyCodes.X; break;
                case (Keys)0x59: keyCode = KeyCodes.Y; break;
                case (Keys)0x5A: keyCode = KeyCodes.Z; break;

                // Upper numeric keys
                case Keys.D1: keyCode = KeyCodes.D1; break;
                case Keys.D2: 
                    keyCode = KeyCodes.D2;
                    if (shift)
                        keyCode = KeyCodes.At;   // @
                    break;
                case Keys.D3: keyCode = KeyCodes.D3; break;
                case Keys.D4: keyCode = KeyCodes.D4; break;
                case Keys.D5: keyCode = KeyCodes.D5; break;
                case Keys.D6: 
                    keyCode = KeyCodes.D6;
                    if (shift)
                        keyCode = KeyCodes.Circumflex;   // ^
                    break;
                case Keys.D7: keyCode = KeyCodes.D7; break;
                case Keys.D8: keyCode = KeyCodes.D8; break;
                case Keys.D9: keyCode = KeyCodes.D9; break;
                case Keys.D0: keyCode = KeyCodes.D0; break;

                // Symbol keys
                case (Keys)0xDE: keyCode = KeyCodes.Apostrophe; break;   // ' (")
                case (Keys)0xDC: keyCode = KeyCodes.Backslash; break;   // \ (|)
                case (Keys)0xBA: 
                    keyCode = KeyCodes.Semicolon;   // ;
                    if (shift)
                        keyCode = KeyCodes.Colon;   // :
                    break;
                case (Keys)0xBC: keyCode = KeyCodes.Comma; break;   // , (>)
                case (Keys)0xBE: keyCode = KeyCodes.Period; break;   // . (<)
                case (Keys)0xBB: keyCode = KeyCodes.Equals; break;   // = (+)
                case (Keys)0xC0: keyCode = KeyCodes.Grave; break;   // ~ (`)
                case (Keys)0xDB: keyCode = KeyCodes.LBracket; break;   // [ ({)
                case (Keys)0xDD: keyCode = KeyCodes.RBracket; break;   // ] (})
                case (Keys)0xBD:
                    keyCode = KeyCodes.Minus;   // -
                    if (shift)
                        keyCode = KeyCodes.Underline;   // _
                    break;
                case (Keys)0xBF: keyCode = KeyCodes.Slash; break;   // / (?)
                case (Keys)0x20: keyCode = KeyCodes.Space; break;   // [SPACEBAR]
                case (Keys)0x09: keyCode = KeyCodes.Tab; break;     // Tab (<--  -->)

                // Modifier and special keys
                case (Keys)0x08: keyCode = KeyCodes.Backspace; break;
                case (Keys)0x14: keyCode = KeyCodes.CapsLock; break;
                case (Keys)0x0D: keyCode = KeyCodes.Enter; break;
                case (Keys)0xA4: keyCode = KeyCodes.LAlt; break;
                case (Keys)0xA2: keyCode = KeyCodes.LControl; break;
                case (Keys)0xA0: keyCode = KeyCodes.LShift; break;
                case (Keys)0xA5: keyCode = KeyCodes.RAlt; break;
                case (Keys)0xA3: keyCode = KeyCodes.RControl; break;
                case (Keys)0xA1: keyCode = KeyCodes.RShift; break;
                case (Keys)0x90: keyCode = KeyCodes.NumLock; break;
                case (Keys)0x13: keyCode = KeyCodes.Pause; break;
                case (Keys)0x91: keyCode = KeyCodes.ScrollLock; break;
                case (Keys)0x03: keyCode = KeyCodes.Stop; break;
                case (Keys)0x26: keyCode = KeyCodes.Up; break;
                case (Keys)0x28: keyCode = KeyCodes.Down; break;
                case (Keys)0x25: keyCode = KeyCodes.Left; break;
                case (Keys)0x27: keyCode = KeyCodes.Right; break;
                case (Keys)0x24: keyCode = KeyCodes.Home; break;
                case (Keys)0x23: keyCode = KeyCodes.End; break;
                case (Keys)0x21: keyCode = KeyCodes.PageUp; break;
                case (Keys)0x22: keyCode = KeyCodes.PageDown; break;
                case (Keys)0x2D: keyCode = KeyCodes.Insert; break;
                case (Keys)0x2E: keyCode = KeyCodes.Delete; break;

                // Function keys
                case (Keys)0x70: keyCode = KeyCodes.F1; break;
                case (Keys)0x71: keyCode = KeyCodes.F2; break;
                case (Keys)0x72: keyCode = KeyCodes.F3; break;
                case (Keys)0x73: keyCode = KeyCodes.F4; break;
                case (Keys)0x74: keyCode = KeyCodes.F5; break;
                case (Keys)0x75: keyCode = KeyCodes.F6; break;
                case (Keys)0x76: keyCode = KeyCodes.F7; break;
                case (Keys)0x77: keyCode = KeyCodes.F8; break;
                case (Keys)0x78: keyCode = KeyCodes.F9; break;
                case (Keys)0x79: keyCode = KeyCodes.F10; break;
                case (Keys)0x7A: keyCode = KeyCodes.F11; break;
                case (Keys)0x7B: keyCode = KeyCodes.F12; break;
                case (Keys)0x7C: keyCode = KeyCodes.F13; break;
                case (Keys)0x7D: keyCode = KeyCodes.F14; break;
                case (Keys)0x7E: keyCode = KeyCodes.F15; break;
                case (Keys)0x7F: keyCode = KeyCodes.F16; break;
                case (Keys)0x80: keyCode = KeyCodes.F17; break;
                case (Keys)0x81: keyCode = KeyCodes.F18; break;
                case (Keys)0x82: keyCode = KeyCodes.F19; break;

                // Japanese keys
                case (Keys)0x15: keyCode = KeyCodes.Kana; break;
                case (Keys)0x19: keyCode = KeyCodes.Kanji; break;
                case (Keys)0x1C: keyCode = KeyCodes.Convert; break;
                case (Keys)0x1D: keyCode = KeyCodes.NoConvert; break;
                // case (Keys)0x??: keyCode = KeyCodes.Yen; break;    // May interfere with \ (|) key
                // case (Keys)0x??: keyCode = KeyCodes.Ax; break;     // May interfere with Esc key

                // Numpad numeric keys
                case (Keys)0x60: keyCode = KeyCodes.Num0; break;
                case (Keys)0x61: keyCode = KeyCodes.Num1; break;
                case (Keys)0x62: keyCode = KeyCodes.Num2; break;
                case (Keys)0x63: keyCode = KeyCodes.Num3; break;
                case (Keys)0x64: keyCode = KeyCodes.Num4; break;
                case (Keys)0x65: keyCode = KeyCodes.Num5; break;
                case (Keys)0x66: keyCode = KeyCodes.Num6; break;
                case (Keys)0x67: keyCode = KeyCodes.Num7; break;
                case (Keys)0x68: keyCode = KeyCodes.Num8; break;
                case (Keys)0x69: keyCode = KeyCodes.Num9; break;

                // Numpad additional keys
                case (Keys)0x6A: keyCode = KeyCodes.NumMul; break;
                case (Keys)0x6B: keyCode = KeyCodes.NumAdd; break;
                case (Keys)0x6C: keyCode = KeyCodes.NumComma; break;
                case (Keys)0x6D: keyCode = KeyCodes.NumSub; break;
                case (Keys)0x6F: keyCode = KeyCodes.NumDiv; break;
                case (Keys)0x6E: keyCode = KeyCodes.NumDecimal; break;
                // case (Keys)0x??: keyCode = KeyCodes.NumEnter; break;    // Windows sends same code for NumEnter and Enter
                // case (Keys)0x??: keyCode = KeyCodes.NumEquals; break;   // Windows doesn't have this key code
            }

            SendKeyboardSignal(new KeyboardSignal(type, keyCode, key, kbdInpProcessor.KeyCodeToUnicode(key)));
        }

        private bool OnKeyboardEvent(uint key, BaseHook.KeyState keyState)
        {
            switch (keyState)
            {
                case BaseHook.KeyState.Keydown:
                    //Logger.Instance.Log(LogEntryType.Debug, "KeyDown event from hook: " + key.ToString() + " | " + kbdInpProcessor.KeyCodeToUnicode((Keys)key));
                    SendKeyboardSignal(KeyboardSignalType.KeyDown, ((Keys)key).ToString(), key);
                    break;
                case BaseHook.KeyState.Keyup:
                    SendKeyboardSignal(KeyboardSignalType.KeyUp, ((Keys)key).ToString(), key);
                    break;
            }
            return false;
        }

        public void SendKeyboardSignal(KeyboardSignalType signal, params object[] data)
        {
            switch (signal)
            {
                case KeyboardSignalType.KeyUp:
                    VM.RunningVM.ComputerInstance.PushSignal("key_up", Address, (uint)data[1], (char)((uint)data[1]));
                    break;
                case KeyboardSignalType.KeyDown:
                    VM.RunningVM.ComputerInstance.PushSignal("key_down", Address, (uint)data[1], (char)((uint)data[1]));
                    break;
                case KeyboardSignalType.Clipboard:
                    VM.RunningVM.ComputerInstance.PushSignal("clipboard", Address, (string)data[0]);
                    break;
                case KeyboardSignalType.Generic:
                default:
                    break;
            }
        }

        public void SendKeyboardSignal(KeyboardSignal signal)
        {
            switch (signal.Type)
            {
                case KeyboardSignalType.KeyUp:
                    VM.RunningVM.ComputerInstance.PushSignal("key_up", Address, signal.Character, (int)signal.KeyCode);
                    break;
                case KeyboardSignalType.KeyDown:
                    VM.RunningVM.ComputerInstance.PushSignal("key_down", Address, signal.Character, (int)signal.KeyCode);
                    break;
                case KeyboardSignalType.Generic:
                default:
                    break;
            }
        }
    }

    public struct KeyboardSignal
    {
        public KeyboardSignalType Type { get; set; }
        public KeyCodes KeyCode { get; set; }
        public char Character { get; set; }
        public Keys Key { get; set; }

        public KeyboardSignal(KeyboardSignalType type, KeyCodes lwjglCode, Keys code, string character)
        {
            Key = code;
            KeyCode = lwjglCode;
            Type = type;
            if (character.Length > 0)
                Character = character[0];
            else
                Character = (char)0;
        }
    }

    public enum KeyboardSignalType
    {
        KeyUp,
        KeyDown,
        Clipboard,
        Generic
    }
}
