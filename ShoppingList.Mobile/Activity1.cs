using Android.App;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;

namespace ShoppingList.Mobile
{
#if DEBUG
	[Activity(Label = "ShoppingList.Mobile", MainLauncher = true, Icon = "@drawable/icon")]
#else
	[Activity(Label = "ShoppingList.Mobile.Web", MainLauncher = true, Icon = "@drawable/icon")]
#endif
	public class HomeScreen : ListActivity 
	{
		private ShoppingListService _shoppingListService;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			_shoppingListService = new ShoppingListService(Build.Brand == "generic");
			_shoppingListService.UpgradeDatabase(this);
			InitializeUI();

			Refresh();
		}

		private void InitializeUI()
		{
			ListView.ChoiceMode = ChoiceMode.Multiple;
			var buttonView = new LinearLayout(this)
			{
				Orientation = Orientation.Horizontal
			};
			CreateButton("Add", AddClick, buttonView);
			CreateButton("Remove", RemoveClick, buttonView);
			CreateButton("Refresh", RefreshClick, buttonView);
			ListView.AddFooterView(buttonView);
		}

		private void CreateButton(string text, EventHandler click, ViewGroup buttonView)
		{
			var button = new Button(this)
			{
				Text = text
			};
			button.Click += click;
			buttonView.AddView(button);
		}

		private void RefreshClick(object sender, EventArgs e)
		{
			Refresh();
		}

		private void RemoveClick(object sender, EventArgs e)
		{
			var positions = ListView.CheckedItemPositions;
			for ( var i = 0; i < positions.Size(); i++ )
			{
				if (!positions.ValueAt(i)) continue;
				var item = (string)ListView.GetItemAtPosition(positions.KeyAt(i));
				RemoveItem(item);
			}
		}

		private void Refresh()
		{
			var results = _shoppingListService.GetShoppingList();
			RunOnUiThread(() =>
			{
				ListAdapter = new ArrayAdapter<string>(this,
					Android.Resource.Layout.SimpleListItemMultipleChoice, results.ToArray());
				Title = _shoppingListService.Message;
			});
		}

		private void AddClick(object sender, EventArgs e)
		{
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle("Title");

			var input = new EditText(this) { InputType = InputTypes.ClassText };

			builder.SetView(input)
				.SetTitle("Item")
				.SetPositiveButton("OK", (s1, args) => AddItem(input.Text))
				.SetNegativeButton("Cancel", (s2, args) =>
				{
				})
				.Show();
		}

		private void AddItem(string text)
		{
			_shoppingListService.AddItem(text);
			Refresh();
		}

		private void RemoveItem(string text)
		{
			_shoppingListService.RemoveItem(text);
			Refresh();
		}
	}
}

