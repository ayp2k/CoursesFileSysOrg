using CoursesFileSysOrg.DataMembers.APIresultData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
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
                return @"https://sp10050dad.guided.ss-omtrdc.net/?q=" + QueryPlaceHolder + "&x10=categories&q10=course&m_Sort=relevance&m_Count=" + SearchResultsCount;
            }
        }

        internal override string RefererURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int SearchResultsCount { get; set; }

        public Pluralsight()
        {
            this.SearchResultsCount = 25;
        }

        internal async override Task<List<Course>> SearchCourse(string courseName)
        {
            string SearchPageJSON;
            List<Course> singleCourse = new List<Course>();
            List<Course> courses = new List<Course>();
            using (WebClient client = new WebClient())
            {
                SearchPageJSON = await client.DownloadStringTaskAsync(SearchURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName)));
            }


            PluralsightSearchResults pluralsightSearchResults;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PluralsightSearchResults));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(SearchPageJSON)))
            {
                pluralsightSearchResults = serializer.ReadObject(ms) as PluralsightSearchResults;
            }

            foreach (var item in pluralsightSearchResults.resultsets[0].results)
            {
                Course course = new Course();
                course.id = item.prodId;
                course.Name = WebUtility.HtmlDecode(item.title); //Encoding.UTF8.GetString(Encoding.Default.GetBytes(item.title));
                course.URL = item.url.Replace("index:", BaseURL).Replace("?key=", "/").ToLower();
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
            
            //ArrayList arr = (ArrayList)(new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(SearchPageJSON)["resultsets"]);
            //foreach (Dictionary<string, object> item in ((ArrayList)((Dictionary<string, object>)arr[0])["results"]))
            //{
            //    Course course = new Course();
            //    //course.id = "0";
            //    course.Name = item["title"].ToString();
            //    course.URL = item["url"].ToString().Replace("index:", BaseURL).Replace("?key=", "/").ToLower();
            //    if (course.Name.ToLower() == courseName.ToLower())
            //    {
            //        singleCourse.Add(course);
            //        return singleCourse;
            //    }
            //    else
            //    {
            //        courses.Add(course);
            //    }
            //}
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
            List<string> elements = new List<string>();
            var domDoc = domParser.Parse(Course.GetWebPageHTML);
            var element = domDoc.QuerySelector("div.text-component");
            elements.Add(WebUtility.UrlDecode(element.InnerHtml));
            elements.Add(Environment.NewLine);
            element = domDoc.QuerySelector("div#tab-description");
            elements.Add(WebUtility.UrlDecode(element.TextContent));
            return string.Join(Environment.NewLine, elements);
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            return new List<string>();
        }
    }
}