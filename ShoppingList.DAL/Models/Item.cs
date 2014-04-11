using Microsoft.WindowsAzure.Storage.Table;

namespace DBSoft.ShoppingList.DAL.Models
{
	public class Item : TableEntity
	{
		public Item()
		{
		}
		public Item(string name)
		{
			RowKey = name;
			PartitionKey = name;
			ItemName = name;
		}
		public string ItemName { get; set; }
	}
}
