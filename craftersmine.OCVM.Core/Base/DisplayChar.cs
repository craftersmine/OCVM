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
        public bool IsInvalidated { get; set; }

        public DisplayChar(char chr, Color foreground, Color background)
        {
            Character = chr;
            ForegroundColor = foreground;
            BackgroundColor = background;
            IsInvalidated = true;
        }

        public void SetChar(char chr, Color foreground, Color background)
        {
            Character = chr;
            ForegroundColor = foreground;
            BackgroundColor = background;
            IsInvalidated = true;
        }

        public void SetChar(char chr)
        {
            SetChar(chr, ForegroundColor, BackgroundColor);
        }

        public bool CheckInvalidation(char chr, Color foreground, Color background)
        {
            if (Character != chr)
                return true;
            if (ForegroundColor != foreground)
                return true;
            if (BackgroundColor != background)
                return true;
            else return false;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
