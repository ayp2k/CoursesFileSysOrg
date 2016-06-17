using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg.DataMembers.APIresultData
{
    [CollectionDataContract]
    public class VisibleInstructor : List<object>
    {
        public string _class { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public string job_title { get; set; }
        public string image_50x50 { get; set; }
        public string image_100x100 { get; set; }
        public string url { get; set; }
    }

    [DataContract]
    public class DiscountPrice
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string price_string { get; set; }
        public string currency_symbol { get; set; }
    }

    [DataContract]
    class UdemySearchResults
    {
        [DataMember]
        public string _class { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public bool is_paid { get; set; }
        [DataMember]
        public string price { get; set; }
        [DataMember]
        public List<VisibleInstructor> visible_instructors { get; set; }
        [DataMember]
        public string image_125_H { get; set; }
        [DataMember]
        public string image_240x135 { get; set; }
        [DataMember]
        public string image_480x270 { get; set; }
        [DataMember]
        public string published_title { get; set; }
        [DataMember]
        public string headline { get; set; }
        [DataMember]
        public DiscountPrice discount_price { get; set; }
        [DataMember]
        public double avg_rating { get; set; }
        [DataMember]
        public int num_reviews { get; set; }
        [DataMember]
        public int num_published_lectures { get; set; }
        [DataMember]
        public string image_304x171 { get; set; }
        [DataMember]
        public string instructional_level { get; set; }
        [DataMember]
        public string content_info { get; set; }
    }
}
