using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg.DataMembers.APIresultData
{
    [DataContract(Name = "Data")]
    public class MetaData
    {
        [DataMember]
        public string filepath { get; set; }
        [DataMember]
        public int nid { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string isbn13 { get; set; }
        [DataMember]
        public string publicationDate { get; set; }
        [DataMember]
        public string[] authorList { get; set; }
        [DataMember]
        public string productType { get; set; }
        [DataMember]
        public string pageTitle { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string canonicalUrl { get; set; }
        [DataMember]
        public string coverUrl { get; set; }
    }

    [DataContract]
    public class PacktCourceMetadataResult
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public int httpStatus { get; set; }
        [DataMember]
        public MetaData data { get; set; }
    }
}
