using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.OCVM.Core.Attributes;
using craftersmine.OCVM.Core.Exceptions;

namespace craftersmine.OCVM.Core.MachineComponents
{
    public sealed class DeviceBus
    {
        private readonly Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();
        private int lastSlot = 0;

        public int MaxBusLanes { get; private set; }
        public IComponent this[string address] { get { return GetDevice(address); } }

        public DeviceBus(int maxBusLanes)
        {
            MaxBusLanes = maxBusLanes;
        }

        public void ConnectDevice(IComponent component)
        {
            if (components.Count < MaxBusLanes)
            {
                if (!components.ContainsKey(component.Address))
                {
                    var devType = component.GetDeviceTypeName().ToLower();
                    bool isPrimaryAlreadyExists = false;
                    foreach (var primaryComp in GetPrimaryComponents())
                    {
                        if (primaryComp.GetDeviceTypeName() == component.GetDeviceTypeName())
                            if (primaryComp.IsPrimary)
                                isPrimaryAlreadyExists = true;
                    }
                    if (isPrimaryAlreadyExists)
                        component.IsPrimary = false;
                    else component.IsPrimary = true;
                    components.Add(component.Address, component);
                    component.Slot = lastSlot;
                    lastSlot++;
                }
            }
            else throw new ExcededBusLanesException("Max bus lanes on device bus is excedeed " + MaxBusLanes);
        }

        public IComponent GetDevice(string address)
        {
            foreach (var addr in components.Keys)
            {
                if (addr.StartsWith(address))
                    return components[addr];
                if (addr.Contains(address))
                    return components[addr];
                if (addr == address)
                    return components[addr];
            }

            if (components.ContainsKey(address))
                return components[address];
            else return null;
        }

        public IComponent[] GetPrimaryComponents()
        {
            List<IComponent> primaryComps = new List<IComponent>();
            foreach (var dev in components)
            {
                if (dev.Value.IsPrimary)
                    primaryComps.Add(dev.Value);
            }
            return primaryComps.ToArray();
        }

        public string[] GetAddresses()
        {
            return components.Keys.ToArray();
        }

        public IComponent[] GetDevicesByType(string type, bool exact)
        {
            List<IComponent> devices = new List<IComponent>(); 

            foreach (var dev in components.Values)
            {
                if (type != string.Empty && type != null)
                {
                    if (exact)
                    {
                        if (dev.GetComponentAttribute().ComponentType.ToLower() == type.ToLower())
                            devices.Add(dev);
                    }
                    else
                    {
                        if (dev.GetComponentAttribute().ComponentType.ToLower().Contains(type))
                            devices.Add(dev);
                    }
                }
                else devices.Add(dev);
            }

            return devices.ToArray();
        }

        public IComponent GetPrimaryComponent(string type)
        {
            foreach (var dev in components)
            {
                var deviceAttribute = dev.Value.GetComponentAttribute();
                if (deviceAttribute != null)
                {
                    if (deviceAttribute.ComponentType.ToLower() == type.ToLower())
                    {
                        if (dev.Value.IsPrimary)
                            return dev.Value;
                    }
                    else continue;
                }
                else continue;
            }
            return null;
        }
    }
}
