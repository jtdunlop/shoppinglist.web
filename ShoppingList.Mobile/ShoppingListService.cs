using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
#if ANDROID
using Android.Content;
#endif
using Newtonsoft.Json;
using RestSharp;

namespace ShoppingList.Mobile
{
	public class ShoppingListService
	{
		public string Message { get; set; }
		private readonly bool _emulated;
	    private readonly RestClient _client;
		private readonly string _path;
		private const int Version = 5;

		public ShoppingListService(bool emulated)
		{
		    _emulated = emulated;
			var serviceAddress = GetServiceAddress();
			_client = new RestClient(serviceAddress);
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			_path = Path.Combine(documents, "shopping-list.db");
		}

#if ANDROID
		public void UpgradeDatabase(HomeScreen context)
		{
			using ( var db = context.OpenOrCreateDatabase(_path, FileCreationMode.Private, null) )
			{
				if (db.Version == Version) return;
				db.Close();
				context.DeleteDatabase(_path);
				using (var db2 = context.OpenOrCreateDatabase(_path, FileCreationMode.Private, null))
				{
					db2.Version = Version;
					db2.Close();
				}
				using (var conn = new SQLiteConnection(_path))
				{
					conn.CreateTable<Item>();
					conn.Commit();
				}
				Message = "Delete";
			}
		}
#endif

		private string GetServiceAddress()
		{
            // return "http://192.168.0.101";
            return "http://shopping-list.azurewebsites.net";
#if DEBUG
            Message = "?";
			return string.Format("http://{0}/shoppinglist.services", _emulated ? "10.0.2.2" : "192.168.0.101");
#else
            Message = "Hmm";
            return "http://shopping-list.azurewebsites.net";
#endif
		}

		public IEnumerable<Item> GetShoppingList()
		{
			List<Item> results = null;
			try
			{
#if ANDROID
				SyncAdditions();
				SyncDeletions();
#endif
                var request = new RestRequest("api/item/getitems", Method.GET);
                var response = _client.Execute(request);
                if ( response.ResponseStatus == ResponseStatus.Completed && response.StatusCode != HttpStatusCode.InternalServerError )
                {
                    results = JsonConvert.DeserializeObject<IEnumerable<Item>>(response.Content).ToList();
                }
                else
                {
                    throw new Exception(response.Content);
                }
			}
// ReSharper disable once EmptyGeneralCatchClause
			catch ( Exception e)
			{
				var s = e.Message;
			}
			finally
			{
#if ANDROID	
				using (var conn = new SQLiteConnection(_path))
				{
					conn.CreateTable<Item>();
					if (results != null)
					{
						conn.DeleteAll<Item>();
						foreach (var result in results)
						{
							conn.Insert(new Item { ItemName = result.ItemName, ItemId = result.ItemId });
						}
					}
					results = conn.Table<Item>().Where(f => f.DeleteFlag == 0).ToList().Select(f => f).OrderBy(f => f).ToList();
					conn.Commit();
				}
#endif
			}
			return results.OrderBy(f => f.ItemName);
		}

		private void SyncDeletions()
		{
			using ( var conn = new SQLiteConnection(_path))
			{
				var deletions = conn.Table<Item>().Where(f => f.DeleteFlag == 1);
				foreach ( var deletion in deletions )
				{
					var request = new RestRequest("api/item/{item}", Method.DELETE);
					request.AddUrlSegment("item", deletion.ItemName);
					_client.Execute(request);
				}
				conn.Commit();
			}
		}

		private void SyncAdditions()
		{
			using (var conn = new SQLiteConnection(_path))
			{
				var additions = conn.Table<Item>().Where(f => f.AddFlag == 1);
				foreach (var addition in additions)
				{
					var request = new RestRequest("api/item?id=" + addition.ItemName, Method.POST);
					_client.Execute(request);
				}
				conn.Commit();
			}
		}

		public void AddItem(string text)
		{
			var request = new RestRequest("api/item?id=" + text, Method.POST);
			var response = _client.Execute(request);
			if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.NoContent) return;
#if ANDROID
			using (var conn = new SQLiteConnection(_path))
			{
				conn.Insert(new Item { ItemName = text, AddFlag = 1 });
				conn.Commit();
			}
#endif
		}
		
		public void RemoveItem(string text)
		{
			var request = new RestRequest("api/item?id=" + text, Method.DELETE);
			var response = _client.Execute(request);
			if (response.ResponseStatus == ResponseStatus.Completed && response.StatusCode == HttpStatusCode.NoContent) return;
#if ANDROID
			using (var conn = new SQLiteConnection(_path))
			{
				var item = conn.Get<Item>(text);
				item.DeleteFlag = 1;
				conn.Update(item);
				conn.Commit();
			}
#endif
		}
	}
}