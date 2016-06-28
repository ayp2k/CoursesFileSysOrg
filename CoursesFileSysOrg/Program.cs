using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    class Program
    {
        const string QueryPlaceHolder = "{query}";
        
        static void Main(string[] args)
        {
            string queryPublisherName, queryCourseName;
            ConsoleKeyInfo keyPressed;

            if (args.Length > 0)
            {
                try
                {
                    // TODO: handle the '–' char
                    queryPublisherName = args[0].Substring(0, args[0].IndexOf('-')).TrimEnd();
                    queryCourseName = args[0].Substring(args[0].IndexOf('-') + 1).TrimStart();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Current Parameter: '{0}'", args[0]);
                    Console.WriteLine("ERROR: error parsing passed param");
                    Console.WriteLine(e.Message);
                    return;
                }

            }
            else // Use Folder Name
            {
                string dirName = new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
                try
                {
                    queryPublisherName = dirName.Substring(0, dirName.IndexOf('-')).TrimEnd();
                    queryCourseName = dirName.Substring(dirName.IndexOf('-') + 1).TrimStart();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Current Directory: {0}", dirName);
                    Console.WriteLine("ERROR: error parsing folder name [{0}]", dirName);
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            Console.WriteLine("Searching for course name: \"{1}\" @ \"{0}.com\"", queryPublisherName, queryCourseName);



            Publisher publisher = Publisher.Create(queryPublisherName);
            if (publisher == null)
            {
                Console.WriteLine("Publisher Name \"{0}\" not found!", queryPublisherName);
                Environment.Exit(0);
            }

            Course course = new Course();

            List<Course> courseSearchResults = publisher.SearchCourse(queryCourseName);
            if (courseSearchResults.Count == 1)
            {
                course = courseSearchResults[0];
            }
            else if (courseSearchResults.Count > 1)
            {
                int indexer = 0;
                Console.WriteLine();
                Console.WriteLine("Please choose relevant course OR '0' to exit");
                foreach (Course item in courseSearchResults)
                {
                    Console.WriteLine("({0}) - \"{1}\"", ++indexer, item.Name);
                }
                keyPressed = Console.ReadKey();
                Console.WriteLine();
                if (keyPressed.KeyChar == '0')
                    Environment.Exit(0);
                else if (keyPressed.KeyChar > '0' && Convert.ToInt32(keyPressed.KeyChar.ToString()) <= (courseSearchResults.Count))
                {
                    course = courseSearchResults[Convert.ToInt16(keyPressed.KeyChar.ToString()) - 1];
                }
                else
                { 
                    Console.WriteLine("Unrecognized option keypress");
                    Environment.Exit(0);
                }
            }

            publisher.Course = course;
            Console.WriteLine("...Processing course name: \"{0}\" @ \"{1}\"", publisher.Course.Name, publisher.Name);
            //publisher.Course.GetCourseWebPage();
            publisher.PopulateAllCourseItems();
            course.Description = publisher.GetCourseMetaDataDescription();
            course.Categories = publisher.GetCourseMetaDataCategories();

            do
            {
                Console.WriteLine();
                Console.WriteLine("1. List Course Folders & Video items");
                Console.WriteLine("2. Create Folders Structure");
                Console.WriteLine("3. Rename & Move Video Files");
                Console.WriteLine("4. Move Files to Folders");
                Console.WriteLine("5. Print Metadata");
                Console.WriteLine("0. Exit");
                Console.WriteLine();

                keyPressed = Console.ReadKey();
                Console.WriteLine();
                switch (keyPressed.KeyChar)
                {
                    case '0':
                        // exit;
                        continue;
                    case '1':
                        // List Course Folders & Video items
                        ListFormatedItems(course);
                        break;
                    case '2':
                        // Create Folders Structure
                        CreateFolders(course);
                        break;
                    case '3':
                        // Rename Video Files 
                        FilesRenameAndMove(course);
                        break;
                    case '4':
                        // Move Video Files to SubFolders...
                        break;
                    case '5':
                        // Display Metadata ...
                        DisplayMetadata(course);
                        break;
                    default:
                        Console.WriteLine("option not supported!");
                        break;
                }
            }
            while (keyPressed.KeyChar != '0');

        }

        private static void ListFormatedItems(Course course)
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

        private static void CreateFolders(Course course)
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

        private static void FilesRenameAndMove(Course course)
        {
            var searchLocation = SearchOption.TopDirectoryOnly;
            DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var origenalFilesNames = currentDirectory.EnumerateFiles("*", searchLocation).OrderBy(f => f.Name.GetNumericIndex())
                .Where(f => f.Extension.ToLower() == ".mp4"
                        || f.Extension.ToLower() == ".mov"
                        || f.Extension.ToLower() == ".flv").ToArray();
            if (origenalFilesNames.Length == 0)
            {
                searchLocation = SearchOption.AllDirectories;
                origenalFilesNames = currentDirectory.EnumerateFiles("*", searchLocation).OrderBy(f => (f.Name.GetNumericIndex() + (f.Directory.Name.GetNumericIndex() * 1000)))
                    .Where(f => f.Extension.ToLower() == ".mp4"
                        || f.Extension.ToLower() == ".mov"
                        || f.Extension.ToLower() == ".flv").ToArray();
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
                int fromLength = origenalFilesNames.Max(x => x.Name.Length);
                // 2---> the two quotation marks
                Console.WriteLine("{0,-" + (fromLength + 2) + "} {1}", "From Name:", "To Name:");
                Console.WriteLine("{0,-" + (fromLength + 2) + "} {1}", "----------", "--------");

                for (int i = 0; i < origenalFilesNames.Length; i++)
                {
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
                    for (int j = 0; j < origenalFilesNames.Length; j++)
                    {
                        var fullFilePath = origenalFilesNames[j].DirectoryName + "\\";
                        if (searchLocation == SearchOption.TopDirectoryOnly)
                            fullFilePath += newFilesNames[j].formatedFolderName + "\\";

                        Console.WriteLine(" » new {0,3} name: {1}{2}{3}", j + 1,
                        fullFilePath, newFilesNames[j].videoFormatedFileName, origenalFilesNames[j].Extension);

                        origenalFilesNames[j].MoveTo(string.Format("{0}{1}{2}",
                            fullFilePath, newFilesNames[j].videoFormatedFileName, origenalFilesNames[j].Extension));
                    }
                }
            }
            else
                Console.WriteLine("Abort, incompatible number of files found to be renamed...({0} exist vs {1} expected)", origenalFilesNames.Length, newFilesNames.Length);
        }

        private static void DisplayMetadata(Course course)
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
    }
}
