namespace DBSoft.ShoppingList.DAL
{
	using System.Configuration;

	public class AppConfig
	{
		public T GetSetting<T>(string key) where T : struct
		{
			return ConfigurationExtensions.TryParse<T>(GetSetting(key));
		}

		public string GetSetting(string key)
		{
			var setting = ConfigurationManager.AppSettings[key];
			return setting;
		}

		public string GetConnectionString(string key)
		{
			return ConfigurationManager.ConnectionStrings[key].ConnectionString;
		}
	}
}
