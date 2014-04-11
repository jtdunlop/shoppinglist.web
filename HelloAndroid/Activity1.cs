using Android.App;
using Android.OS;
using Android.Widget;

namespace HelloAndroid
{
	[Activity(Label = "HelloAndroid", MainLauncher = true, Icon = "@drawable/icon")]
	public class Activity1 : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			//Create the user interface in code
			var layout = new LinearLayout(this) {Orientation = Orientation.Vertical};

			var label = new TextView(this) {Text = "Hello, Xamarin.Android"};

			var button = new Button(this) {Text = "Say Hello"};
			button.Click += delegate {
				label.Text = "Hello from the button";
			}; 
			layout.AddView(label);
			layout.AddView(button);
			SetContentView(layout);
		}
	}
}

