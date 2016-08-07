using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesFileSysOrg
{
    class Menu
    {
        private Dictionary<string, string> menuItems;
        private int menuIndex;

        public String Title { get; set; }
        public string MenuItemFormat { get; set; }

        public Menu()
        {
            menuItems = new Dictionary<string, string>();
            menuIndex = 1;
            Title = string.Empty;
            MenuItemFormat = "{0}. {1}";
        }

        public Menu(string title) : this()
        {
            Title = title;
        }

        public void AddItem(string itemName)
        {
            this.menuItems.Add((menuIndex++).ToString(), itemName);
        }

        public void AddItems(List<string> itemsNames)
        {
            foreach(var item in itemsNames)
            {
                AddItem(item);
            }
        }

        public void Render()
        {
            if (Title != string.Empty)
            {
                Console.WriteLine(Title);
                renderSeparator(Title.Length);
            }

            Console.WriteLine();
            foreach(var item in menuItems)
            {
                Console.WriteLine(MenuItemFormat, item.Key, item.Value);
            }
            Console.WriteLine(MenuItemFormat, '0', "Exit");
            Console.WriteLine();
        }

        private void renderSeparator(int separatorLength, char separatorChar = '-')
        {
            for (int i = 0; i < separatorLength; i++)
            {
                Console.Write(separatorChar);
            }
        }
    }
}
