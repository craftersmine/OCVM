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
        public static event EventHandler VMReady;
        public static event EventHandler<VMStateChangedEventArgs> VMStateChanged;

        public static void OnVMReady()
        {
            VMReady?.Invoke(VM.RunningVM, EventArgs.Empty);
        }

        public static void OnVMStateChanged(VMState state)
        {
            VMStateChanged?.Invoke(VM.RunningVM, new VMStateChangedEventArgs() { State = state });
        }

        public static void OnDiskActivity(string fsAddress, DiskActivityType activityType)
        {
            DiskActivity?.Invoke(VM.RunningVM, new DiskActivityEventArgs() { FileSystemAddress = fsAddress, DiskActivityType = activityType });
        }
    }

    public sealed class DiskActivityEventArgs : EventArgs
    {
        public DiskActivityType DiskActivityType { get; set; }
        public string FileSystemAddress { get; set; }
    }

    public sealed class VMStateChangedEventArgs : EventArgs
    {
        public VMState State { get; set; }
    }

    public enum DiskActivityType
    {
        Read, Write, Generic = 0
    }
}
