using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    class MetaData
    {
        public string Description { get; set; }
        public string PublicationDate { get; set; }
        public string MainCategory { get; set; }

        public List<string> Categories
        {
            get
            {
                if (this.Categories == null)
                    this.Categories = new List<string>();
                return this.Categories;
            }
            set
            {
                if (this.Categories == null)
                    this.Categories = new List<string>();
                this.Categories = value;
            }
        }

        public List<string> Tags
        {
            get
            {
                if (this.Tags == null)
                    this.Tags = new List<string>();
                return this.Tags;
            }
            set
            {
                if (this.Tags == null)
                    this.Tags = new List<string>();
                this.Tags = value;
            }
        }
    }
}
