using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Base.LuaApi.OpenComputers;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.IO;
using MoonSharp.VsCodeDebugger;

namespace craftersmine.OCVM.Core.Base.LuaApi
{
    public sealed class LuaExecutionModule
    {
        //private int maxRamForLoad;
        private readonly Script env;
        private MoonSharpVsCodeDebugServer svc;

        public LuaExecutionModule(int maxRamAvailable)
        {
            env = new Script();
            svc = new MoonSharpVsCodeDebugServer();
            RegisterGlobals();
            RegisterModules();
            svc.Start();
            svc.AttachToScript(env, "mainChunk");
        }

        public async void ExecuteFile(string path)
        {
            env.Options.ScriptLoader = new FileSystemScriptLoader() { IgnoreLuaPathGlobal = true, ModulePaths = new string[] { Path.GetDirectoryName(path) } };
            //await env.DoFileAsync(path);
        }

        public void ExecuteString(string str, string chunkName = "mainChunk")
        {
            env.DoString(str, env.Globals, chunkName);
            //svc.Detach(env);
        }

        public void RegisterGlobals()
        {
            env.Globals["print"] = (Action<string>)Root.print;
        }

        public void RegisterModules()
        {
            env.Globals.RegisterModuleType<Component>();
        }
    }
}
