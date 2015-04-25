using System.Collections.Generic;
using ShoppingList.Mobile;

namespace DBSoft.ShoppingList.Web.Models
{
    public class ShoppingListModel
    {
        public ShoppingListModel()
        {
            Items = new List<Item>();
        }

        public IEnumerable<Item> Items { get; set; } 
    }
}