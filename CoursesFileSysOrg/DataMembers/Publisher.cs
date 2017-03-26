using System.Collections.Generic;
using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using System.Threading.Tasks;
using System.Net;
using AngleSharp.Dom.Html;

namespace CoursesFileSysOrg
{

    internal abstract class Publisher
    {
        internal static readonly string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586";
        internal static readonly string ContentTypePost = "application/x-www-form-urlencoded";
        internal static readonly string QueryPlaceHolder = "{query}";

        internal abstract string BaseURL { get; }
        internal abstract string SearchURL { get; }
        internal abstract string RefererURL { get; }
        internal HtmlParser domParser = new HtmlParser();
        
        public abstract string Name { get; }
        public Course Course { get; internal set; }

        public static Publisher Create(string publisherName)
        {
            switch (publisherName.ToLower())
            {
                case "pluralsight":
                case "frontend masters":
                case "frontendmasters":
                case "digital tutors":
                case "digital-tutors":
                case "digitaltutors":
                    return new Pluralsight();
                case "lynda":
                    return new Lynda();
                case "udemy":
                    return new Udemy();
                case "tutsplus":
                case "tuts+":
                case "envato":
                    return new TutsPlus();
                case "infiniteskills":
                case "infinite skills":
                case "o'reilly":
                case "oreilly":
                    return new InfiniteSkills();
                case "livelessons":
                case "prentice hall":
                case "prenticehall":
                case "informit":
                    return new LiveLessons();
                case "packt":
                case "packtpub":
                    return new Packt();
                default:
                    return null;
            }
        }

        internal abstract Task<List<Course>> SearchCourse(string courseName);
        internal abstract void PopulateAllCourseItems();

        internal abstract string GetCourseMetaDataDescription();
        internal abstract List<string> GetCourseMetaDataCategories();

        internal string GetCourseURL(string courseName)
        {
            return SearchURL.Replace(QueryPlaceHolder, WebUtility.UrlEncode(courseName));
        }

        internal IHtmlDocument GetDomObjectFromSourceHtml(string sourceHtml)
        {
            return domParser.Parse(sourceHtml);
        }

        internal async Task<IHtmlDocument> GetDomObjectFromSiteUrlAsync(string siteUrl)
        {
            var SearchPageHTML = await WebUtils.GetWebContentAsync(siteUrl);
            return await domParser.ParseAsync(SearchPageHTML);
        }

        internal IHtmlDocument GetDomObjectFromSiteUrl(string siteUrl)
        {
            return GetDomObjectFromSiteUrlAsync(siteUrl).Result;
        }
    }
}