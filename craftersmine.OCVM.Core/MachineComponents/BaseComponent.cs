using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Exceptions;
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
        public DeviceInfo DeviceInfo { get; set; }

        public BaseComponent()
        {
            Address = Guid.NewGuid().ToString();
            DeviceInfo = new DeviceInfo();
        }

        public BaseComponent(string address) : base()
        {
            if (address != string.Empty)
                Address = address;
            DeviceInfo = new DeviceInfo();
        }

        public Dictionary<string, LuaMethodInfo> GetDeviceMethods()
        {
            Dictionary<string, LuaMethodInfo> methods = new Dictionary<string, LuaMethodInfo>();

            var methodsReflect = this.GetType().GetMethods().Where(m => m.GetCustomAttributes(typeof(LuaCallbackAttribute), false).Length > 0).ToArray();
            foreach(var m in methodsReflect)
            {
                string methodName = m.Name.Substring(0,1).ToLower() + m.Name.Substring(1);
                if (!methods.ContainsKey(methodName))
                {
                    LuaMethodInfo info = new LuaMethodInfo();
                    var attribute = m.GetCustomAttribute<LuaCallbackAttribute>();
                    info.Doc = attribute.Doc;
                    info.IsDirect = attribute.IsDirect;
                    info.IsGetter = attribute.IsGetter;
                    info.IsSetter = attribute.IsSetter;
                    methods.Add(methodName, info);
                }
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

        public OpenComputersComponentAttribute GetComponentAttribute()
        {
            OpenComputersComponentAttribute attribute = (OpenComputersComponentAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(OpenComputersComponentAttribute));
            if (attribute == null)
                throw new InvalidDeviceException(this.GetType().ToString() + " type is not valid device type! Did you forgot to add OpenComputersComponentAttribute?");
            return attribute;
        }
    }

    public static class DeviceExtentions
    {
        public static object InvokeMethod(this IComponent component, string method, params object[] args)
        {
            Type deviceType = component.GetType();
            
            if (deviceType.IsSubclassOf(typeof(BaseComponent)))
            {
                var reflectedMethods = deviceType.GetMethods().Where(m => m.GetCustomAttributes(typeof(LuaCallbackAttribute), false).Length > 0).ToArray();
                foreach(var reflectedMethod in reflectedMethods)
                {
                    if (reflectedMethod.Name == method)
                    {
                        var _params = reflectedMethod.GetParameters();
                        object[] _args = new object[(_params.Length - args.Length) + args.Length];
                        for (int i = 0; i < _args.Length; i++)
                            if (_params[i].HasDefaultValue)
                                _args[i] = _params[i].DefaultValue;
                        for (int i = 0; i < args.Length; i++)
                        {
                            _args[i] = args[i];
                        }

                        return reflectedMethod.Invoke(component, _args);
                    }
                }
            }

            return null;
        }

        public static string GetDeviceTypeName(this IComponent component)
        {
            var attribute = component.GetComponentAttribute();
            if (attribute != null)
                return attribute.ComponentType.ToLower();
            throw new InvalidDeviceException(component.GetType().ToString() + " type is not valid device type! Did you forgot to add OpenComputersComponentAttribute?");
        }

        public static string GenerateAddress()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
