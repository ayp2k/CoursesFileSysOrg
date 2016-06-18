using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Net;

namespace CoursesFileSysOrg
{
    internal class TutsPlus : Publisher
    {
        public override string Name
        {
            get
            {
                return "Tuts Plus";
            }
        }

        internal override string BaseURL
        {
            get
            {
                return @"http://www.tutsplus.com/";
            }
        }

        internal override string SearchURL
        {
            get
            {
                return @"http://tutsplus.com/courses/search?search%5Bterms%5D=" + QueryPlaceHolder;
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

        internal override List<Course> SearchCourse(string courseName)
        {
            string SearchPageHTML;
            IElement domItem;
            List<Course> singleCourse = new List<Course>();
            List<Course> courses = new List<Course>();

            using (WebClient client = new WebClient())
            {
                client.Headers["User-Agent"] = UserAgent;
                //client.Headers.Add(HttpRequestHeader.Cookie, "__cfduid");
                SearchPageHTML = client.DownloadString(SearchURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName)));
            }
            var domDoc = domParser.Parse(SearchPageHTML);
            domItem = domDoc.QuerySelector(".posts");

            foreach (var item in domItem.Children)
            {
                Course course = new Course();
                //course.id = "0";
                course.Name = item.QuerySelector(".posts__post-title").TextContent.Trim();
                course.URL = item.QuerySelector(".posts__post-title").Attributes["href"].Value;

                if (course.Name.ToLower() == courseName.ToLower())
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

            var elements = domDoc.QuerySelector("div.lesson-index");
            foreach (var node in elements.QuerySelectorAll("h2"))
            {
                currChapter = new Chapter(++modelIndex, node.QuerySelector(".lesson-index__chapter-title-text").TextContent);
                Course.Chapters.Add(currChapter);
                Course.Chapters[modelIndex - 1].TimeStamp = node.QuerySelector(".lesson-index__chapter-meta").TextContent;
            }
            foreach (var node in elements.QuerySelectorAll("h3"))

            {
                var videoName = node.QuerySelector(".lesson-index__lesson-title").TextContent;
                var videoIndex = node.QuerySelector(".lesson-index__lesson-number").TextContent.Split(new char[] { '.' });
                modelIndex = Convert.ToInt32(videoIndex[0]);
                localVideoIndex = Convert.ToInt32(videoIndex[1]);
                VideoItem vItem = new VideoItem(globalVideoIndex++, localVideoIndex, videoName);
                vItem.TimeStamp = node.QuerySelector(".lesson-index__lesson-duration").TextContent;
                if (modelIndex != 0)
                {
                    Course.Chapters[modelIndex - 1].VideoItems.Add(vItem);
                }
            }
        }

        internal override string GetCourseMetaDataDescription()
        {
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            var element = domDoc.QuerySelector("div.course__description").FirstChild;
            return element.TextContent.Trim(new char[] { '\t', '\n', ' ' });
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            List<string> categorieValue = new List<string>();
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            foreach(var element in domDoc.QuerySelectorAll(".course-meta__tag"))
            {
                categorieValue.Add(element.TextContent);
            }
            return categorieValue;
        }
    }
}

