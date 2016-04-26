using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    static class StringUtils
    {
        public static string CleanUpFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "-"));
        }

        public static string CleanUpPathName(this string fileName)
        {
            return Path.GetInvalidPathChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "-"));
        }
    }
}
