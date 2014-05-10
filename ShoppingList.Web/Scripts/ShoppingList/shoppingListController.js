/// <reference path="../app.js" />

shoppingListModule.controller('shoppingListController', function ($scope, $http) {
	$scope.model = window.shoppingListModel;
	
	$scope.addItem = function () {
		$scope.model.Items.push({ isAdding: true });
		$scope.$apply();
		window.scrollTo(0, document.body.scrollHeight);
	};

	$scope.saveItem = function (item) {
		var url = $('#shoppingList').data('save-item-url');
		$http.post(url + "?item=" + escape(item.ItemName))
			.success(function () {
				item.isAdding = false;
			});
	};

	$scope.cancelSaveItem = function (item) {
		if (item.isAdding) {
			item.isRemoved = true;
		}
		item.isAdding = false;
	};

	$scope.toggle = function (item) {
		item.selected = !item.selected;
	};

	$scope.removeItems = function () {
		var url = $('#shoppingList').data('remove-item-url');
		_.each($scope.model.Items, function (item) {
			if (item.selected) {
				$http.post(url + "?item=" + escape(item.ItemName))
					.success(function () {
						item.isRemoved = true;
					});
			}
		});
	};
});

shoppingListModule.filter('filterItems', function () {
	return function (items) {
		var visibleItems = [];
		window._.each(items, function (item) {
			if (!item.isRemoved ) {
				visibleItems.push(item);
			}
		});
		return visibleItems;
	};
});




