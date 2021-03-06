﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base
{
    public enum KeyCodes
    {
        // Upper numeric keys
        D1 = 0x02, D2 = 0x03, D3 = 0x04, D4 = 0x05, D5 = 0x06,
        D6 = 0x07, D7 = 0x08, D8 = 0x09, D9 = 0x0A, D0 = 0x0B,
        
        // Latin keys
        A = 0x1E, B = 0x30, C = 0x2E, D = 0x20, E = 0x12,
        F = 0x21, G = 0x22, H = 0x23, I = 0x17, J = 0x24,
        K = 0x25, L = 0x26, M = 0x32, N = 0x31, O = 0x18,
        P = 0x19, Q = 0x10, R = 0x13, S = 0x1F, T = 0x14,
        U = 0x16, V = 0x2F, W = 0x11, X = 0x2D, Y = 0x15, Z = 0x2C,

        // Symbol keys
        Apostrophe = 0x28, At = 0x91, Backslash = 0x2B,
        Colon = 0x92, Comma = 0x33, Equals = 0x0D, Grave = 0x29,
        LBracket = 0x1A, Minus = 0x0C, Period = 0x34,
        RBracket = 0x1B, Semicolon = 0x27, Slash = 0x27,
        Space = 0x39, Tab = 0x0F, Circumflex = 0x90, Underline = 0x93,

        // Modifier and special keys
        Backspace = 0x0E, CapsLock = 0x3A, Enter = 0x1C,
        LAlt = 0x38, LShift = 0x2A, LControl = 0x1D,
        RAlt = 0xB8, RShift = 0x36, RControl = 0x9D,
        NumLock = 0x45, Pause = 0xC5, ScrollLock = 0x46,
        Stop = 0x95, NoKey = 0x1FFFFFFF,

        // Additional keypad keys
        Up = 0xC8, Down = 0xD0, Left = 0xCB, Right = 0xCD,
        Home = 0xC7, End = 0xCF, PageUp = 0xC9, PageDown = 0xD1,
        Insert = 0xD2, Delete = 0xD3,

        // Function keys
        F1 = 0x3B, F2 = 0x3C, F3 = 0x3D, F4 = 0x3E, F5 = 0x3F,
        F6 = 0x40, F7 = 0x41, F8 = 0x42, F9 = 0x43, F10 = 0x44,
        F11 = 0x57, F12 = 0x58, F13 = 0x64, F14 = 0x65, F15 = 0x66,
        F16 = 0x67, F17 = 0x68, F18 = 0x69, F19 = 0x71,

        // Japanese keys
        Kana = 0x70, Kanji = 0x94, Convert = 0x79, NoConvert = 0x7B,
        Yen = 0x7D, Ax = 0x96,

        // Numpad numeric keys
        Num0 = 0x52, Num1 = 0x4F, Num2 = 0x50, Num3 = 0x51, Num4 = 0x4B,
        Num5 = 0x4C, Num6 = 0x4D, Num7 = 0x47, Num8 = 0x48, Num9 = 0x49,

        // Numpad additional keys
        NumMul = 0x37, NumDiv = 0xB5, NumSub = 0x4A, NumAdd = 0x4E,
        NumDecimal = 0x53, NumComma = 0xB3, NumEnter = 0x9C,
        NumEquals = 0x8D
    }
}
