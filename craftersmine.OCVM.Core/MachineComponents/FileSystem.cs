using craftersmine.OCVM.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLua;
using craftersmine.OCVM.Core.Base.LuaApi;
using craftersmine.OCVM.Core.Extensions;
using System.Security;
using craftersmine.OCVM.Core.Exceptions;

namespace craftersmine.OCVM.Core.MachineComponents
{
    [OpenComputersComponent(ComponentType = "filesystem")]
    public sealed class FileSystem : BaseComponent
    {
        private string hostFolderPath = "";
        private Dictionary<int, FileSystemHandle> handles = new Dictionary<int, FileSystemHandle>();
        public string HostFolderPath { get { return hostFolderPath; } }
        public string FileSystemRootPath { get { return Path.Combine(hostFolderPath, "storage\\"); } }
        public string MetadataFilePath { get { return Path.Combine(hostFolderPath, "ocvm.filesystem.metadata"); } }
        public string FSLabel { get; private set; } = "";
        public object IsReadOnly { get; private set; } = false;

        private FileSystem() : base() 
        {
        }
        private FileSystem(string hostFolderPath) : base()
        {
            this.hostFolderPath = hostFolderPath;
            var drv = new DriveInfo(HostFolderPath.Substring(0, 2));
            if (drv != null)
            {
                DeviceInfo.Capacity = drv.TotalSize.ToString();
                DeviceInfo.Size = drv.AvailableFreeSpace.ToString();
            }
            else
            {
                DeviceInfo.Capacity = "N/A";
                DeviceInfo.Size = "N/A";
            }
            DeviceInfo.Product = "craftersmine OCVM Virtual Filesystem";
            DeviceInfo.Version = VM.CurrentVersion.ToString();
            DeviceInfo.Vendor = DeviceInfo.DefaultVendor;
            DeviceInfo.Clock = "0/0/0";
            DeviceInfo.Description = "Filesystem";
        }

        public void SaveMeta()
        {
            Logger.Instance.Log(LogEntryType.Info, "Saving filesystem metadata... (" + Address + " | " + FSLabel + ")");
            string[] metadataToRestore = {
                "version=1",
                "filesystem.address=" + Address,
                "filesystem.label=" + FSLabel,
                "filesystem.isReadOnly=" + IsReadOnly.ToString()
            }; 
            File.WriteAllLines(MetadataFilePath, metadataToRestore);
        }

        public string RestoreFileSystem()
        {
            Logger.Instance.Log(LogEntryType.Info, "Restoring filesystem metadata... (" + Address + " | " + FSLabel + ")");
            string[] metadataToRestore = {
                "version=1",
                "filesystem.address=" + Address,
                "filesystem.label=" + FSLabel,
                "filesystem.isReadOnly=" + IsReadOnly.ToString()
            };

            string retrievedAddress = "";

            if (!Directory.Exists(HostFolderPath))
                Directory.CreateDirectory(HostFolderPath);
            if (!Directory.Exists(FileSystemRootPath))
                Directory.CreateDirectory(FileSystemRootPath);
            if (!File.Exists(MetadataFilePath))
            {
                File.WriteAllLines(MetadataFilePath, metadataToRestore);
                return Address;
            }
            else
            {
                bool isNeededRestore = false;
                bool isLabelExists = false;
                string[] existantData = File.ReadAllLines(MetadataFilePath);
                foreach (var ln in existantData)
                {
                    string[] separated = ln.Split('=');
                    if (separated.Length > 1)
                    {
                        if (separated[0] != "filesystem.label")
                        {
                            switch (separated[0])
                            {
                                case "version":
                                    if (separated[1] != "1")
                                        isNeededRestore = true;
                                    break;
                                case "filesystem.address":
                                    if (!Guid.TryParse(separated[1], out Guid addr))
                                        isNeededRestore = true;
                                    else retrievedAddress = separated[1];
                                    break;
                                case "filesystem.isReadOnly":
                                    if (!bool.TryParse(separated[1], out bool isRO))
                                        isNeededRestore = true;
                                    else IsReadOnly = isRO;
                                    break;
                            }
                        }
                        else
                        {
                            FSLabel = separated[1];
                            isLabelExists = true;
                        }
                    }
                    else isNeededRestore = true;
                }
                if (isNeededRestore || !isLabelExists)
                {
                    Logger.Instance.Log(LogEntryType.Warning, "Restoring corrupted filesystem metadata... (" + Address + " | " + FSLabel + ")");
                    File.WriteAllLines(MetadataFilePath, metadataToRestore);
                    return Address;
                }
                else
                    return retrievedAddress;
            }
        }

