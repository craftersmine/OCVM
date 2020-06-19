using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace craftersmine.OCVM.GUI
{
    public sealed class StatusIcon
    {
        public ToolStripStatusLabel ToolStripStatusLabel { get; private set; }
        public Dictionary<string, Image> ImageList { get; set; }
        public string Id { get; private set; }

        public string Tooltip { get { return ToolStripStatusLabel.ToolTipText; } set { ToolStripStatusLabel.ToolTipText = value; } }
        
        private StatusIcon()
        { }

        public StatusIcon(string id, Image defaultImage)
        {
            ImageList = new Dictionary<string, Image>();
            Id = id;
            ToolStripStatusLabel = new ToolStripStatusLabel(defaultImage);
            AddImage("default", defaultImage);
            ToolStripStatusLabel.Name = id;
            ToolStripStatusLabel.Text = "";
            ToolStripStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.Image;
        }

        public void AddImage(string key, Image img)
        {
            if (!ImageList.ContainsKey(key))
                ImageList.Add(key, img);
        }
    }
}
