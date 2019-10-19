using System.Collections.Generic;
using System.Linq;

namespace SiteWithAuthentication.WEB.Util.SiteMenu
{
    public class MenuList
    {
        private List<MenuItem> menuItems;

        public MenuList()
        {
            menuItems = new List<MenuItem>() {
                new MenuItem{MenuItemId=1, Header = "Home", Url = "/Home/Index", Order = 1},
                new MenuItem{MenuItemId=2, Header = "About", Url = "/Home/About", Order = 2},
                new MenuItem{MenuItemId=3, Header = "Contact", Url = "/Home/Contact", Order = 3},
                new MenuItem{MenuItemId=40, Header = "Test", Url = "#", Order = 4},
                new MenuItem{MenuItemId=41, Header = "Start test", Url = "/Test/Index", Order = 1, ParentId = 40},
            };
        }

        public List<MenuItem> GetMenu(string role)
        {
            if (role != null) role = role.ToLower();
            switch (role)
            {
                case "user":
                    menuItems.AddRange
                         (
                             new List<MenuItem>()
                             {
                                 new MenuItem{MenuItemId=42, Header = "Create a new test", Url = "/ModeratorSubscription/Index", Order = 2, ParentId = 40},
                                 //new MenuItem{MenuItemId=44, Header = "Your assignments", Url = "/Assignments/Index", Order = 4, ParentId = 40},
                                 new MenuItem{MenuItemId=50, Header = "Test results", Url = "#", Order = 5},
                                 new MenuItem{MenuItemId=51, Header = "Your results", Url = "/TestResults/Index", Order = 1, ParentId = 50},
                             }
                         );
                    break;
                case "moderator":
                    menuItems.AddRange
                         (
                             new List<MenuItem>()
                             {
                                 new MenuItem{MenuItemId=42, Header = "Create a new test", Url = "/ModeratorSubscription/Index", Order = 2, ParentId = 40},
                                 new MenuItem{MenuItemId=43, Header = "Your tests", Url = "/TestManagement/Index", Order = 3, ParentId = 40},
                                 new MenuItem{MenuItemId=44, Header = "Your assignments", Url = "/Assignments/Index", Order = 4, ParentId = 40},
                                 new MenuItem{MenuItemId=50, Header = "Test results", Url = "#", Order = 5},
                                 new MenuItem{MenuItemId=51, Header = "Your results", Url = "/TestResults/Index", Order = 1, ParentId = 50},
                                 new MenuItem{MenuItemId=52, Header = "Subscriber results", Url = "/TestResults/ModeratorCourses", Order = 2, ParentId = 50},
                                 //new MenuItem{MenuItemId=60, Header = "Moderator", Url = "#", Order = 6}
                             }
                         );
                    break;
                case "admin":
                    menuItems.AddRange
                         (
                             new List<MenuItem>()
                             {
                                 new MenuItem{MenuItemId=42, Header = "Create a new test", Url = "/ModeratorSubscription/Index", Order = 2, ParentId = 40},
                                 new MenuItem{MenuItemId=43, Header = "Your tests", Url = "/TestManagement/Index", Order = 3, ParentId = 40},
                                 new MenuItem{MenuItemId=44, Header = "Your assignments", Url = "/Assignments/Index", Order = 4, ParentId = 40},
                                 new MenuItem{MenuItemId=50, Header = "Test results", Url = "#", Order = 5},
                                 new MenuItem{MenuItemId=51, Header = "Your results", Url = "/TestResults/Index", Order = 1, ParentId = 50},
                                 new MenuItem{MenuItemId=52, Header = "Subscriber results", Url = "/TestResults/ModeratorCourses", Order = 2, ParentId = 50},
                                 //new MenuItem{MenuItemId=60, Header = "Moderator", Url = "#", Order = 6},
                                 new MenuItem{MenuItemId=70, Header = "Admin", Url = "#", Order = 7},
                                 new MenuItem{MenuItemId=71, Header = "Test administration", Url = "/Admin/Index", Order = 1, ParentId = 70},
                                 new MenuItem{MenuItemId=72, Header = "Moderator subscription", Url = "/Admin/ModeratorSubcriptions", Order = 2, ParentId = 70},
                                 new MenuItem{MenuItemId=73, Header = "User messages", Url = "/Admin/Messages", Order = 3, ParentId = 70}
                             }
                         );
                    break;
                default:
                    break;
            }
            // Take a value of Children property.
            for (int i = 0, length = menuItems.Count; i < length; i++)
            {
                menuItems[i].Children = (from MenuItem item in menuItems
                                         where item.ParentId == menuItems[i].MenuItemId
                                         select item).ToList();
            }
            return menuItems;
        }
    }
}