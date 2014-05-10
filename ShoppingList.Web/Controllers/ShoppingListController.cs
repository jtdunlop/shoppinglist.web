using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using ShoppingList.Mobile;

namespace DBSoft.ShoppingList.Web.Controllers
{
    public class ShoppingListController : Controller
    {
		private readonly ShoppingListService _service;

		public ShoppingListController()
		{
			_service = new ShoppingListService(false);
		}

		public ActionResult Index()
        {
			var counter = 0;
			var model = new ShoppingListModel
			{
				Items = _service.GetShoppingList().Select(f => new Item { ItemName = f, Id = ++counter })
			};
            return View(model);
        }

	    [HttpPost]
		public JsonResult SaveItem(string item)
	    {
			_service.AddItem(item);
			return Json("OK");
	    }

		[HttpPost]
		public JsonResult RemoveItem(string item)
		{
			_service.RemoveItem(item);
			return Json("OK");
		}
    }

	public class ShoppingListModel
	{
		public ShoppingListModel()
		{
			Items = new List<Item>();
		}

		public IEnumerable<Item> Items { get; set; } 
	}
}
