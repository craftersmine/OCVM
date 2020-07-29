using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace craftersmine.OCVM.Core
{
    public static class Settings
    {
        private static bool enableLuaLogging = false; 
        private static PrivateFontCollection pfc = new PrivateFontCollection();
        private static bool enableLuaLoggingToFile = false;

        public const int GpuMaxWidthConst = 160;
        public const int GpuMaxHeightConst = 50;

        public static bool CapScreenDepth { get; set; }
        public static bool EnableLuaLogging { get { return enableLuaLogging & EnableLogging; } set { enableLuaLogging = value; } }
        public static bool EnableLuaLoggingToFile { get { return EnableLuaLogging & enableLuaLoggingToFile; } set { enableLuaLoggingToFile = value; } }
        public static bool EnableLogging { get; set; }
        public static bool ShowFullErrorMessage { get; set; } = true;

        public static Font DisplayFont { get; private set; }
        public static int GpuMaxWidth { get; set; } = GpuMaxWidthConst;
        public static int GpuMaxHeight { get; set; } = GpuMaxHeightConst;

        public static void InitializeFont()
        {
            Logger.Instance.Log(LogEntryType.Info, "Initializing Unscii display font...");
            pfc.AddFontFile(Path.Combine(Application.StartupPath, "unscii-16-full.ttf"));
            DisplayFont = new Font(pfc.Families[0], 16f, FontStyle.Regular, GraphicsUnit.Pixel);
            Logger.Instance.Log(LogEntryType.Info, "Display font successfully initialized!");
        }
    }
}
