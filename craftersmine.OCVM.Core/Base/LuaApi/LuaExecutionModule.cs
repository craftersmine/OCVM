using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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

        public Random Random { get; private set; }

        public LuaExecutionModule(int maxRamAvailable)
        {
            Random = new Random();
            env = new Lua();
            env.UseTraceback = true;
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
                    str = "import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.MachineComponents');local component = require('component');local computer = require('computer');\r\n" + str;
                    var code = env.LoadString(str, chunkName);
                    return code.Call();
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                    return null;
                }
            }));
        }

        public LuaTable SetMetatable(LuaTable table, LuaTable metatable)
        {
            var setMeta = env.GetFunction("setmetatable");
            var res = setMeta.Call(table, metatable);
            return res[0] as LuaTable;
        }

        public LuaTable GetMetatable(LuaTable table)
        {
            var setMeta = env.GetFunction("getmetatable");
            var res = setMeta.Call(table);
            return res[0] as LuaTable;
        }

        public LuaFunction CreateLuaFunction(string name, string body)
        {
            string func = name + " = function(...) " + body + " end; return " + name;
            var fnc = env.DoString(func)[0];
            return (LuaFunction)fnc;
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
