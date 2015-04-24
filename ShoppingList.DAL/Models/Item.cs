using Microsoft.WindowsAzure.Storage.Table;

namespace DBSoft.ShoppingList.DAL.Models
{
	public class Item : TableEntity
	{
		public Item()
		{
		}
		public Item(string guid, string name)
		{
			RowKey = guid;
            PartitionKey = guid;
			ItemName = name;
		}
		public string ItemName { get; set; }
	}
}
