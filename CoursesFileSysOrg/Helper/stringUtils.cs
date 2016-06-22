using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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

        public static int GetNumericIndex(this string name)
        {
            int result;
            foreach (var x in name.Split(new char[] { ' ', '_', '-', '.' }))
            {
                if (int.TryParse(x, out result)) return result;
            }
            return 0;
        }

        public static string SerializeJSON<T>(this T obj)
        {
            if (obj == null)
                throw new NullReferenceException();

            string retVal;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                retVal = Encoding.UTF8.GetString(ms.ToArray());
            }
            return retVal;
        }
        
        public static T DeserializeJSON<T>(this string json)
        {
            if (json == null)
                throw new NullReferenceException();

            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
            }
            return obj;
        }
    }
}
