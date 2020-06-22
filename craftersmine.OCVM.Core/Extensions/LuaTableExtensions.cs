using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace craftersmine.OCVM.Core.Extensions
{
    public static class LuaTableExtensions
    {
        public static int CountKeys(this LuaTable table)
        {
            return table.Keys.Count;
        }

        public static int CountValues(this LuaTable table)
        {
            return table.Values.Count;
        }

        public static List<object> GetKeysAsList(this LuaTable table)
        {
            List<object> list = new List<object>();
            foreach (var key in table.Keys)
                list.Add(key);
            return list;
        }

        public static List<object> GetValuesAsList(this LuaTable table)
        {
            List<object> list = new List<object>();
            foreach (var value in table.Values)
                list.Add(value);
            return list;
        }

        public static Dictionary<object, object> ToDictionary(this LuaTable table)
        {
            Dictionary<object, object> dict = new Dictionary<object, object>();
            foreach (var key in table.Keys)
            {
                dict.Add(key, table[key]);
            }
            return dict;
        }

        public static object[] GetKeysAsArray(this LuaTable table)
        {
            if (table != null)
                return table.GetKeysAsList().ToArray();
            else return null;
        }

        public static object[] GetValuesAsArray(this LuaTable table)
        {
            if (table != null)
                return table.GetValuesAsList().ToArray();
            else return null;
        }

        public static LuaTable ToLuaTable<T>(this List<T> list)
        {
            LuaTable table = VM.RunningVM.ExecModule.CreateTable();

            for (int i = 0; i < list.Count; i++)
            {
                table[i] = list[i];
            }

            return table;
        }
    }
}
