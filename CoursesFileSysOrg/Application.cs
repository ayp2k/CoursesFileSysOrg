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
        private static string[] fileExtensions = new string[] { ".mp4", ".mov", ".wmv", ".flv", ".f4v", ".m4a" };

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
            DirectoryInfo currentDirectory = new DirectoryInfo(GetWorkingDirectory());
            SearchOption searchLocation = SearchOption.TopDirectoryOnly;
            FileInfo[] origenalFilesNames;

            origenalFilesNames = Application.GetFilesinFolder(currentDirectory,
                                                searchLocation, 
                                                fileExtensions, 
                                                f => f.Name.GetNumericIndex());
            if (origenalFilesNames.Length == 0)
            {
                searchLocation = SearchOption.AllDirectories;
                origenalFilesNames = Application.GetFilesinFolder(currentDirectory,
                                                searchLocation, 
                                                fileExtensions,
                                                f => (f.Name.GetNumericIndex() + (f.Directory.Name.GetNumericIndex() * 1000)));
            }
            var newFilesNames = course.Chapters.SelectMany(
                x => x.VideoItems, (chapter, videoitem) => new
                {
                    folderid = chapter.id,
                    folderName = chapter.Name,
                    formatedFolderName = chapter.GetFormatedFolderName(course.GetChaptersNumOfDigits),
                    videoName = videoitem.Name,
                    videoLocalIndex = videoitem.localIndex,
                    videoGlobalIndex = videoitem.globalIndex,
                    videoFormatedFileName = videoitem.GetFormatedFileName(course.GetVideoItemsNumOfDigits),
                    videoitem.IsVideo
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
                    if (searchLocation == SearchOption.TopDirectoryOnly)
                    {
                        Console.WriteLine("Creating Folders ....");
                        Application.CreateFolders(course);
                        Console.WriteLine();
                    }
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
            DirectoryInfo currentDirectory = new DirectoryInfo(GetWorkingDirectory());
            FileInfo[] origenalFilesNames;
            origenalFilesNames = Application.GetFilesinFolder(currentDirectory, SearchOption.TopDirectoryOnly, fileExtensions, null);
            if (origenalFilesNames.Length == 0)
            {
                origenalFilesNames = Application.GetFilesinFolder(currentDirectory, SearchOption.AllDirectories, fileExtensions, null);
            }
            VideoItem[] filesMatchIndex = new VideoItem[origenalFilesNames.Length];
            int arrayIndex = 0;

            foreach (FileInfo file in origenalFilesNames)
            {
                try
                {
                    TimeSpan fileDuration = TagLib.File.Create(file.FullName).Properties.Duration;

                    var filesMatch = course.Chapters.SelectMany(c => c.VideoItems)
                                    .Where(x => x.TimeStamp != null
                                    && x.TimeStamp.GetMinutes() == fileDuration.Minutes.ToString()
                                    && x.TimeStamp.GetSeconds() == fileDuration.Seconds.ToString());
                    if (filesMatch.Count() == 1)
                        filesMatchIndex[arrayIndex] = filesMatch.SingleOrDefault();
                    arrayIndex++;
                }
                catch (TagLib.CorruptFileException e)
                {
                    Console.WriteLine($"File {file.Name} is Corrupt");
                }
            }

            if (origenalFilesNames.Length == 0)
            {
                Console.WriteLine("no files fonud!...");
                return;
            }
            if (ConfirmePrintedFilesToRename(course, origenalFilesNames, filesMatchIndex))
            {
                RenameAndMoveFiles(course, origenalFilesNames, filesMatchIndex);
            }
        }

        private static Boolean ConfirmePrintedFilesToRename(Course course, FileInfo[] origenalFilesNames, VideoItem[] filesMatchIndex)
        {
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
            if (getConfirmationKey.KeyChar == 13 || getConfirmationKey.KeyChar.ToString().ToLower() == "y")
                return true;
            else
                return false;
        }

        private static void RenameAndMoveFiles(Course course, FileInfo[] origenalFilesNames, VideoItem[] filesMatchIndex)
        {
            Console.WriteLine();
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

        private static FileInfo[] GetFilesinFolder(DirectoryInfo currentDirectory, SearchOption searchLocation, string[] fileExtensions, Func<FileInfo, double> orderBy)
        {
            IEnumerable<FileInfo> filesInfo = currentDirectory.EnumerateFiles("*", searchLocation)
                                    .Where(e => fileExtensions.Contains(e.Extension.ToLower()));
            if (orderBy != null)
                filesInfo = filesInfo.OrderBy(orderBy);
            return filesInfo.ToArray();
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
