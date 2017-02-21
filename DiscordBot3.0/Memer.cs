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
            //try for the impact font if not just take a basic serif
            try
            {
                _Font = new Font(new FontFamily("Impact"), 14);
            }
            catch
            {
                _Font = new Font(new FontFamily(GenericFontFamilies.Serif), 14);
            } 
        }

        public Image DrawText(string Text, string Text2, string Path)
        {
            Bitmap I = new Bitmap(Image.FromFile(Path));

            Graphics _Graphics = Graphics.FromImage(I);
            GraphicsPath _PathT = new GraphicsPath();
            GraphicsPath _PathB = new GraphicsPath();

            _Graphics.CompositingQuality = CompositingQuality.HighQuality;
            _Graphics.TextContrast = 10;
            _Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            _Graphics.SmoothingMode = SmoothingMode.HighQuality;


            // do this once for speed; quarter out the image for text space
            Rectangle RTop = new Rectangle(0, 0, (int)I.Width, (int)(I.Height / 4));
            Rectangle RBot = new Rectangle(0, (int)(I.Height * 3 / 4), (int)I.Width, (int)(I.Height / 4));

            Pen _Pen = new Pen(Color.White, 4);

            float EMSize = _Graphics.DpiY * _Font.SizeInPoints / 72;

            _PathT.AddString(Text, _Font.FontFamily, (int)_Font.Style, EMSize, RTop, SF);
            _Graphics.DrawPath(_Pen, _PathT);

            _Graphics.DrawString(Text, _Font, Brushes.Black, RTop, SF);
            _Graphics.DrawString(Text, _Font, Brushes.Black, RTop, SF);

            _PathB.AddString(Text2, _Font.FontFamily, (int)_Font.Style, EMSize, RBot, SF);
            _Graphics.DrawPath(_Pen, _PathB);

            _Graphics.DrawString(Text2, _Font, Brushes.Black, RBot, SF);
            _Graphics.DrawString(Text2, _Font, Brushes.Black, RBot, SF);

            _Graphics.Flush();

            return I;
        }

    }
}
