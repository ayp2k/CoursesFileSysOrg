using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    class LiveLessons : Publisher
    {
        private Chapter currChapter;

        public override string Name
        {
            get
            {
                return "LiveLessons (Prentice Hall)";
            }
        }

        internal override string BaseURL
        {
            get
            {
                return @"http://www.informit.com";
            }
        }

        internal override string SearchURL
        {
            get
            {
                return @"http://www.informit.com/search/index.aspx?query=" + QueryPlaceHolder;
            }
        }

        internal override string RefererURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal override async Task<List<Course>> SearchCourse(string courseName)
        {
            string SearchPageHTML;
            IElement domItem;
            List<Course> singleCourse = new List<Course>();
            List<Course> courses = new List<Course>();

            using (WebClient client = new WebClient())
            {
                //client.Headers["User-Agent"] = UserAgent;
                SearchPageHTML = await client.DownloadStringTaskAsync(SearchURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName)));
            }
            var domDoc = domParser.Parse(SearchPageHTML);
            domItem = domDoc.QuerySelector("div#docs");

            foreach (var item in domItem.QuerySelectorAll(".title"))
            {
                Course course = new Course();
                course.URL = item.Attributes["href"].Value;
                course.id = course.URL.Substring(course.URL.LastIndexOf('-') + 1); // ISBN-13
                course.Name = item.TextContent.Split(new string[] { "LiveLessons" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                course.URL = this.BaseURL + course.URL;

                if (course.Name.StripNonAlphaNumeric().ToLower() == courseName.StripNonAlphaNumeric().ToLower())
                {
                    singleCourse.Add(course);
                    return singleCourse;
                }
                else
                {
                    courses.Add(course);
                }
            }
            return courses;
        }

        internal override void PopulateAllCourseItems()
        {
            int modelIndex = 0, localVideoIndex = 0, globalVideoIndex = 1;
            var domDoc = domParser.Parse(Course.GetWebPageHTML);

            var nodes= domDoc.QuerySelector("div#bssContent").ChildNodes[5];
            foreach(var node in nodes.ChildNodes)
            {
                if (node.HasChildNodes)
                {
                    var subNode = node.FirstChild;
                    if (subNode.NodeName == "B")
                    {
                        var chapterName = subNode.TextContent.StartsWith("Lesson") ? subNode.TextContent.Substring(10) : subNode.TextContent;
                        currChapter = new Chapter(++modelIndex, chapterName);
                        Course.Chapters.Add(currChapter);
                    }
                }
                if (node.NodeName == "BLOCKQUOTE")
                {
                    foreach(var subNode in node.ChildNodes)
                    {
                        if (subNode.NodeName == "P")
                        {
                            var videoName = subNode.TextContent.Trim();
                            //var videoIndex = node.QuerySelector(".lesson-index__lesson-number").TextContent.Split(new char[] { '.' });
                            VideoItem vItem = new VideoItem(globalVideoIndex++, localVideoIndex++, videoName);
                            if (modelIndex != 0)
                            {
                                Course.Chapters[modelIndex - 1].VideoItems.Add(vItem);
                            }
                        }
                    }
                    localVideoIndex = 0;
                }
            }
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            return new List<string>();
        }

        internal override string GetCourseMetaDataDescription()
        {
            return string.Empty;
        }
    }
}
