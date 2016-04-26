using System.Diagnostics;

namespace CoursesFileSysOrg
{
    [DebuggerDisplay("{Name}", Name = "{localIndex}")]
    class VideoItem
    {
        public int localIndex { get; set; }
        public int globalIndex { get; set; }
        public string Name { get; set; }

        public string GetFormatedName
        {
            get
            {
                return string.Format("{0:00} {1}", this.localIndex, this.Name);
            }
        }

        public string GetFormatedFileName
        {
            get
            {
                return string.Format("{0:00}. {1}", this.globalIndex, this.Name.Replace("?", "").Replace("\"", "“").CleanUpFileName());
            }
        }

        public string TimeStamp { get; internal set; }
        public bool IsVideo { get; internal set; }

        public VideoItem(int globalID, int localID, string name, bool isVideo = true)
        {
            this.localIndex = localID;
            this.globalIndex = globalID;
            this.Name = name;
            this.IsVideo = isVideo;
        }
    }
}