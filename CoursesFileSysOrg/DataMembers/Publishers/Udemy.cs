using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using CoursesFileSysOrg.DataMembers.APIresultData;
using System.Text;

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
                //return @"https://www.udemy.com/courses/search/?q=" + QueryPlaceHolder + "&lang=en";
                return @"https://www.udemy.com/api-2.0/search-courses/?lang=en&q=" + QueryPlaceHolder + "&src=ukw";
            }
        }

        internal override string RefererURL
        {
            get
            {
                return @"https://www.udemy.com/courses/search/?q=" + QueryPlaceHolder + "&src=ukw&lang=en";
            }
        }

        private string CourseIDItemsURL
        {
            get
            {
                return @"https://www.udemy.com/api-2.0/courses/{id}/public-curriculum-items?page_size=100&fields[results]=@min";
            }
        }

        private Chapter currChapter;

        internal override List<Course> SearchCourse(string courseName)
        {
            string SearchPageJSON;
            //IElement domItem;
            List<Course> singleCourse = new List<Course>();
            List<Course> courses = new List<Course>();

            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Referer, RefererURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName.Replace(' ', '+'))));
                SearchPageJSON = client.DownloadString(SearchURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName.Replace(' ', '+'))));
                int startIndex = SearchPageJSON.IndexOf("\"courses\":") + 10;
                int endIndex = SearchPageJSON.IndexOf(", \"pagination\":");
                SearchPageJSON = SearchPageJSON.Substring(startIndex, endIndex - startIndex);
            }

            //var domDoc = domParser.Parse(SearchPageJSON);
            //domItem = domDoc.QuerySelector("[id='courses']");
            List<UdemySearchResults> udemySearchResults;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<UdemySearchResults>));
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(SearchPageJSON)))
            {
                udemySearchResults = serializer.ReadObject(ms) as List<UdemySearchResults>;
            }

            //foreach (var item in domItem.Children)
            foreach (var item in udemySearchResults)
            {
                Course course = new Course();
                //course.id = item.Attributes["data-courseid"].Value;
                //course.Name = item.QuerySelector(".title.bold.fs15-force.ng-binding").TextContent.Trim();
                //course.URL = this.BaseURL + item.QuerySelector(".course-box-flat.pr0-force-xs.pl0-force-xs a").Attributes["href"].Value;
                course.id = item.id.ToString();
                course.Name = item.title;
                course.URL = this.BaseURL + item.url;

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
            PopulateAllCourseItemsByJSON();
        }

        private void PopulateAllCourseItemsByJSON()
        {
            string CurriculumPageJSON;
            using (WebClient client = new WebClient())
            {
                CurriculumPageJSON = client.DownloadString(CourseIDItemsURL.Replace("{id}", Course.id));
                int startIndex = CurriculumPageJSON.IndexOf("\"results\":") + 10;
                CurriculumPageJSON = CurriculumPageJSON.Substring(startIndex, CurriculumPageJSON.Length - startIndex - 1);
            }

            List<UdemyCourseResult> udemyCourseResult;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<UdemyCourseResult>));
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(CurriculumPageJSON)))
            {
                udemyCourseResult = serializer.ReadObject(ms) as List<UdemyCourseResult>;
            }

            int modelIndex = 0, localVideoIndex = 1, globalVideoIndex = 1;
            foreach (var item in udemyCourseResult)
            {
                switch (item._class)
                {
                    case "chapter":
                        Course.Chapters.Add(new Chapter(++modelIndex, item.title));
                        localVideoIndex = 1;
                        break;
                    case "lecture":
                        VideoItem vItem = new VideoItem(globalVideoIndex++, localVideoIndex++, item.title);
                        vItem.IsVideo = (item.asset.asset_type == "Video");
                        if (!vItem.IsVideo)
                            vItem.TimeStamp = "ntvid";
                        if (modelIndex != 0)
                        {
                            Course.Chapters[modelIndex - 1].VideoItems.Add(vItem);
                        }
                        break;
                }
            }
        }

        private void PopulateAllCourseItemsByHTML()
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