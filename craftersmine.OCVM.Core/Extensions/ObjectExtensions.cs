using craftersmine.OCVM.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static MethodInfo[] GetLuaCallbacks(this object obj)
        {
            Type objType = obj.GetType();
            var reflectedMethods = objType.GetMethods().Where(m => m.GetCustomAttributes(typeof(LuaCallbackAttribute), false).Length > 0).ToArray();
            return reflectedMethods;
        }

        public static MethodInfo GetLuaCallback(this object obj, string name)
        {
            Type objType = obj.GetType();
            var reflectedMethod = objType.GetMethod(name);
            var attr = reflectedMethod.GetCustomAttribute(typeof(LuaCallbackAttribute));
            if (attr != null)
                return reflectedMethod;
            return null;
        }
    }
}
