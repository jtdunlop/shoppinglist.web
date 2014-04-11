using System.Collections.Generic;
using System.Linq;
using DBSoft.ShoppingList.DAL.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DBSoft.ShoppingList.DAL.Services
{
	public class ItemService
	{
		private readonly CloudTable _table;

		public ItemService()
		{
			var storage = CloudStorageAccount.Parse(new AppConfig().GetConnectionString("shoppingListStorage"));
			var client = storage.CreateCloudTableClient();
			_table = client.GetTableReference("items");
			_table.CreateIfNotExists();
		}

		public IEnumerable<Item> GetItems()
		{
			var query = new TableQuery<Item>();
			return _table.ExecuteQuery(query).ToList();
		}

		public void Add(string item)
		{
			var add = new Item(item);
			var op = TableOperation.Insert(add);
			_table.Execute(op);
		}

		public void Remove(string item)
		{
			var get = TableOperation.Retrieve<Item>(item, item);
			var result = _table.Execute(get);
			var remove = (Item)result.Result;
			var op = TableOperation.Delete(remove);
			result = _table.Execute(op);
		}
	}
}
