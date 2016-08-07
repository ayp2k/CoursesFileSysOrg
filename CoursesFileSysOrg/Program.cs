using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            // Hard Coded Folder Location
            //Application.WorkingDirectory =  @""; 

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

            List<Course> courseSearchResults = publisher.SearchCourse(queryCourseName).Result;
            switch (courseSearchResults.Count)
            {
                case 0:
                    Console.WriteLine("Course name:\"{0}\" not found @ {1} web site!", queryCourseName, queryPublisherName);
                    Environment.Exit(0);
                    break;
                case 1:
                    course = courseSearchResults[0];
                    break;
                default: //(courseSearchResults.Count > 1)
                    {
                        Menu coursesMenu = new Menu("Please choose relevant course OR '0' to exit");
                        coursesMenu.MenuItemFormat = "({0}) - \"{1}\"";
                        coursesMenu.AddItems(courseSearchResults.Select(x => x.Name).ToList());
                        coursesMenu.Render();

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
                    break;
            }
            publisher.Course = course;
            Console.WriteLine("...Processing course name: \"{0}\" @ \"{1}\"", publisher.Course.Name, publisher.Name);
            //publisher.Course.GetCourseWebPage();
            publisher.PopulateAllCourseItems();
            course.Description = publisher.GetCourseMetaDataDescription();
            course.Categories = publisher.GetCourseMetaDataCategories();

            Menu mainMenu = new Menu();
            mainMenu.AddItems(new List<string> {
                                                "List Course Folders",
                                                "List Course Folders & Video items",
                                                "Create Folders Structure",
                                                "Rename (& Move) Video Files by File Numiric Index",
                                                "Rename (& Move) Video Files by Video Length",
                                                "Print Metadata",
                                                "Open Course Page in Browser",
                                                });
            do
            {
                mainMenu.Render();
                keyPressed = Console.ReadKey();
                Console.WriteLine();
                switch (keyPressed.KeyChar)
                {
                    case '0':
                        // exit;
                        continue;
                    case '1':
                        // List Course Folders & Video items
                        Application.ListFormatedFolders(course);
                        break;
                    case '2':
                        // List Course Folders & Video items
                        Application.ListFormatedItems(course);
                        break;
                    case '3':
                        // Create Folders Structure
                        Application.CreateFolders(course);
                        break;
                    case '4':
                        // Rename (& move) Video Files .... match by numeric index
                        Application.FilesRenameAndMoveByNumericIndex(course);
                        break;
                    case '5':
                        // Rename (& move) Video Files .... match by video length
                        Application.FilesRenameAndMoveByVideoLength(course);
                        break;
                    case '6':
                        // Display Metadata ...
                        Application.DisplayMetadata(course);
                        break;
                    case '7':
                        // open course page in browser
                        Application.OpenCourseInBrowser(course);
                        break;
                    default:
                        Console.WriteLine("option not supported!");
                        break;
                }
            }
            while (keyPressed.KeyChar != '0');
        }
    }
}
