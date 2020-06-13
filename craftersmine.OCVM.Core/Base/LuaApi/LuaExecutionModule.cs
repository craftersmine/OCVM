using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Base.LuaApi.OpenComputers;
using NLua;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public sealed class LuaExecutionModule
    {
        //private int maxRamForLoad;
        //private readonly Script env;
        private readonly Lua env;

        public LuaExecutionModule(int maxRamAvailable)
        {
            env = new Lua();
            env.LoadCLRPackage();
            RegisterGlobals();
            RegisterModules();
        }

        private void PrintException(Exception e)
        {
            VM.RunningVM.Display.SetColor(BaseColors.Black, BaseColors.Red);
            Root.print(e.Source + e.Message);
            VM.RunningVM.Display.SetColor(BaseColors.Black, BaseColors.White);
        }

        public async Task<object[]> ExecuteFile(string path)
        {
            //env.Options.ScriptLoader = new FileSystemScriptLoader() { IgnoreLuaPathGlobal = true, ModulePaths = new string[] { Path.GetDirectoryName(path) } };
            //await env.DoFileAsync(path);
            return await Task<object[]>.Run(new Func<object[]>(() => {
                try
                {
                    return env.DoFile(path);
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                    return null;
                }
            }));
        }

        public async Task<object[]> ExecuteString(string str, string chunkName = "mainChunk")
        {
            return await Task<object[]>.Run(new Func<object[]>(() => {
                try
                {
                    return env.DoString(str, chunkName);
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                    return null;
                }
            }));
        }

        public void RegisterGlobals()
        {
            //env.Globals["print"] = (Action<string>)Root.print;
            env.RegisterFunction("print", typeof(Root).GetMethod("print"));
            env.RegisterFunction("breakpoint", typeof(Root).GetMethod("breakpoint"));
        }

        public void RegisterModules()
        {
            env["component"] = new Component();
        }

        public LuaTable CreateTable()
        {
            env.NewTable("tempTable");
            LuaTable table = env["tempTable"] as LuaTable;
            env["tempTable"] = null;
            return table;
        }
    }
}
