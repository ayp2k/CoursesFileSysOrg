using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    internal class InfiniteSkills : Publisher
    {
        public override string Name
        {
            get
            {
                return "Infinite Skills";
            }
        }

        internal override string BaseURL
        {
            get
            {
                return @"http://www.infiniteskills.com"; 
            }
        }

        internal override string SearchURL
        {
            get
            {
                return @"http://www.infiniteskills.com/files/search.html";
            }
        }

        internal override string RefererURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private Chapter currChapter;

        internal async override Task<List<Course>> SearchCourse(string courseName)
        {
            IElement domItem;
            string SearchPageHTML;
            List<Course> singleCourse = new List<Course>();
            List<Course> courses = new List<Course>();

            using (WebClient client = new WebClient())
            {
                string postParameters = "keywordsearch=" + WebUtility.UrlEncode(courseName) + "&x=25&y=25";
                client.Headers[HttpRequestHeader.ContentType] = ContentTypePost;
                //client.Headers[HttpRequestHeader.UserAgent] = UserAgent;
                //client.Headers[HttpRequestHeader.Referer] = BaseURL;
                SearchPageHTML = await client.UploadStringTaskAsync(this.SearchURL, postParameters);
            }
            var domDoc = domParser.Parse(SearchPageHTML);
            domItem = domDoc.QuerySelector("section.category-product-wrapper");

            foreach (var item in domItem.QuerySelectorAll("a"))
            {
                Course course = new Course();
                course.id = item.FirstElementChild.Id.Substring(7);
                course.Name = item.QuerySelector("h2").TextContent;
                course.URL = this.BaseURL + item.Attributes["href"].Value;

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
            int modelIndex = 0, localVideoIndex = 1, globalVideoIndex = 1;
            var domDoc = domParser.Parse(Course.GetWebPageHTML);

            var elements = domDoc.QuerySelector("ul#menu1");
            foreach(var node in elements.Children)
            {
                var chapterName = node.QuerySelector("h5").TextContent;
                if (chapterName.Contains("00. Free Videos"))
                    continue;
                currChapter = new Chapter(++modelIndex, chapterName.Substring(chapterName.IndexOf(". ") + 2));
                Course.Chapters.Add(currChapter);
                //Course.Chapters[modelIndex - 1].TimeStamp = "";
                foreach(var subNode in node.QuerySelector("ul").QuerySelectorAll("li"))
                {
                    var videoName = subNode.TextContent;
                    if (videoName.IndexOf(string.Format("{0:00}{1:00}", modelIndex, localVideoIndex)) != -1)
                        videoName = videoName.Substring(videoName.IndexOf(' ') + 1);
                    VideoItem vItem = new VideoItem(globalVideoIndex++, localVideoIndex++, videoName);
                    Course.Chapters[modelIndex - 1].VideoItems.Add(vItem);
                }
                localVideoIndex = 1;
            }
        }

        internal override string GetCourseMetaDataDescription()
        {
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            var element = domDoc.QuerySelector("div#example2 p");
            return element.TextContent;
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            return new List<string>();
        }
    }
}