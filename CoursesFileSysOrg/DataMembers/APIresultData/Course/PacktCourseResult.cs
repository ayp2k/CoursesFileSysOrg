using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg.DataMembers.APIresultData
{
    [DebuggerDisplay("{index}", Name = "{title}")]
    [DataContract]
    public class Child
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public int index { get; set; }
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public string seoUrl { get; set; }
    }

    [DebuggerDisplay("{index}", Name = "{title}")]
    [DataContract]
    public class TableOfContent
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public Child[] children { get; set; }
        [DataMember]
        public int index { get; set; }
        [DataMember]
        public string summary { get; set; }
    }

    [DataContract]
    public class AvailableDownloads
    {
        [DataMember]
        public string[] codeDownloads { get; set; }
    }

    [DataContract]
    public class FirstSampleSection
    {
        [DataMember]
        public string path { get; set; }
        [DataMember]
        public string title { get; set; }
    }

    [DataContract]
    public class Data
    {
        [DataMember]
        public string digitalNid { get; set; }
        [DataMember]
        public string nid { get; set; }
        [DataMember]
        public TableOfContent[] tableOfContents { get; set; }
        [DataMember]
        public AvailableDownloads availableDownloads { get; set; }
        [DataMember]
        public FirstSampleSection firstSampleSection { get; set; }
        [DataMember]
        public string imageUrl { get; set; }
        [DataMember]
        public string title { get; set; }
    }

    [DataContract]
    public class PacktCourseResult
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public int httpStatus { get; set; }
        [DataMember]
        public Data data { get; set; }
    }
}
