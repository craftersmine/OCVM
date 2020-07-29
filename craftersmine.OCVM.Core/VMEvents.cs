using craftersmine.OCVM.Core.MachineComponents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core
{
    public sealed class VMEvents
    {
        public static event EventHandler<DiskActivityEventArgs> DiskActivity;
        public static event EventHandler ScreenStateChanged;
        public static event EventHandler VMReady;
        public static event EventHandler<VMStateChangedEventArgs> VMStateChanged;
        public static event EventHandler<GpuOperationEventArgs> GpuOperationRequested;

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

        public static void OnScreenStateChanged(Screen screen)
        {
            ScreenStateChanged?.Invoke(screen, EventArgs.Empty);
        }

        public static void OnGpuOperationRequested(GPU gpu, GpuOperation operation)
        {
            GpuOperationRequested?.Invoke(gpu, new GpuOperationEventArgs() { Operation = operation });
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

    public sealed class GpuOperationEventArgs : EventArgs
    {
        public GpuOperation Operation { get; set; }
        public Size Viewport { get; set; }
        public Size Resolution { get; set; }
    }

    public enum GpuOperation
    {
        SetResolution = 1, SetViewport = 2
    }

    public enum DiskActivityType
    {
        Read, Write, Generic = 0
    }
}
