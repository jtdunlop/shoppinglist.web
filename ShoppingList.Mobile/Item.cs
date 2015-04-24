using System;

namespace ShoppingList.Mobile
{
    public class Item : IComparable
    {
        public int Id { get; set; }
        public string ItemId { get; set; }
        [PrimaryKey]
        public string ItemName { get; set; }
        public int AddFlag { get; set;}
        public int DeleteFlag { get; set; }
        public int CompareTo(object obj)
        {
            var compare = obj as Item;
            if (compare != null) return String.CompareOrdinal(ItemName, compare.ItemName);
            return 0;
        }
    }
}