using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    static class Application
    {
        private static List<string> fileExtensions = new List<string>() { ".mp4", ".mov", ".wmv", ".flv", ".f4v", ".m4a" };

        public static string WorkingDirectory { get; set; }

        private static string GetWorkingDirectory()
        {
            return (WorkingDirectory == null || WorkingDirectory == string.Empty) ? Environment.CurrentDirectory : WorkingDirectory;
        }

        public static void ListFormatedFolders(Course course)
        {
            foreach (var chapter in course.Chapters)
            {
                Console.WriteLine("[{0:D2}] {1}", chapter.VideoItems.Count, chapter.GetFormatedName(course.GetChaptersNumOfDigits));
            }
        }

        public static void ListFormatedItems(Course course)
        {
            foreach (var chapter in course.Chapters)
            {
                Console.WriteLine();
                Console.WriteLine("{0}", chapter.GetFormatedName(course.GetChaptersNumOfDigits));
                foreach (var videoitem in chapter.VideoItems)
                {
                    Console.WriteLine("{0:D" + course.GetVideoItemsNumOfDigits.ToString() + "} » [{3,7}] » {1:D2}.{2:D2}", videoitem.globalIndex, chapter.id, videoitem.GetFormatedName, videoitem.TimeStamp);
                }
            }
        }

        public static void CreateFolders(Course course)
        {
            foreach (var chapter in course.Chapters)
            {
                //string folderName = string.Format("{0}", chapter.GetFormatedFolderName);
                if (!Directory.Exists(chapter.GetFormatedFolderName(course.GetChaptersNumOfDigits)))
                {
                    Directory.CreateDirectory(chapter.GetFormatedFolderName(course.GetChaptersNumOfDigits));
                    Console.WriteLine("\"{0}\" Foder Created", chapter.GetFormatedFolderName(course.GetChaptersNumOfDigits));
                }
                else // already exist
                {
                    Console.WriteLine("\"{0}\" Folder Already Exist", chapter.GetFormatedFolderName(course.GetChaptersNumOfDigits));
                }
            }
        }

        public static void FilesRenameAndMoveByNumericIndex(Course course)
        {
            var searchLocation = SearchOption.TopDirectoryOnly;
            DirectoryInfo currentDirectory = new DirectoryInfo(GetWorkingDirectory());
            var origenalFilesNames = currentDirectory.EnumerateFiles("*", searchLocation)
                .Where(e => fileExtensions.Contains(e.Extension.ToLower()))
                .OrderBy(f => f.Name.GetNumericIndex())
                .ToArray();
            if (origenalFilesNames.Length == 0)
            {
                searchLocation = SearchOption.AllDirectories;
                origenalFilesNames = currentDirectory.EnumerateFiles("*", searchLocation)
                    .Where(e => fileExtensions.Contains(e.Extension.ToLower()))
                    .OrderBy(f => (f.Name.GetNumericIndex() + (f.Directory.Name.GetNumericIndex() * 1000)))
                    .ToArray();
            }

            var newFilesNames = course.Chapters.SelectMany(
                x => x.VideoItems, (i, j) => new
                {
                    folderid = i.id,
                    folderName = i.Name,
                    formatedFolderName = i.GetFormatedFolderName(course.GetChaptersNumOfDigits),
                    videoName = j.Name,
                    videoLocalIndex = j.localIndex,
                    videoGlobalIndex = j.globalIndex,
                    videoFormatedFileName = j.GetFormatedFileName(course.GetVideoItemsNumOfDigits),
                    j.IsVideo
                }).Where(x => x.IsVideo == true).ToArray();

            if (origenalFilesNames.Length == newFilesNames.Length)
            {
                int fromLength = (origenalFilesNames.Length > 0) ? origenalFilesNames.Max(x => x.Name.Length) : 0;
                // 2---> the two quotation marks
                Console.WriteLine("{0,-" + (fromLength + 2) + "} {1}", "From Name:", "To Name:");
                Console.WriteLine("{0,-" + (fromLength + 2) + "} {1}", "----------", "--------");

                int folderIndex = 0;
                for (int i = 0; i < origenalFilesNames.Length; i++)
                {
                    // insert break between course chapters
                    if (newFilesNames[i].folderid > folderIndex)
                    {
                        Console.WriteLine();
                        folderIndex++;
                    }
                    //Console.WriteLine("Renaming file: \t\"{0,-80}\" to: \t\"{1}{2}\"", origenalFilesNames[i].Name,
                    //    newFilesNames[i].videoFormatedFileName, origenalFilesNames[i].Extension);
                    Console.WriteLine("\"{0}\"{1," + (fromLength - origenalFilesNames[i].Name.Length).ToString() + "} \"{2}{3}\"",
                        origenalFilesNames[i].Name,
                        string.Empty,
                        newFilesNames[i].videoFormatedFileName, origenalFilesNames[i].Extension);
                }
                Console.WriteLine("Continue... (Y/n)");
                var getConfirmationKey = Console.ReadKey();
                Console.WriteLine();
                if (getConfirmationKey.KeyChar == 13 || getConfirmationKey.KeyChar.ToString().ToLower() == "y")
                {
                    Console.WriteLine("Renaming...");
                    Console.WriteLine(" » (id):new name");
                    for (int j = 0; j < origenalFilesNames.Length; j++)
                    {
                        var fullFilePath = origenalFilesNames[j].DirectoryName + "\\";
                        if (searchLocation == SearchOption.TopDirectoryOnly)
                            fullFilePath += newFilesNames[j].formatedFolderName + "\\";

                        Console.WriteLine(" » {0,3}: {1}{2}{3}", j + 1,
                        fullFilePath, newFilesNames[j].videoFormatedFileName, origenalFilesNames[j].Extension);

                        origenalFilesNames[j].MoveTo(string.Format("{0}{1}{2}",
                            fullFilePath, newFilesNames[j].videoFormatedFileName, origenalFilesNames[j].Extension));
                    }
                }
            }
            else
                Console.WriteLine("Abort, incompatible number of files found to be renamed...({0} exist vs {1} expected)", origenalFilesNames.Length, newFilesNames.Length);
        }

        public static void FilesRenameAndMoveByVideoLength(Course course)
        {
            var searchLocation = SearchOption.TopDirectoryOnly;
            DirectoryInfo currentDirectory = new DirectoryInfo(GetWorkingDirectory());
            var origenalFilesNames = currentDirectory.EnumerateFiles("*", searchLocation)
                .Where(e => fileExtensions.Contains(e.Extension.ToLower()))
                //.OrderBy(f => TagLib.File.Create(f.FullName).Properties.Duration)
                .ToArray();
            if (origenalFilesNames.Length == 0)
            {
                searchLocation = SearchOption.AllDirectories;
                origenalFilesNames = currentDirectory.EnumerateFiles("*", searchLocation)
                    .Where(e => fileExtensions.Contains(e.Extension.ToLower()))
                    //.OrderBy(f => TagLib.File.Create(f.FullName).Properties.Duration)
                    .ToArray();
            }

            VideoItem[] filesMatchIndex = new VideoItem[origenalFilesNames.Length];
            int arrayIndex = 0;

            foreach (var file in origenalFilesNames)
            {
                TimeSpan fileDuration = TagLib.File.Create(file.FullName).Properties.Duration;
                try
                {
                    VideoItem fileMatch = course.Chapters.SelectMany(c => c.VideoItems)
                                    .Where(x => x.TimeStamp != null
                                    && x.TimeStamp.GetMinutes() == fileDuration.Minutes.ToString()
                                    && x.TimeStamp.GetSeconds() == fileDuration.Seconds.ToString())
                                    .Single();
                    filesMatchIndex[arrayIndex++] = fileMatch;
                }
                catch (Exception e) // "Sequence contains more than one element exception" OR "Sequence contains no elements"
                {
                    arrayIndex++;
                }
            }

            if (origenalFilesNames.Length == 0)
            {
                Console.WriteLine("no files fonud!...");
                return;
            }
            int fromLength = (origenalFilesNames.Length > 0) ? origenalFilesNames.Max(x => x.Name.Length) : 0;
            // 2---> the two quotation marks
            Console.WriteLine("{0,-" + (fromLength + 2) + "} {1}", "From Name:", "To Name:");
            Console.WriteLine("{0,-" + (fromLength + 2) + "} {1}", "----------", "--------");

            for (int i = 0; i < origenalFilesNames.Length; i++)
            {
                if (filesMatchIndex[i] == null)
                    continue;

                Console.WriteLine("\"{0}\"{1," + (fromLength - origenalFilesNames[i].Name.Length).ToString() + "} \"{2}{3}\"",
                        origenalFilesNames[i].Name,
                        string.Empty, // header place holder
                        filesMatchIndex[i].GetFormatedFileName(course.GetChaptersNumOfDigits),
                        origenalFilesNames[i].Extension);
                Console.WriteLine("[{0}]--->[{1}]", 
                        TagLib.File.Create(origenalFilesNames[i].FullName).Properties.Duration.ToString().Substring(3, 5),
                        filesMatchIndex[i].TimeStamp);
            }
            Console.WriteLine("Continue... (Y/n)");
            var getConfirmationKey = Console.ReadKey();
            Console.WriteLine();
            if (getConfirmationKey.KeyChar == 13 || getConfirmationKey.KeyChar.ToString().ToLower() == "y")
            {
                Console.WriteLine("Renaming...");
                Console.WriteLine(" » (id):new name");
                for (int j = 0; j < origenalFilesNames.Length; j++)
                {
                    if (filesMatchIndex[j] == null)
                        continue;

                    var fullFilePath = origenalFilesNames[j].DirectoryName + "\\";

                    Console.WriteLine(" » {0,3}: {1}{2}{3}", j + 1,
                    fullFilePath, filesMatchIndex[j].GetFormatedFileName(course.GetChaptersNumOfDigits), origenalFilesNames[j].Extension);

                    origenalFilesNames[j].MoveTo(string.Format("{0}{1}{2}",
                        fullFilePath, filesMatchIndex[j].GetFormatedFileName(course.GetChaptersNumOfDigits), origenalFilesNames[j].Extension));
                }
            }
        }

        public static void DisplayMetadata(Course course)
        {
            if (course.Description != null || course.Description != string.Empty)
            {
                Console.WriteLine("Description: \n{0}", course.Description.Replace(". ", ".\n").Replace("? ", "?\n").Replace("! ", "!\n"));
                Console.WriteLine("");
            }
            if (course.Categories.Count > 0)
            {
                Console.WriteLine("Categories: {0}", course.Categories.Aggregate((x, y) => x + " | " + y));
                Console.WriteLine("");
            }
        }

        public static void OpenCourseInBrowser(Course course)
        {
            //ProcessStartInfo browserStart = new ProcessStartInfo();
            //browserStart.Arguments = course.URL;
            Process.Start(course.URL);
        }        
    }

}
