using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg.DataMembers.APIresultData
{
    [DataContract]
    public class Asset
    {
        [DataMember]
        public string _class { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string asset_type { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string created { get; set; }
    }

    [DataContract]
    class UdemyCourseResult
    {
        [DataMember]
        public string _class { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string created { get; set; }
        [DataMember]
        public int sort_order { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string title_cleaned { get; set; }
        [DataMember]
        public bool? is_published { get; set; }
        [DataMember]
        public string transcript { get; set; }
        [DataMember]
        public bool? is_downloadable { get; set; }
        [DataMember]
        public bool? is_free { get; set; }
        [DataMember]
        public Asset asset { get; set; }
    }
}