        public static FileSystem MountFileSystem(string hostFolderPath)
        {
            Logger.Instance.Log(LogEntryType.Info, "Mounting filesystem at \"" + hostFolderPath + "\"...");
            FileSystem fs = new FileSystem(hostFolderPath);
            var restoredAddress = fs.RestoreFileSystem();
            if (!restoredAddress.IsNullEmptyOrWhitespace())
                fs.Address = restoredAddress;
            Logger.Instance.Log(LogEntryType.Info, "Mounted filesystem address " + fs.Address);
            Logger.Instance.Log(LogEntryType.Info, "FS host path: \"" + fs.HostFolderPath + "\"");
            return fs;
        }

        public string GetPath(string ocPath)
        {
            if (ocPath.StartsWith("/"))
                ocPath = ocPath.Substring(1);
            ocPath = ocPath.Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(FileSystemRootPath, ocPath);
        }

        public bool IsFileExists(string ocPath)
        {
            VMEvents.OnDiskActivity(this.Address, DiskActivityType.Read);
            return File.Exists(GetPath(ocPath));
        }

        public void CloseHandle(FileSystemHandle handle)
        {
            if (handles.ContainsKey(handle.HandleId))
            {
                handle.IsClosed = true;
                handle.FileStream.Flush();
                handle.FileStream.Close();
                Logger.Instance.Log(LogEntryType.Info, "Closed handle " + handle.HandleId + " \"" + handle.HostFilePath + "\"");
                handles.Remove(handle.HandleId);
            }
        }

        public void CloseHandles()
        {
            Logger.Instance.Log(LogEntryType.Info, "Closing filesystem handles...");
            for (int i = 0; i < handles.Count; i++)
            {
                try
                {
                    CloseHandle(handles.ElementAt(i).Value);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(LogEntryType.Warning, "Unable to close handle! Is it was closed before?");
                    Logger.Instance.LogException(LogEntryType.Warning, ex);
                }
            }
        }

        public FileSystemHandleMode ParseMode(string mode)
        {
            if (!mode.IsNullEmptyOrWhitespace())
            {
                mode = mode.ToLower();
                if ((mode == "r") || mode == "rb")
                    return FileSystemHandleMode.Read;
                if ((mode == "w") || mode == "wb")
                    return FileSystemHandleMode.Write;
                if ((mode == "a") || mode == "ab")
                    return FileSystemHandleMode.Append;
                else return FileSystemHandleMode.Unsupported;
            }
            else return FileSystemHandleMode.Read;
        }

        #region Lua Callbacks

        [LuaCallback(IsDirect = true)]
        public object[] open(string path, string mode)
        {
            object[] result = new object[2];
            var m = ParseMode(mode);
            if (m == FileSystemHandleMode.Unsupported)
            {
                result[0] = null;
                result[1] = OCErrors.UnsupportedMode;
                return result;
            }

            if (IsFileExists(path))
            {
                LuaTable h = VM.RunningVM.ExecModule.CreateTable();
                FileSystemHandle fsHandle = new FileSystemHandle(GetPath(path), m);
                fsHandle.HandleId += fsHandle.HostFilePath.GetHashCode();
                fsHandle.ParentFS = this;
                if (handles.ContainsKey(fsHandle.HandleId))
                    fsHandle.HandleId += VM.RunningVM.ExecModule.Random.Next(0x0, 0x7fffff);

                handles.Add(fsHandle.HandleId, fsHandle);

                result[0] = fsHandle;
                VMEvents.OnDiskActivity(this.Address, DiskActivityType.Read);
            }
            else
            {
                result[0] = null;
                result[1] = OCErrors.FileNotFound;
            }

