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
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < _args.Length; i++)
            {
                    bytes.Add(Convert.ToByte(_args[i]));
            }

            char[] chrs = Encoding.UTF8.GetChars(bytes.ToArray());

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
    }
}
