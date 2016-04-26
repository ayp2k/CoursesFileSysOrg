using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CoursesFileSysOrg
{
    internal class Udemy : Publisher
    {
        public override string Name
        {
            get
            {
                return "Udemy";
            }
        }

        internal override string BaseURL
        {
            get
            {
                return @"http://www.udemy.com";
            }
        }

        internal override string SearchURL
        {
            get
            {
                return @"https://www.udemy.com/courses/search/?q=" + QueryPlaceHolder + "&lang=en";
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
                SearchPageHTML = client.DownloadString(SearchURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName.Replace(' ', '+'))));
            }
            var domDoc = domParser.Parse(SearchPageHTML);
            domItem = domDoc.QuerySelector("[id='courses']");

            foreach (var item in domItem.Children)
            {
                Course course = new Course();
                course.id = item.Attributes["data-courseid"].Value;
                course.Name = item.QuerySelector(".title.bold.fs15-force.ng-binding").TextContent.Trim();
                course.URL = this.BaseURL + item.QuerySelector(".course-box-flat.pr0-force-xs.pl0-force-xs a").Attributes["href"].Value;

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

            var elements = domDoc.QuerySelector("table.cur-list.table.table-hover.ud-landingpage-curriculum"); //.TextContent
            foreach (var node in elements.QuerySelectorAll("[class^='cur-list-']"))
            {
                if (node.ClassName.Contains("cur-list-title"))
                {
                    currChapter = new Chapter(++modelIndex, node.TextContent.Trim(new char[] { '\t', '\n', ' ' }).Split(new char[] { ':' })[1].TrimStart());
                    Course.Chapters.Add(currChapter);
                    //Course.chapters[modelIndex - 1].TimeStamp = "";
                    localVideoIndex = 1;
                }
                else if (node.ClassName.Contains("cur-list-row-detail"))
                { }
                else if (node.ClassName.Contains("cur-list-row"))
                {
                    var videoName = node.QuerySelector("div.lec-title.fx").TextContent.Trim(new char[] { '\t', '\n', ' ' });
                    VideoItem vItem = new VideoItem(globalVideoIndex++, localVideoIndex++, videoName);
                    vItem.IsVideo = node.QuerySelector(".icon-play-sign") != null;
                    if (vItem.IsVideo)
                        vItem.TimeStamp = node.QuerySelector("td.tar.lec-det").TextContent.Trim(new char[] { '\t', '\n', ' ' });
                    else
                        vItem.TimeStamp = "ntvid";
                    if (modelIndex != 0)
                    {
                        Course.Chapters[modelIndex - 1].VideoItems.Add(vItem);
                    }
                }
            }
        }

        internal override string GetCourseMetaDataDescription()
        {
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            var element = domDoc.QuerySelector("div.ci-d").FirstChild;
            return element.TextContent.Trim(new char[] { '\t', '\n', ' ' });
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            List<string> categorieValue = new List<string>();
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            foreach (var element in domDoc.QuerySelectorAll("span.cats a"))
            {
                categorieValue.Add(element.TextContent);
            }
            return categorieValue;
        }
    }
}