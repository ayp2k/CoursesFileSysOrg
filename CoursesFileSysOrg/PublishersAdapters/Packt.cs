using AngleSharp.Dom;
using CoursesFileSysOrg.DataMembers.APIresultData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    internal class Packt : Publisher
    {
        public override string Name
        {
            get
            {
                return "PacktPub";
            }
        }

        internal override string BaseURL
        {
            get
            {
                return @"http://www.packtpub.com";
            }
        }

        internal override string SearchURL
        {
            get
            {
                return @"https://www.packtpub.com/all?search=" + QueryPlaceHolder;
            }
        }

        internal override string RefererURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal string CourceURL { get; set; }
        internal string MetaDataURL { get; set; }

        internal Packt()
        {
            this.CourceURL = @"/mapt-rest/products/{id}/metadata";
            this.MetaDataURL = @"/mapt-rest/products/{id}/header-metadata";
        }

        internal async override Task<List<Course>> SearchCourse(string courseName)
        {
            List<Course> courses = new List<Course>();
            var domDoc = await GetDomObjectFromSiteUrlAsync(GetCourseURL(courseName));
            var domItems = domDoc.QuerySelectorAll(".book-block-outer");

            if (domItems.Length > 0)
            {
                foreach (var item in domItems)
                {
                    Course course = new Course();
                    var innerElement = item.GetElementsByClassName("book-block-outer").FirstOrDefault();
                    if (innerElement != null)
                    {
                        course.id = innerElement.GetAttribute("data-product-id");
                        course.Name = innerElement.GetAttribute("data-product-title").Replace("[Video]", "").Trim();
                        course.URL = BaseURL + innerElement.QuerySelector("a[href]").GetAttribute("href");
                        if (course.URL != null && course.URL.Length > 0)
                        {
                            var domSubDoc = await GetDomObjectFromSiteUrlAsync(course.URL);
                            var domItem = domSubDoc.QuerySelector(".preview-product-mapt"); // or showPacktLibReaderNewWindow class
                            if (domItem != null)
                            {
                                //var type = domItem.GetAttribute("type");
                                //var category = domItem.GetAttribute("category");
                                var isbn = domItem.GetAttribute("isbn");
                                if (isbn != null)
                                {
                                    course.URL = BaseURL + CourceURL.Replace("{id}", isbn);
                                    course.id = isbn;
                                }
                            }
                        }
                        if (course.Name.StripNonAlphaNumeric().ToLower() == courseName.StripNonAlphaNumeric().ToLower())
                        {
                            return new List<Course>(1) { course };
                        }
                        courses.Add(course);
                    }
                }
            }
            return courses;
        }

        internal override void PopulateAllCourseItems()
        {
            int chapterIndex = 1;
            int globalVideoIndex = 1;
            int localVideoIndex = 1;

            var jsonResult = Course.GetWebPageHTML;
            PacktCourseResult packtCourseResult = jsonResult.DeserializeJSON<PacktCourseResult>();
            foreach (var item in packtCourseResult.data.tableOfContents)
            {
                Chapter chapter = new Chapter(chapterIndex++, item.title);
                localVideoIndex = 1;
                foreach (var subItem in item.children)
                {
                    chapter.VideoItems.Add(new VideoItem(globalVideoIndex++, localVideoIndex++, WebUtility.HtmlDecode(subItem.title.Trim())));
                }
                Course.Chapters.Add(chapter);
            }
        }

        private void PopulateCourceMetadata()
        {
            var metaDataUrl = BaseURL + MetaDataURL.Replace("{id}", Course.id);
            var jsonResult = WebUtils.GetWebContentAsync(metaDataUrl).Result;
            PacktCourceMetadataResult packtCourceMetadataResult = jsonResult.DeserializeJSON<PacktCourceMetadataResult>();
            Course.Description = packtCourceMetadataResult.data.description;
            Course.MainCategory = packtCourceMetadataResult.data.category;
        }

        internal override string GetCourseMetaDataDescription()
        {
            if (Course.Description == null || Course.Description == string.Empty)
                PopulateCourceMetadata();
            return Course.Description;
        }

        internal override List<string> GetCourseMetaDataCategories()
        {
            if (Course.MainCategory == null || Course.MainCategory == string.Empty)
                PopulateCourceMetadata();
            return new List<string>(1) { Course.MainCategory };
        }


    }
}
