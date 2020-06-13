using System;
using System.Collections.Generic;
using System.Linq;
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
