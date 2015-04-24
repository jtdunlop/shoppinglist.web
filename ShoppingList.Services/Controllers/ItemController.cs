using System.Web.Mvc;
using DBSoft.ShoppingList.DAL.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DBSoft.ShoppingList.Services.Models;

namespace DBSoft.ShoppingList.Services.Controllers
{
    public class ItemController : ApiController
    {
		private readonly ItemService _service;

		public ItemController()
		{
			_service = new ItemService();
		}

		public IEnumerable<ItemModel> GetItems()
		{
            var result = _service.GetItems().Select(f => new ItemModel { ItemName = f.ItemName, ItemId = f.RowKey }).ToList();
			return result;
		}

		[System.Web.Mvc.HttpPost, ValidateInput(false)]
		public void PostItem(string id)
		{
			_service.Add(id);
		}

		[System.Web.Mvc.HttpDelete, ValidateInput(false)]
		public void DeleteItem(string id)
		{
			_service.Remove(id);
		}
    }
}
