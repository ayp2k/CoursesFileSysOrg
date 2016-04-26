using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;


namespace CoursesFileSysOrg
{
    internal class Pluralsight : Publisher
    {
        public override string Name
        {
            get
            {
                return "PluralSight";
            }
        }

        internal override string BaseURL
        {
            get
            {
                return @"http://www.pluralsight.com/";
            }
        }

        internal override string SearchURL
        {
            get
            {
                return @"https://sp10050dad.guided.ss-omtrdc.net/?q=" + QueryPlaceHolder + "&x10=categories&q10=course&m_Sort=relevance&m_Count=5";
            }
        }

        internal override List<Course> SearchCourse(string courseName)
        {
            string SearchPageHTML;
            List<Course> singleCourse = new List<Course>();
            List<Course> courses = new List<Course>();
            using (WebClient client = new WebClient())
            {
                SearchPageHTML = client.DownloadString(SearchURL.Replace(QueryPlaceHolder, courseName.Replace(' ', '+')));
            }
            ArrayList arr = (ArrayList)(new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(SearchPageHTML)["resultsets"]);
            foreach (Dictionary<string, object> item in ((ArrayList)((Dictionary<string, object>)arr[0])["results"]))
            {
                Course course = new Course();
                //course.id = "0";
                course.Name = item["title"].ToString();
                course.URL = item["url"].ToString().Replace("index:", BaseURL).Replace("?key=", "/").ToLower();
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

            foreach (var node in domDoc.GetElementById("tab-toc__accordion").ChildNodes)
            {
                foreach (var subnode in node.ChildNodes.Where(f => f.NodeName == "DIV"))
                {
                    foreach (var subSubNode in subnode.ChildNodes.Where(f => f.NodeName == "A"))
                    {
                        // chapter / folder name
                        //Course.chapters.Add(new Chapter(++modelIndex, WebUtility.HtmlDecode(subSubNode.TextContent)));
                        Course.Chapters.Add(new Chapter(++modelIndex, subSubNode.TextContent));
                        Course.Chapters[modelIndex-1].TimeStamp = subSubNode.NextSibling.NextSibling.TextContent;
                        localVideoIndex = 1;
                    }
                }
                foreach (var subnode in node.ChildNodes.Where(f => f.NodeName == "A"))
                {
                    foreach (var subSubNode in subnode.ChildNodes.Where(f => f.NodeName == "SPAN"))
                    {
                        // videoItem / file(s) name(s)
                        //VideoItem videoItem = new VideoItem(globalVideoIndex++, localVideoIndex++, WebUtility.HtmlDecode(subSubNode.TextContent));
                        VideoItem videoItem = new VideoItem(globalVideoIndex++, localVideoIndex++, subSubNode.TextContent);
                        videoItem.TimeStamp = subSubNode.NextSibling.NextSibling.TextContent;
                        Course.Chapters[modelIndex - 1].VideoItems.Add(videoItem);
                        break;
                    }
                }
            }
        }

        internal override string GetCourseMetaDataDescription()
        {
            return string.Empty;
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            return new List<string>();
        }
    }
}