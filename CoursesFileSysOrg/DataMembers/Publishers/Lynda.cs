using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Net;

namespace CoursesFileSysOrg
{
    internal class Lynda : Publisher
    {
        public override string Name
        {
            get
            {
                return "Lynda";
            }
        }

        internal override string BaseURL
        {
            get
            {
                return @"http://www.lynda.com/";
            }
        }

        internal override string SearchURL
        {
            get
            {
                return @"http://www.lynda.com/search?q=" + QueryPlaceHolder + "&f=producttypeid:2";
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
                //client.Headers["User-Agent"] = UserAgent;
                SearchPageHTML = client.DownloadString(SearchURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName)));
            }
            var domDoc = domParser.Parse(SearchPageHTML);
            domItem = domDoc.QuerySelector("ul.course-list");

            foreach (var item in domItem.QuerySelectorAll("div.details-row"))
            {
                Course course = new Course();
                course.id = item.FirstElementChild.Id.Split(new char[] { '-' })[3];
                course.Name = item.FirstElementChild.TextContent.Trim();
                course.URL = item.FirstElementChild.Attributes["href"].Value.Split(new char[] { '?' })[0];

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

            foreach (var node in domDoc.QuerySelectorAll("[id^='toc-chapter-']"))
            {
                var item = node.QuerySelector("div h3 a");
                string chapterName = item.TextContent;
                if (chapterName.Substring(0,5).IndexOf('.') != -1)
                {
                    chapterName = chapterName.Substring(chapterName.IndexOf('.') + 2);
                }
                currChapter = new Chapter(modelIndex++, chapterName);
                Course.Chapters.Add(currChapter);
                Course.Chapters[modelIndex - 1].TimeStamp = node.QuerySelector("span").TextContent;

                localVideoIndex = 1;
                foreach (var subNode in node.QuerySelectorAll("li dl"))
                {
                    var videoName = subNode.QuerySelector("dt b a").InnerHtml.TrimEnd(new char[] { '\t', '\n', ' ' });
                    VideoItem vItem = new VideoItem(globalVideoIndex++, localVideoIndex++, videoName);
                    vItem.TimeStamp = subNode.QuerySelector("dd").TextContent.Trim(new char[] { '\t', '\n', ' ' });
                    currChapter.VideoItems.Add(vItem);
                }
            }
        }

        internal override string GetCourseMetaDataDescription()
        {
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            var element = domDoc.QuerySelector("div.course-description").FirstElementChild;
            //return WebUtility.HtmlDecode(element.TextContent.Trim(new char[] { '\t', '\n', ' ' }));
            return element.TextContent.Trim(new char[] { '\t', '\n', ' ' });
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            List<string> categorieValue = new List<string>();
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            foreach (var element in domDoc.QuerySelectorAll("dl.course-categories dd"))
            {
                if (element.QuerySelector("span a").GetAttribute("itemprop") != "author")
                {
                    foreach (var item in element.TextContent.Replace("\t", "").Replace("\n", "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        categorieValue.Add(item);
                    }
                }
            }
            return categorieValue;
        }
    }
}