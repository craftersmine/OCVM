using craftersmine.OCVM.Core.Extensions;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base.LuaApi.OpenComputers
{
    public sealed class Unicode
    {
        public static LuaTable _char(LuaTable args)
        {
            LuaTable chars = VM.RunningVM.ExecModule.CreateTable();
            var _args = args.GetValuesAsArray();
            List<int> values = new List<int>();
            char[] chrs = new char[_args.Length];
            for (int i = 0; i < _args.Length; i++)
            {
                chrs[i] = (char)Convert.ToInt32(_args[i]);
            }

            for (int i = 1; i <= chrs.Length; i++)
            {
                chars[i] = chrs[i - 1].ToString();
            }

            return chars;
        }

        public static int charWidth(string _char)
        {
            char chr = _char[0];
            return Encoding.UTF8.GetByteCount(chr.ToString());
        }

        public static bool isWide(string _char)
        {
            if (charWidth(_char) > 1)
                return true;
            else return false;
        }

        public static int len(string str)
        {
            return Encoding.UTF8.GetCharCount(Encoding.UTF8.GetBytes(str));
        }

        public static string lower(string str)
        {
            return str.ToLower();
        }

        public static string reverse(string str)
        {
            return new string(str.Reverse().ToArray());
        }

        public static string sub(string str, int start)
        {
            int startI;
            if (start < 0)
                startI = str.Length + start;
            else startI = start - 1;
            start = Math.Max(0, startI);
            if (start > str.Length)
                start = str.Length;
            return str.Substring(start);
        }

        public static string sub(string str, int start, int end)
        {
            int startI;
            if (start < 0)
                startI = str.Length + start;
            else startI = start - 1;
            start = Math.Max(0, startI);
            int endI;
            if (end < 0)
                endI = str.Length + end;
            else endI = end;
            end = Math.Min(str.Length, endI);
            int subStrLen = end - start;
            string subStr = str.Substring(start, subStrLen);
            return subStr;
        }

        public static string upper(string str)
        {
            return str.ToUpper();
        }

        public static int wlen(string str)
        {
            return Encoding.UTF8.GetBytes(str).Length;
        }

        public static string wtrunc(string str, int count)
        {
            return str;
        }
    }
}
