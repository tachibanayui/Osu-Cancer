using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osu_Cancer
{
    class BeatmapGroup
    {
        public BeatmapGroup(string songName, string artist, string author, string groupPath)
        {
            SongName = songName;
            Artist = artist;
            Author = author;
            GroupPath = groupPath;
        }

        public BeatmapGroup(string path)
        {
            GroupPath = path;
            GetData();
        }

        private void GetData()
        {
            string s = FileOperation.FileToString(GroupPath + @"\beatmapInfo.bmi", EncodingType.UTF8);
            string[] spString = s.Split('#')[1].Split('\n');
            SongName = spString[1].Substring(6);
            SongName = SongName.Substring(0, SongName.Length - 1);
            Artist = spString[2].Substring(8);
            Artist = Artist.Substring(0, Artist.Length - 1);
            Author = spString[3].Substring(8);
        }

        public string GroupPath { get; set; }
        public string SongName { get; set; }
        public string Artist { get; set; }
        public string Author { get; set; }
        public List<Beatmap> BeatmapDiff { get; set; }
    }
}
