using System.Web.Optimization;

namespace DBSoft.ShoppingList.Web
{
	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
            bundles.Add(new ScriptBundle("~/bundles/lib")
                .IncludeDirectory("~/Scripts/lib", "*.js"));

			//bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
			//			"~/Scripts/jquery-ui*"));

			//bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
			//			"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new StyleBundle("~/styles/css")
				.Include("~/Content/site.css"));
		}
	}
}