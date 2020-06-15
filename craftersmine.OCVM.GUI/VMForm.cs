using craftersmine.OCVM.Core;
using craftersmine.OCVM.Core.Base;
using craftersmine.OCVM.Core.Base.LuaApi;
using craftersmine.OCVM.Core.MachineComponents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace craftersmine.OCVM.GUI
{
    public partial class VMForm : Form
    {
        private TimeSpan frameTime = TimeSpan.Zero;
        private Dictionary<string, ToolStripStatusLabel> statusIcons = new Dictionary<string, ToolStripStatusLabel>();

        public VMForm(Tier displayTier)
        {
            InitializeComponent();
            display1.SetTier(displayTier);
            LuaApi.DisplayOutput += LuaApi_DisplayOutput;
            LuaApi.DisplayScroll += LuaApi_DisplayScroll;
            LuaApi.DisplayCursorPositionChange += LuaApi_DisplayCursorPositionChange;
            display1.DisplayRedrawn += Display1_DisplayRedrawn;
            display1.EnableCursor = true;
            new VM().Launch(display1);
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
            display1.ScrollScreenBuffer();
        }

        private void Shutdown_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < display1.DisplayHeight; y++)
                display1.PlaceString(0, y, "Scrolling test " + y, display1.ForeColor, display1.BackColor);
            display1.Redraw();
        }

        int count = 0;

        private async void Configure_Click(object sender, EventArgs e)
        {
            VM.RunningVM.ExecModule.ExecuteString(((EEPROM)VM.RunningVM.DeviceBus.GetPrimaryComponent("eeprom")).EEPROMCode);
        }

        private void aboutMenu_Click(object sender, EventArgs e)
        {
            CreateStatusIcon("test" + count++, Resources.configure);
        }

        public ToolStripStatusLabel CreateStatusIcon(string id, Image image)
        {
            ToolStripStatusLabel icon = new ToolStripStatusLabel(image);
            icon.Name = id;
            icon.Text = "";
            icon.DisplayStyle = ToolStripItemDisplayStyle.Image;
            if (!statusIcons.ContainsKey(id))
            {
                statusIcons.Add(id, icon);
                statusStrip1.Items.Add(icon);
            }
            else throw new Exception("Status icon exists! " + id);
            return icon;
        }
    }
}
