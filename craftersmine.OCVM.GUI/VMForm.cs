using craftersmine.OCVM.Core;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Base.LuaApi;
using craftersmine.OCVM.Core.MachineComponents;
using craftersmine.OCVM.GUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace craftersmine.OCVM.GUI
{
    public partial class VMForm : Form
    {
        private TimeSpan frameTime = TimeSpan.Zero;
        private Dictionary<string, StatusIcon> statusIcons = new Dictionary<string, StatusIcon>();
        private Timer resetTimer = new Timer();

        public VMForm(Tier displayTier)
        {
            InitializeComponent();
            resetTimer.Interval = 20;
            resetTimer.Tick += ResetTimer_Tick;
            display1.SetTier(displayTier);
            LuaApi.DisplayOutput += LuaApi_DisplayOutput;
            LuaApi.DisplayScroll += LuaApi_DisplayScroll;
            LuaApi.DisplayCursorPositionChange += LuaApi_DisplayCursorPositionChange;
            display1.DisplayRedrawn += Display1_DisplayRedrawn;
            display1.EnableCursor = true;
            VM vm = new VM();
            VMEvents.DiskActivity += VMEvents_DiskActivity;
            VMEvents.VMReady += VMEvents_VMLaunched;
            VMEvents.VMStateChanged += VMEvents_VMStateChanged;
            vm.Initialize(display1);
        }

        private void VMEvents_VMStateChanged(object sender, VMStateChangedEventArgs e)
        {
            if (e.State == VMState.Rebooting || e.State == VMState.Stopped || e.State == VMState.Stopping)
                display1.ClearScreenBuffer();
        }

        private void VMEvents_VMLaunched(object sender, EventArgs e)
        {
            CreateStatusIcons();
            resetTimer.Start();
            VM.RunningVM.Run();
        }

        private void CreateStatusIcons()
        {
            var fs = VM.RunningVM.DeviceBus.GetDevicesByType("filesystem", false);
            foreach (var fse in fs)
            {
                CreateStatusIcon(new StatusIcon(fse.Address, Resources.hdd_idle));
                statusIcons[fse.Address].Tooltip = "Mounted Filesystem:\r\nAddress: " + fse.Address + "\r\nHost path: " + ((FileSystem)fse).HostFolderPath;
                statusIcons[fse.Address].AddImage("read", Resources.hdd_read);
                statusIcons[fse.Address].AddImage("write", Resources.hdd_write);
            }
            PutIcons();
        }

        private void ResetTimer_Tick(object sender, EventArgs e)
        {
            foreach (var icon in statusIcons)
            {
                ShowStatusIcon(icon.Key, "default");
            }
        }

        private void VMEvents_DiskActivity(object sender, DiskActivityEventArgs e)
        {
            switch (e.DiskActivityType)
            {
                case DiskActivityType.Read:
                    ShowStatusIcon(e.FileSystemAddress, "read");
                    break;
                case DiskActivityType.Write:
                    ShowStatusIcon(e.FileSystemAddress, "write");
                    break;
                default:
                    ShowStatusIcon(e.FileSystemAddress, "default");
                    break;
            }
        }

        private void Display1_DisplayRedrawn(object sender, DisplayRedrawnEventArgs e)
        {
            status.Text = e.DrawTime.TotalMilliseconds.ToString();
        }

        private void LuaApi_DisplayCursorPositionChange(object sender, DisplayCursorPositionEventArgs e)
        {
            display1.SetCursorPosition(e.X, e.Y);
        }

        private void LuaApi_DisplayScroll(object sender, EventArgs e)
        {
            display1.ScrollScreenBuffer();
        }

        private void LuaApi_DisplayOutput(object sender, DisplayOutputEventArgs e)
        {
            if (e.UseDefaultColors)
                display1.PlaceString(e.Position.X, e.Position.Y, e.StringValue, display1.ForeColor, display1.BackColor);
            else display1.PlaceString(e.Position.X, e.Position.Y, e.StringValue, e.Foreground, e.Background);
            display1.Redraw();
        }

        private void Display1_SizeChanged(object sender, EventArgs e)
        {
            //this.Size = display1.ClientSize;
        }

        Tier t = Tier.Base;

        private void Button1_Click(object sender, EventArgs e)
        {
            switch (t)
            {
                case Tier.Base:
                    t = Tier.Medium;
                    display1.SetTier(t);
                    break;
                case Tier.Medium:
                    t = Tier.Base;
                    display1.SetTier(t);
                    break;
                case Tier.Advanced:
                    t = Tier.Base;
                    display1.SetTier(t);
                    break;
            }
        }

        int curPosX = 0;
        int curPosY = 0;

        private void VMForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                if (curPosX < display1.DisplayWidth)
                {
                    display1.SetScreenBufferData(curPosX, curPosY, new DisplayChar(e.KeyChar, display1.ForeColor, display1.BackColor));
                    curPosX++;
                }
                else
                {
                    curPosX = 0;
                    curPosY++;
                    if (curPosY > display1.DisplayHeight)
                        curPosY = 0;
                }
            }
            else
            {
                if (e.KeyChar == (char)Keys.Back)
                {
                    if (curPosX >= 0 && curPosX <= display1.DisplayWidth)
                    {
                        display1.SetScreenBufferData(curPosX, curPosY, new DisplayChar(' ', display1.ForeColor, display1.BackColor));
                        curPosX--;
                    }
                    else
                    {
                        curPosX = 0;
                        curPosY--;
                        if (curPosY <= 0)
                            curPosY = 0;
                    }
                }
            }
            display1.Redraw();
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            VM.RunningVM.Stop(true);
        }

        private void Shutdown_Click(object sender, EventArgs e)
        {
            VM.RunningVM.Stop(false);
        }

        int count = 0;

        private void Configure_Click(object sender, EventArgs e)
        {
            
        }

        private void aboutMenu_Click(object sender, EventArgs e)
        {

        }

        public void ShowStatusIcon(string iconId, string imageId)
        {
            if (statusIcons.ContainsKey(iconId))
                if (statusIcons[iconId].ImageList.ContainsKey(imageId))
                    if (InvokeRequired)
                        Invoke(new Action(() => { statusIcons[iconId].ToolStripStatusLabel.Image = statusIcons[iconId].ImageList[imageId]; }));
                    else { statusIcons[iconId].ToolStripStatusLabel.Image = statusIcons[iconId].ImageList[imageId]; }
        }

        public void PutIcons()
        {
            foreach (var icon in statusIcons)
            {
                statusStrip1.Items.Add(icon.Value.ToolStripStatusLabel);
            }
        }

        public ToolStripStatusLabel CreateStatusIcon(StatusIcon statusIcon)
        {
            if (!statusIcons.ContainsKey(statusIcon.Id))
                statusIcons.Add(statusIcon.Id, statusIcon);
            else throw new Exception("Status icon exists! " + statusIcon.Id);
            return statusIcon.ToolStripStatusLabel;
        }
    }
}
