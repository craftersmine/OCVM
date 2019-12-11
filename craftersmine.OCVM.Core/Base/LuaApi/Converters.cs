using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public static class Converters
    {
        public static Dictionary<object, object> ToDictionary(this Table table)
        {
            Dictionary<object, object> arr = new Dictionary<object, object>();

            foreach (var entry in table.Pairs)
            {
                arr.Add(ConvertType(entry.Key), ConvertType(entry.Value));
            }

            return arr;
        }

        public static object ConvertType(DynValue entry)
        {
            switch (entry.Type)
            {
                case DataType.Number:
                    return entry.Number;
                case DataType.String:
                    return entry.CastToString();
                case DataType.Table:
                    return entry.Table.ToDictionary();
                case DataType.Void:
                case DataType.Nil:
                    return null;
                case DataType.Boolean:
                    return entry.Boolean;
                default:
                    return null;
            }
        }
    }
}