            return result;
        }

        [LuaCallback(IsDirect = true)]
        public string read(FileSystemHandle handle, double length)
        {
            if (handle == null)
                throw new OpenComputersException(OCErrors.BadFileDescriptor); ;
            return handle.read((int)length);
        }

        [LuaCallback(IsDirect = true)]
        public bool write(FileSystemHandle handle, string data)
        {
            if (handle == null)
                throw new OpenComputersException(OCErrors.BadFileDescriptor);
            return handle.write(data);
        }

        [LuaCallback(IsDirect = true)]
        public long seek(FileSystemHandle handle, string whence, int offset)
        {
            return handle.seek(whence, offset);
        }

        [LuaCallback(IsDirect = true)]
        public void close(FileSystemHandle handle)
        {
            handle.close();
        }

        [LuaCallback(IsDirect = true)]
        public string getLabel()
        {
            if (FSLabel.IsNullEmptyOrWhitespace())
                return null;
            return FSLabel;
        }

        [LuaCallback(IsDirect = true)]
        public string setLabel(string value)
        {
            FSLabel = value;
            if (value == null)
                FSLabel = value;
            else FSLabel = value;
            SaveMeta();
            return getLabel();
        }

        [LuaCallback(IsDirect = true)]
        public long spaceUsed()
        {
            DriveInfo drive = new DriveInfo(HostFolderPath[0].ToString());
            return drive.AvailableFreeSpace;
        }

        [LuaCallback(IsDirect = true)]
        public long spaceTotal()
        {
            DriveInfo drive = new DriveInfo(HostFolderPath[0].ToString());
            return drive.TotalSize;
        }

