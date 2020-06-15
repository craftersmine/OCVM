using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core
{
    public sealed class VMEvents
    {
        public static event EventHandler<DiskActivityEventArgs> DiskActivity;

        public static void OnDiskActivity(string fsAddress, DiskActivityType activityType)
        {
            DiskActivity?.Invoke(null, new DiskActivityEventArgs() { FileSystemAddress = fsAddress, DiskActivityType = activityType });
        }
    }

    public sealed class DiskActivityEventArgs : EventArgs
    {
        public DiskActivityType DiskActivityType { get; set; }
        public string FileSystemAddress { get; set; }
    }

    public enum DiskActivityType
    {
        Read, Write, Generic = 0
    }
}
