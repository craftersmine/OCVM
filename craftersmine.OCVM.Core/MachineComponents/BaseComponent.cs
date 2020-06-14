using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.MachineComponents
{
    public class BaseComponent : IComponent
    {
        public string Address { get; set; }
        public bool IsPrimary { get; set; }
        public int Slot { get; set; }

        public BaseComponent()
        {
            Address = Guid.NewGuid().ToString();
        }

        public BaseComponent(string address) : base()
        {
            if (address != string.Empty)
                Address = address;
        }

        public Dictionary<string, LuaMethodInfo> GetDeviceMethods()
        {
            Dictionary<string, LuaMethodInfo> methods = new Dictionary<string, LuaMethodInfo>();

            var methodsReflect = this.GetType().GetMethods().Where(m => m.GetCustomAttributes(typeof(LuaCallbackAttribute), false).Length > 0).ToArray();
            foreach(var m in methodsReflect)
            {
                string methodName = m.Name.Substring(0,1).ToLower() + m.Name.Substring(1);
                LuaMethodInfo info = new LuaMethodInfo();
                var attribute = m.GetCustomAttribute<LuaCallbackAttribute>();
                info.Doc = attribute.Doc;
                info.IsDirect = attribute.IsDirect;
                info.IsGetter = attribute.IsGetter;
                info.IsSetter = attribute.IsSetter;
                methods.Add(methodName, info);
            }

            return methods;
        }

        public string GetDeviceMethodDoc(string method)
        {
            var m = this.GetType().GetMethod(method);
            if (m != null)
            {
                var attribute = m.GetCustomAttribute<LuaCallbackAttribute>();
                if (attribute != null)
                    return attribute.Doc;
            }
            return "";
        }
    }

    public static class DeviceExtentions
    {
        public static object InvokeMethod(this IComponent component, string method, params object[] args)
        {
            Type deviceType = component.GetType();
            
            if (deviceType.IsSubclassOf(typeof(BaseComponent)))
            {
                var reflectedMethod = deviceType.GetMethod(method);
                return reflectedMethod.Invoke(component, args);
            }

            return null;
        }

        public static string GetDeviceTypeName(this IComponent component)
        {
            Type deviceType = component.GetType();
            return deviceType.Name.ToLower();
        }

        public static string GenerateAddress()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
