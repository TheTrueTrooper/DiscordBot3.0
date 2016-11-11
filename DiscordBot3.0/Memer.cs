using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;

namespace DiscordBot3._0
{

    public class Memer
    {
        static Font _Font = new Font(new FontFamily(GenericFontFamilies.Serif), 14);

        static StringFormat SF = new StringFormat(StringFormatFlags.FitBlackBox);
        
        static Memer()
        {
            SF.Alignment = StringAlignment.Center;
        }

        public Image DrawText(string Text, string Text2, string Path)
        {
            Bitmap I = new Bitmap(Image.FromFile(Path));

            Graphics _Graphics = Graphics.FromImage(I);

            _Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            _Graphics.TextContrast = 10;
            _Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            _Graphics.DrawString(Text, _Font, Brushes.Black, new Rectangle(0, 0, (int)I.Width, (int)(I.Height / 4)), SF);
            _Graphics.DrawString(Text, _Font, Brushes.Black, new Rectangle(0, 0, (int)I.Width, (int)(I.Height / 4)), SF);
            _Graphics.DrawString(Text, _Font, Brushes.RoyalBlue, new Rectangle(0, 0, (int)I.Width, (int)(I.Height / 4)), SF);
            _Graphics.DrawString(Text, _Font, Brushes.RoyalBlue, new Rectangle(0, 0, (int)I.Width, (int)(I.Height / 4)), SF);

            _Graphics.DrawString(Text2, _Font, Brushes.Black, new Rectangle(0, (int)(I.Height * 3 / 4), (int)I.Width, (int)(I.Height / 4)), SF);
            _Graphics.DrawString(Text2, _Font, Brushes.Black, new Rectangle(0, (int)(I.Height * 3 / 4), (int)I.Width, (int)(I.Height / 4)), SF);
            _Graphics.DrawString(Text2, _Font, Brushes.RoyalBlue, new Rectangle(0, (int)(I.Height * 3 / 4), (int)I.Width, (int)(I.Height / 4)), SF);
            _Graphics.DrawString(Text2, _Font, Brushes.RoyalBlue, new Rectangle(0, (int)(I.Height * 3 / 4), (int)I.Width, (int)(I.Height / 4)), SF);

            _Graphics.Flush();

            return I;
        }

    }
}
