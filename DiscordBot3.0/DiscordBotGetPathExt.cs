using System;

namespace DiscordBot3._0
{
    /// <summary>
    /// a static (non instanced class of Path getter)
    /// </summary>
    static class PathGetter
    {
        public static string TempImage { get { return Environment.CurrentDirectory + @"\TempImage.jpg";  } }

        public static string GetSoundBoardPath(string file)
        {
            return Environment.CurrentDirectory + @"\..\..\_SoundBoard_\" + file;
        }

        public static string GetMemePath(string file)
        {
            return Environment.CurrentDirectory + @"\..\..\_BlankMemes_\" + file;
        }

        public static string GetImagePath(string file)
        {
            return Environment.CurrentDirectory + @"\..\..\_Images_\" + file;
        }
    }

}
