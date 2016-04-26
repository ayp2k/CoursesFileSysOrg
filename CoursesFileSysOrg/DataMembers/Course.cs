using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

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