using craftersmine.OCVM.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NLua;
using craftersmine.OCVM.Core.Base.LuaApi;
using System.Reflection.Emit;

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
        private FileSystem() : base() { }
        private FileSystem(string hostFolderPath) : base()
        {
            this.hostFolderPath = hostFolderPath;
        }

        public void RestoreFileSystem()
        {
            string[] metadataToRestore = {
                "version=1",
                "filesystem.address=" + Address
            };

            if (!Directory.Exists(HostFolderPath))
                Directory.CreateDirectory(HostFolderPath);
            if (!Directory.Exists(FileSystemRootPath))
                Directory.CreateDirectory(FileSystemRootPath);
            if (!File.Exists(MetadataFilePath))
                File.WriteAllLines(MetadataFilePath, metadataToRestore);
            else
            {
                bool isNeededRestore = false;
                string[] existantData = File.ReadAllLines(MetadataFilePath);
                foreach (var ln in existantData)
                {
                    string[] separated = ln.Split('=');
                    if (separated.Length > 1)
                        switch (separated[0])
                        {
                            case "version":
                                if (separated[1] != "1")
                                    isNeededRestore = true;
                                break;
                            case "filesystem.address":
                                if (separated[1] != Address)
                                    isNeededRestore = true;
                                break;
                        }
                }
                if (isNeededRestore)
                    File.WriteAllLines(MetadataFilePath, metadataToRestore);
            }
        }

        public static FileSystem MountFileSystem(string hostFolderPath)
        {
            FileSystem fs = new FileSystem(hostFolderPath);
            fs.RestoreFileSystem();
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

        #region Lua Callbacks
        
        [LuaCallback(IsDirect = true)]
        public object[] open(string path, string mode)
        {
            object[] result = new object[2];

            if (IsFileExists(path))
            {
                LuaTable h = VM.RunningVM.ExecModule.CreateTable();
                LuaTable fs = VM.RunningVM.ExecModule.CreateTable();
                FileSystemHandle fsHandle = new FileSystemHandle();
                handles.Add(fsHandle.HandleId, fsHandle);

                h["handle"] = fsHandle.HandleId;

                result[0] = h;
            }
            else
            {
                result[0] = null;
                result[1] = OCErrors.FileNotFound;
            }

            return result;
        }

        public void close(int handle)
        {
            if (handles.ContainsKey(handle))
            {
                handles[handle].IsClosed = true;
                handles.Remove(handle);
            }
        }

        #endregion
    }

    public sealed class FileSystemHandle
    {
        public int HandleId { get; set; }
        public string HostFilePath { get; set; }
        public FileSystem ParentFS { get; set; }
        public bool IsClosed { get; set; }
        public FileSystemHandle()
        {
            HandleId = new Random().Next(100000000, int.MaxValue);
        }
    }
}
