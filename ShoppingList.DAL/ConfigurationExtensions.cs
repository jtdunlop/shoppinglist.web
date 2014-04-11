using System.ComponentModel;

namespace DBSoft.ShoppingList.DAL
{
	public static class ConfigurationExtensions
	{
		public static T TryParse<T>(string value) where T : struct
		{
			var result = value.TryParse<T>();
			return result.HasValue ? result.Value : default(T);
		}

		public static T? TryParse<T>(this object o) where T : struct
		{
			if (o == null)
			{
				return null;
			}
			var converter = TypeDescriptor.GetConverter(typeof(T));
			var s = o.ToString();
			var convertFrom = converter.ConvertFrom(s);
			if (convertFrom == null)
			{
				return null;
			}
			return (T)convertFrom;
		}
	}
}