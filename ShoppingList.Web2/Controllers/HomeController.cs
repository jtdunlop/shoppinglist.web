namespace DbSoft.ShoppingList.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using DBSoft.ShoppingList.Web.Models;
    using global::ShoppingList.Mobile;

    public class HomeController : Controller
    {
        private readonly ShoppingListService _service;

		public HomeController()
		{
			_service = new ShoppingListService(false);
		}        
        
        public ActionResult Index()

        {
            var counter = 0;
            var model = new ShoppingListModel
            {
                Items = _service.GetShoppingList().Select(f => new Item { Id = ++counter, ItemName = f.ItemName, ItemId = f.ItemId })
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
}