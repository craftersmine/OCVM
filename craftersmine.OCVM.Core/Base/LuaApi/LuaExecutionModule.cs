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
        private object locker = new object();

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
            VM.RunningVM.Display.SetColor(BaseColors.Blue, BaseColors.White);
            VM.RunningVM.Display.ClearScreenBuffer();
            Console.ForegroundColor = ConsoleColor.Red;
            string uerr = "Unrecoverable error";
            int xPos1 = (VM.RunningVM.Display.DisplayWidth / 2) - (uerr.Length / 2);
            int yPos1 = (VM.RunningVM.Display.DisplayHeight / 2) - 3;
            VM.RunningVM.Display.PlaceString(xPos1, yPos1, uerr, VM.RunningVM.Display.ForeColor, VM.RunningVM.Display.BackColor);
            if (e.Message.Length > VM.RunningVM.Display.DisplayWidth)
            {
                for (int i = 0; i < e.Message.Length / VM.RunningVM.Display.DisplayWidth; i++)
                {
                    string tmpStr = e.Message.Substring(i * VM.RunningVM.Display.DisplayWidth, VM.RunningVM.Display.DisplayWidth);
                    int xPos2 = (VM.RunningVM.Display.DisplayWidth / 2) - (tmpStr.Length / 2);
                    int yPos2 = (VM.RunningVM.Display.DisplayHeight / 2) + 1;
                    VM.RunningVM.Display.PlaceString(xPos1, yPos1, tmpStr, VM.RunningVM.Display.ForeColor, VM.RunningVM.Display.BackColor);
                }
            }
            else
            {
                int xPos2 = (VM.RunningVM.Display.DisplayWidth / 2) - (e.Message.Length / 2);
                int yPos2 = (VM.RunningVM.Display.DisplayHeight / 2);
                VM.RunningVM.Display.PlaceString(xPos1, yPos1, e.Message, VM.RunningVM.Display.ForeColor, VM.RunningVM.Display.BackColor);
            }
            Console.WriteLine(e.Source);
            Console.WriteLine(e.Message);
            VM.RunningVM.Display.SetColor(BaseColors.Black, BaseColors.White);
            Console.ResetColor();
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

        public void Close()
        {
            lock (locker)
            {
                if (env.IsExecuting)
                    env.Close();
            }
        }

        public async Task<object[]> ExecuteString(string str, string chunkName = "mainChunk")
        {
            VMEvents.VMStateChanged += VMEvents_VMStateChanged;
            return await Task<object[]>.Run(new Func<object[]>(() => {
                try
                {
                    str = "import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.MachineComponents');local component = require('component');local computer = require('computer');local std = require('stdlib');local unicode = require('unicode');_G['computer'] = computer;_G['component'] = component;_G['unicode'] = unicode;_G['checkArg'] = std.checkArg;_G['dofile'] = nil;\r\n" + str;
                    var code = env.LoadString(str, chunkName);
                    return code.Call();
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                    if (ex.Message.Contains(OCErrors.NoBootableMediumFound))
                        SoundGenerator.BeepMorse("--");
                    return null;
                }
            }));
        }

        private void VMEvents_VMStateChanged(object sender, VMStateChangedEventArgs e)
        {
            if (e.State == VMState.Stopped || e.State == VMState.Rebooting || e.State == VMState.Stopping)
                Close();
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
            env.RegisterFunction("checkArgType", typeof(Root).GetMethod("checkArgType"));
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
