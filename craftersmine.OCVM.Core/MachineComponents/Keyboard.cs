using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinputManager;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "keyboard")]
    public class Keyboard : BaseComponent
    {
        private KeyboardHook kbdHook;

        public Keyboard() : base()
        {
            DeviceInfo.Class = DeviceClass.Input;
            DeviceInfo.Description = "Keyboard";
            DeviceInfo.Vendor = DeviceInfo.DefaultVendor;
            DeviceInfo.Product = "craftersmine Virtual Input Device";
            kbdHook = new KeyboardHook();
            kbdHook.OnKeyboardEvent += OnKeyboardEvent;
            //kbdHook.Install();
        }

        internal void UninstallHook()
        {
            //kbdHook.Uninstall();
        }

        internal void ProcessKeyEvent(KeyboardSignalType type, Keys key, bool shift, bool control, bool alt)
        {
            KeyCodes keyCode = KeyCodes.NoKey;

            KeysConverter converter = new KeysConverter();
            var a = converter.ConvertToString(key);

            // Convert Windows keycodes to LWJGL keycodes that used by Minecraft as well by OpenComputers
            switch(key)
            {
                // Latin alphabet keys
                case Keys.A: keyCode = KeyCodes.A; break;
                case Keys.B: keyCode = KeyCodes.B; break;
                case Keys.C: keyCode = KeyCodes.C; break;
                case Keys.D: keyCode = KeyCodes.D; break;
                case Keys.E: keyCode = KeyCodes.E; break;
                case Keys.F: keyCode = KeyCodes.F; break;
                case Keys.G: keyCode = KeyCodes.G; break;
                case Keys.H: keyCode = KeyCodes.H; break;
                case Keys.I: keyCode = KeyCodes.I; break;
                case Keys.J: keyCode = KeyCodes.J; break;
                case Keys.K: keyCode = KeyCodes.K; break;
                case Keys.L: keyCode = KeyCodes.L; break;
                case Keys.M: keyCode = KeyCodes.M; break;
                case Keys.N: keyCode = KeyCodes.N; break;
                case Keys.O: keyCode = KeyCodes.O; break;
                case Keys.P: keyCode = KeyCodes.P; break;
                case Keys.Q: keyCode = KeyCodes.Q; break;
                case Keys.R: keyCode = KeyCodes.R; break;
                case Keys.S: keyCode = KeyCodes.S; break;
                case Keys.T: keyCode = KeyCodes.T; break;
                case Keys.U: keyCode = KeyCodes.U; break;
                case Keys.V: keyCode = KeyCodes.V; break;
                case Keys.W: keyCode = KeyCodes.W; break;
                case Keys.X: keyCode = KeyCodes.X; break;

                // Upper numeric keys
                case Keys.D1: keyCode = KeyCodes.D1; break;
                case Keys.D2: 
                    keyCode = KeyCodes.D2;
                    if (shift)
                        keyCode = KeyCodes.At;
                    break;
                case Keys.D3: keyCode = KeyCodes.D3; break;
                case Keys.D4: keyCode = KeyCodes.D4; break;
                case Keys.D5: keyCode = KeyCodes.D5; break;
                case Keys.D6: keyCode = KeyCodes.D6; break;
                case Keys.D7: keyCode = KeyCodes.D7; break;
                case Keys.D8: keyCode = KeyCodes.D8; break;
                case Keys.D9: keyCode = KeyCodes.D9; break;
                case Keys.D0: keyCode = KeyCodes.D0; break;

                // Symbol keys
                case Keys.OemQuotes: keyCode = KeyCodes.Apostrophe; break;
                case Keys.OemBackslash: keyCode = KeyCodes.Backslash; break;
                case Keys.OemSemicolon: keyCode = KeyCodes.Semicolon; break;
                
            }
        }

        private bool OnKeyboardEvent(uint key, BaseHook.KeyState keyState)
        {
            switch (keyState)
            {
                case BaseHook.KeyState.Keydown:
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
                    VM.RunningVM.ComputerInstance.PushSignal("key_up", Address, (int)signal.Character, (int)signal.Key);
                    break;
                case KeyboardSignalType.KeyDown:
                    VM.RunningVM.ComputerInstance.PushSignal("key_down", Address, (int)signal.Character, (int)signal.Key);
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

        public KeyboardSignal(KeyboardSignalType type, Keys code, char character)
        {
            Key = code;
            KeyCode = KeyCodes.NoKey;
            Type = type;
            Character = character;
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
