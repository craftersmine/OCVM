using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Base
{
    public class DisplayChar
    {
        public char Character { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public DisplayChar(char chr, Color foreground, Color background)
        {
            Character = chr;
            ForegroundColor = foreground;
            BackgroundColor = background;
        }
    }
}
