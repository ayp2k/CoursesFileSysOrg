using System.Collections.Generic;
using System.Diagnostics;

namespace CoursesFileSysOrg
{
    [DebuggerDisplay("{Name}", Name = "{id}")]
    class Chapter
    {
        public int id { get; set; }
        public string Name { get; set; }
        public List<VideoItem> VideoItems { get; set; }

        public string GetFormatedName
        {
            get
            {
                return string.Format("{0:00}. {1}", this.id, this.Name);
            }
        }

        public string GetFormatedFolderName
        {
            get
            {
                return string.Format("{0}. {1}", this.id, this.Name.Replace("?", "").Replace("\"", "“").CleanUpFileName());
            }
        }

        public string TimeStamp { get; internal set; }

        public Chapter(int id, string name)
        {
            this.id = id;
            this.Name = name;
            VideoItems = new List<VideoItem>();
        }
    }
}