        [LuaCallback(IsDirect = true)]
        public bool makeDirectory(string path)
        {
            string p = GetPath(path);
            try
            {
                var dir = Directory.CreateDirectory(p);
                if (dir != null)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        [LuaCallback(IsDirect = true)]
        public bool exists(string path)
        {
            if (isDirectory(path))
                return true;
            else
            {
                return File.Exists(GetPath(path));
            }
        }

        [LuaCallback(IsDirect = true)]
        public bool isDirectory(string path)
        {
            try
            {
                var p = GetPath(path);
                FileAttributes attributes = File.GetAttributes(p);
                if (attributes.HasFlag(FileAttributes.Directory))
                    return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        [LuaCallback(IsDirect = true)]
        public bool rename(string from, string to)
        {
            try
            {
                if (exists(from))
                {
                    File.Move(GetPath(from), GetPath(to));
                    if (exists(to))
                        return true;
                    else return false;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        [LuaCallback(IsDirect = true)]
        public LuaTable list(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(GetPath(path));
            var files = dirInfo.GetFiles();
            var dirs = dirInfo.GetDirectories();
            List<string> objects = new List<string>();

            LuaTable table = VM.RunningVM.ExecModule.CreateTable();
            foreach (var file in files)
            {
                objects.Add(file.Name);
            }
            foreach (var dir in dirs)
            {
                objects.Add(dir.Name);
            }
            objects.Sort();
            return objects.ToLuaTable();
        }

        [LuaCallback(IsDirect = true)]
        public int lastModified(string path)
        {
            if (exists(path))
                return File.GetLastWriteTimeUtc(GetPath(path)).Second;
            return 0;
        }

        [LuaCallback(IsDirect = true)]
        public bool remove(string path)
        {
            if (exists(path))
            {
                File.Delete(GetPath(path));
                if (!exists(path))
                    return true;
                else return false;
            }
            else return false;
        }

        [LuaCallback(IsDirect = true)]
        public long size(string path)
        {
            if (exists(path))
                return new FileInfo(GetPath(path)).Length;
            else return 0;
        }

        #endregion
    }

    public sealed class FileSystemHandle
    {
        internal int HandleId { get; set; }
        public int handle { get { return HandleId; } }
        internal FileSystemHandleMode Mode { get; set; }
        internal string HostFilePath { get; set; }
        internal FileStream FileStream { get; set; }
        internal FileSystem ParentFS { get; set; }
        internal bool IsClosed { get; set; }
        internal FileSystemHandle HandleInstance { get; set; }

        internal FileSystemHandle(string hostFilepath, FileSystemHandleMode mode)
        {
            HostFilePath = hostFilepath;
            Mode = mode;
            HandleId = new Random().Next(100000000, int.MaxValue);
            FileStream = new FileStream(HostFilePath, FileMode.OpenOrCreate, (FileAccess)Mode);
            HandleInstance = this;
        }

        public void close()
        {
            this.close(this);
        }

        public void close(object handle)
        {
            object[] result = new object[2];
            if (handle.GetType() == typeof(FileSystemHandle))
                ((FileSystemHandle)handle).ParentFS.CloseHandle((FileSystemHandle)handle);
        }

        public string read(double length)
        {
            int len = (int)length;
            if (len == int.MinValue)
                len = int.MaxValue;
            if (Mode.HasFlag(FileSystemHandleMode.Write) || Mode.HasFlag(FileSystemHandleMode.Append))
                throw new OpenComputersException(OCErrors.BadFileDescriptor);
            else
            {
                try
                {
                    if (len > FileStream.Length)
                        len = Convert.ToInt32(FileStream.Length);
                    string data = "";
                    if (FileStream.Position == FileStream.Length)
                        return null;
                    if (len > FileStream.Length - FileStream.Position)
                        len = (int)FileStream.Length - (int)FileStream.Position;
                    for (int i = 0; i < len; i++)
                    {
                        data += Convert.ToChar((byte)FileStream.ReadByte());
                        VMEvents.OnDiskActivity(ParentFS.Address, DiskActivityType.Read);
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    if (ex is SecurityException || ex is AccessViolationException)
                        throw new OpenComputersException(OCErrors.PermissionDenied);
                    else
                        throw new OpenComputersException(OCErrors.BadFileDescriptor);
                }
            }
        }

        public bool write(string value)
        {
            if (Mode == FileSystemHandleMode.Read)
                throw new OpenComputersException(OCErrors.BadFileDescriptor);
            else
            {
                try
                {
                    var data = value.GetBytes();
                    if (Mode == FileSystemHandleMode.Write)
                    {
                        FileStream.Position = 0;
                        for (int i = 0; i < data.Length; i++)
                        {
                            FileStream.WriteByte(data[i]);
                            VMEvents.OnDiskActivity(ParentFS.Address, DiskActivityType.Write);
                        }
                    }
                    if (Mode == FileSystemHandleMode.Append)
                    {
                        FileStream.Position = FileStream.Length;
                        for (int i = 0; i < data.Length; i++)
                        {
                            FileStream.WriteByte(data[i]);
                            VMEvents.OnDiskActivity(ParentFS.Address, DiskActivityType.Write);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex is SecurityException || ex is AccessViolationException)
                        throw new OpenComputersException(OCErrors.PermissionDenied);
                    else
                        throw new OpenComputersException(OCErrors.BadFileDescriptor);
                }
            }
        }

        public long seek(string type)
        {
            return seek(type, 0);
        }

        public long seek(string type, int offset)
        {
            long value = 0;
            type = type.ToLower();
            try
            {
                switch (type)
                {
                    case "cur":
                        value = FileStream.Seek(offset, SeekOrigin.Current);
                        return value;
                    case "set":
                        value = FileStream.Seek(offset, SeekOrigin.Begin);
                        return value;
                    case "end":
                        value = FileStream.Seek(offset, SeekOrigin.End);
                        return value;
                }
            }
            catch (Exception ex)
            {
                throw new OpenComputersException(ex.Message);
            }
            throw new OpenComputersException(OCErrors.BadFileDescriptor);
        }
    }

    public enum FileSystemHandleMode
    {
        Read = FileAccess.Read, Write = FileAccess.Write, ReadWrite = FileAccess.ReadWrite, Append = 8, Unsupported = 0
    }
}
