using System.Collections.Generic;

namespace DBSoft.ShoppingList.Web.Models
{
    using global::ShoppingList.Mobile;

    public class ShoppingListModel
    {
        public ShoppingListModel()
        {
            Items = new List<Item>();
        }

        public IEnumerable<Item> Items { get; set; } 
    }
}