using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Linq;

namespace CoursesFileSysOrg
{
    [DebuggerDisplay("{id}", Name = "{Name}")]
    internal class Course
    {
        public string id { get; set; }
        public string Name { get; internal set; }
        public string URL { get; internal set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public List<Chapter> Chapters { get; set; }

        private string webPageHTML;

        public Course()
        {
            Chapters = new List<Chapter>();
        }

        public string GetWebPageHTML
        {
            get
            {
                if (this.webPageHTML == null || this.webPageHTML == string.Empty)
                    GetCourseWebPage();
                return this.webPageHTML;
            }
        }

        public int GetChaptersCount
        {
            get
            {
                return Chapters.Count;
            }
        }

        public int GetChaptersNumOfDigits
        {
            //int width = (files.Count + 1).ToString("d").Length;
            //string formatString = "{0:D" + width + "}_{1}.xxx";
            get
            {
                return GetChaptersCount.ToString().Length;
            }
        }

        public int GetVideoItemsCount
        {
            get
            {
                return Chapters.Sum(i => i.VideoItems.Count);
            }
        }

        public int GetVideoItemsNumOfDigits
        {
            get
            {
                return GetVideoItemsCount.ToString().Length;
            }
        }

        private void GetCourseWebPage()
        {
            if (this.URL != null || this.URL != string.Empty)
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers["User-Agent"] = Publisher.UserAgent;
                    client.Encoding = System.Text.Encoding.GetEncoding("utf-8");
                    webPageHTML = client.DownloadString(this.URL);
                }
            }
        }
    }
}