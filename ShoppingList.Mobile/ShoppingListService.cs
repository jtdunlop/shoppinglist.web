using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
		private readonly string _serviceAddress;
		private readonly RestClient _client;
		private readonly string _path;
		private const int Version = 4;

		public ShoppingListService(bool emulated)
		{
			_emulated = emulated;
			_serviceAddress = GetServiceAddress();
			_client = new RestClient(_serviceAddress);
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
#if true
			return string.Format("http://{0}/shoppinglist.services", _emulated ? "10.0.2.2" : "192.168.0.102");
#else
			return "http://shopping-list.azurewebsites.net";
#endif
		}

		public IEnumerable<string> GetShoppingList()
		{
			List<string> results = null;
			try
			{
#if ANDROID
				SyncAdditions();
				SyncDeletions();
#endif
				var url = String.Format("{0}/api/item/getitems", _serviceAddress);
				var req = (HttpWebRequest)WebRequest.Create(new Uri(url));
				using (var response = req.GetResponse())
				{
					var stream = response.GetResponseStream();
					if (stream != null)
						using (var reader = new StreamReader(stream, Encoding.UTF8))
						{
							results = JsonConvert.DeserializeObject<IEnumerable<string>>(reader.ReadToEnd()).ToList();
						}
				}
			}
// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
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
							conn.Insert(new Item { ItemName = result });
						}
					}
					results = conn.Table<Item>().Where(f => f.DeleteFlag == 0).ToList().Select(f => f.ItemName).OrderBy(f => f).ToList();
					conn.Commit();
				}
#endif
			}
			return results.OrderBy(f => f);
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
					var response = _client.Execute(request);
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
					var response = _client.Execute(request);
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
			var request = new RestRequest("api/item/{item}", Method.DELETE);
			request.AddUrlSegment("item", text);
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

	public class Item
	{
		[PrimaryKey]
		public string ItemName { get; set; }
		public int AddFlag { get; set;}
		public int DeleteFlag { get; set; }
	}
}