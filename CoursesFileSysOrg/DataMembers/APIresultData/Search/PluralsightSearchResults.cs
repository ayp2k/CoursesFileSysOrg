using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg.DataMembers.APIresultData
{
    [DataContract]
    internal class General
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public string total { get; set; }
        [DataMember]
        public int pageLower { get; set; }
        [DataMember]
        public int pageUpper { get; set; }
        [DataMember]
        public int pageTotal { get; set; }
        [DataMember]
        public string redirect { get; set; }
        [DataMember]
        public string seoSearchTitle { get; set; }
        [DataMember]
        public string seoSearchDescription { get; set; }
        [DataMember]
        public string seoSearchKeywords { get; set; }
        [DataMember]
        public string seoBrowseTitle { get; set; }
        [DataMember]
        public string seoBrowseDescription { get; set; }
        [DataMember]
        public string seoBrowseKeywords { get; set; }
        [DataMember]
        public string seoItemTitle { get; set; }
        [DataMember]
        public string seoItemDescription { get; set; }
        [DataMember]
        public string seoItemKeywords { get; set; }
    }

    [DataContract]
    internal class Banner
    {
        [DataMember]
        public string top { get; set; }
        [DataMember]
        public string bottom { get; set; }
        [DataMember]
        public string side { get; set; }
    }

    [DataContract]
    internal class Item
    {
        [DataMember]
        public bool selected { get; set; }
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public string label { get; set; }
        [DataMember]
        public string path { get; set; }
    }

    [DataContract]
    internal class Menu
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string label { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public Item[] items { get; set; }
    }

    [DataContract]
    internal class Breadcrumb
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public object[] values { get; set; }
    }

    [DataContract]
    internal class Page
    {
        [DataMember]
        public object page { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public bool selected { get; set; }
    }

    [DataContract]
    internal class Pagination
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string previous { get; set; }
        [DataMember]
        public string next { get; set; }
        [DataMember]
        public string last { get; set; }
        [DataMember]
        public string viewall { get; set; }
        [DataMember]
        public Page[] pages { get; set; }
    }

    [DataContract]
    internal class Value
    {
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public bool selected { get; set; }
        [DataMember]
        public int count { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public string undolink { get; set; }
        [DataMember]
        public bool threshold { get; set; }
    }

    [DataContract]
    internal class Facet
    {
        [DataMember]
        public string label { get; set; }
        [DataMember]
        public bool @long { get; set; }
        [DataMember]
        public bool selected { get; set; }
        [DataMember]
        public string undolink { get; set; }
        [DataMember]
        public Value[] values { get; set; }
        [DataMember]
        public string last { get; set; }
    }

    [DataContract]
    internal class Author
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string displayName { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
    }

    [DataContract]
    internal class Result
    {
        [DataMember]
        public string prodId { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string keywords { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string loc { get; set; }
        [DataMember]
        public string categories { get; set; }
        [DataMember]
        public string courseName { get; set; }
        [DataMember]
        public string duration { get; set; }
        [DataMember]
        public int imageVersion { get; set; }
        [DataMember]
        public string subjects { get; set; }
        [DataMember]
        public string skillLevels { get; set; }
        [DataMember]
        public string tools { get; set; }
        [DataMember]
        public string publishedDate { get; set; }
        [DataMember]
        public string updatedDate { get; set; }
        [DataMember]
        public double averageRating { get; set; }
        [DataMember]
        public int ratingCount { get; set; }
        [DataMember]
        public int popularity { get; set; }
        [DataMember]
        public bool retired { get; set; }
        [DataMember]
        public bool hasTranscript { get; set; }
        [DataMember]
        public Author[] authors { get; set; }
        [DataMember]
        public string last { get; set; }
    }

    [DataContract]
    internal class Resultset
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Result[] results { get; set; }
    }

    [DataContract]
    internal class Resultcount
    {
        [DataMember]
        public string total { get; set; }
        [DataMember]
        public string pagelower { get; set; }
        [DataMember]
        public string pageupper { get; set; }
    }


    [DataContract]
    class PluralsightSearchResults
    {
        [DataMember]
        public General general { get; set; }
        [DataMember]
        public Banner[] banners { get; set; }
        [DataMember]
        public Menu[] menus { get; set; }
        [DataMember]
        public Breadcrumb[] breadcrumbs { get; set; }
        [DataMember]
        public Pagination[] pagination { get; set; }
        [DataMember]
        public Facet[] facets { get; set; }
        [DataMember]
        public Resultset[] resultsets { get; set; }
        [DataMember]
        public Resultcount resultcount { get; set; }
    }
}