using System.Collections.Generic;

namespace SiteWithAuthentication.WEB.Util.SiteMenu
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }                         // Admin assigns MenuItemId himself.
        public string Header { get; set; }                          // Menu header.
        public string Url { get; set; }                             // URL. 
        public int? Order { get; set; }                             // Order number in the Menu/Submenu.
        public int? ParentId { get; set; }                          // Ref to parrent MenuItemId.
        public virtual ICollection<MenuItem> Children { get; set; } // Child menu items.
    }
}
