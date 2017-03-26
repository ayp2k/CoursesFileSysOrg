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
    static class WebUtils
    {
        public static async Task<string> GetWebContentAsync(string serverURL, string userAgent = null, string urlEncoding = null)
        {
            string SearchPageHTML;
            using (WebClient client = new WebClient())
            {
                if (userAgent != null)
                    client.Headers["User-Agent"] = userAgent;
                if (urlEncoding == null)
                    client.Encoding = System.Text.Encoding.GetEncoding("utf-8");
                else
                    client.Encoding = System.Text.Encoding.GetEncoding(urlEncoding);
                SearchPageHTML = await client.DownloadStringTaskAsync(serverURL);
            }
            return SearchPageHTML;
        }

        public static string SerializeJSON<T>(this T obj)
        {
            if (obj == null)
                throw new NullReferenceException();

            string retVal;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                retVal = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            return retVal;
        }

        public static T DeserializeJSON<T>(this string json) where T : class
        {
            if (json == null)
                throw new NullReferenceException();

            T obj = Activator.CreateInstance<T>();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = serializer.ReadObject(memoryStream) as T;
            }
            return obj;
        }

        public static T DeSerializeJsonToObject<T>(string jsonResult) where T : class
        {
            T resultObject;
            
            using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(jsonResult)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                resultObject = serializer.ReadObject(memoryStream) as T;
            }
            return resultObject;
        }
    }
}
