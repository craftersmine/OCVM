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
        private bool abort = false;

        public Random Random { get; private set; }

        public LuaExecutionModule(int maxRamAvailable)
        {
            Random = new Random();
            env = new Lua();
            env.UseTraceback = true;
            env.SetDebugHook(KeraLua.LuaHookMask.Line, 0);
            env.DebugHook += Env_DebugHook;
            env.LoadCLRPackage();
            RegisterGlobals();
            RegisterModules();
        }

        private void Env_DebugHook(object sender, NLua.Event.DebugHookEventArgs e)
        {
            Root.LogConsole("LUAEXEC: " + e.LuaDebug.CurrentLine + " SRC: " + e.LuaDebug.Name + " IN " + e.LuaDebug.ShortSource);
            if (abort)
            {
                Lua lState = (Lua)sender;
                lState.State.Error(OCErrors.MachineHalted);
            }
        }

        private void PrintException(Exception e)
        {
            var buffer = ScreenBufferManager.Instance.GetBuffer(0);
            buffer.Begin();
            buffer.ClearColor = BaseColors.Blue;
            buffer.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            string uerr = "Unrecoverable error";
            int xPos1 = (buffer.Width / 2) - (uerr.Length / 2);
            int yPos1 = (buffer.Height / 2) - 3;
            for (int i = 0; i < uerr.Length; i++)
            {
                buffer.Set(xPos1 + i, yPos1, uerr[i], BaseColors.White, BaseColors.Blue);
            }
            if (e.Message.Length > buffer.Width)
            {
                for (int i = 0; i <= e.Message.Length / buffer.Width; i++)
                {
                    string tmpStr = "";
                    if (e.Message.Substring(i * buffer.Width).Length >= buffer.Width)
                        tmpStr = e.Message.Substring(i * buffer.Width, buffer.Width);
                    else tmpStr = e.Message.Substring(i * buffer.Width);
                    int xPos2 = (buffer.Width / 2) - (tmpStr.Length / 2);
                    int yPos2 = (buffer.Height / 2) + i;
                    for (int j = 0; j < tmpStr.Length; j++)
                    {
                        buffer.Set(xPos2 + j, yPos2, tmpStr[j], BaseColors.White, BaseColors.Blue);
                    }
                }
            }
            else
            {
                for (int i = 0; i < e.Message.Length; i++)
                {
                    int xPos2 = (buffer.Width / 2) - (e.Message.Length / 2);
                    int yPos2 = (buffer.Height / 2);
                    buffer.Set(xPos2 + i, yPos2, e.Message[i], BaseColors.White, BaseColors.Blue);
                }
            }
            buffer.End();
            Console.WriteLine(e.Source);
            Console.WriteLine(e.Message);
            //VM.RunningVM.Display.SetColor(BaseColors.Black, BaseColors.White);
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
            abort = true;
        }

        public async Task<object[]> ExecuteString(string str, string chunkName = "mainChunk")
        {
            VMEvents.VMStateChanged += VMEvents_VMStateChanged;
            return await Task<object[]>.Run(new Func<object[]>(() => {
                try
                {
                    str = "import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.Base.LuaApi.OpenComputers');import('craftersmine.OCVM.Core', 'craftersmine.OCVM.Core.MachineComponents');local component = require('component');local computer = require('computer');local std = require('stdlib');local unicode = require('unicode');_G['computer'] = computer;_G['component'] = component;_G['unicode'] = unicode;_G['checkArg'] = std.checkArg;_G['dofile'] = nil;_G['loadfile'] = nil;\r\n" + str;
                    var code = env.LoadString(str, chunkName);
                    return code.Call();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains(OCErrors.NoBootableMediumFound))
                    {
                        PrintException(ex);
                        SoundGenerator.BeepMorse("--");
                    }
                    else if (ex.Message.Contains(OCErrors.MachineHalted))
                    {
                        var buffer = ScreenBufferManager.Instance.GetBuffer(0);
                        buffer.Begin();
                        buffer.ClearColor = BaseColors.Black;
                        buffer.Clear();
                        buffer.End();
                    }
                    else
                    {
                        PrintException(ex);
                        SoundGenerator.BeepMorse("..-");
                    }
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
