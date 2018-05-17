using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Osu_Cancer
{
    class SkinsLib
    {
        public SkinsLib(string path)
        {
            SkinName = FileOperation.TrimPath(path);
            Cursor = string.Format(path + @"\Cursor.png");
            Cursor = string.Format(path + @"\CursorTrail.png");
            Background = new List<string>();
            AddBackGround(path);
        }

        private void AddBackGround(string path)
        {
            string[] filePath = Directory.GetFiles(path + @"\BackGround");
            foreach (var item in filePath)
            {
                Background.Add(item);
            }
        }

        public string SkinName { get; set; }
        public string Cursor { get; set; }
        public string CursorTrail { get; set; }
        public List<string> Background { get; set; }
    }
}
