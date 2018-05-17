using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osu_Cancer
{
    class Beatmap
    {
        public Beatmap(string name, string artist, string author, TimeSpan length, int objectCount, float cs, float ar, float od, float hp, float stardiff)
        {
            Name = name;
            Artist = artist;
            Author = author;
            Length = length;
            ObjectCount = objectCount;
            CS = cs;
            AR = ar;
            OD = od;
            HP = hp;
            StarDiffculity = stardiff;
        }

        public string Name { get; set; }
        public string Artist { get; set; }
        public string Author { get; set; }
        public TimeSpan Length { get; set; }
        public int ObjectCount { get; set; }
        public float CS { get; set; }
        public float AR { get; set; }
        public float OD { get; set; }
        public float HP { get; set; }
        public float StarDiffculity { get; set; }
        public List<NoteInfo> noteInfos = new List<NoteInfo>();
    }
}
