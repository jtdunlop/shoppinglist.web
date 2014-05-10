/// <reference path="../../Scripts/angular.js" />

'use strict';

var shoppingListModule = angular.module('shoppingListModule', []);

shoppingListModule.directive('myRepeatDirective', function() {
	return function(scope, element, attrs) {
		if (scope.$last){
			scope.$emit('LastElem');
		}
	};
});

shoppingListModule.directive('myMainDirective', function () {
	return function (scope, element, attrs) {
		scope.$on('LastElem', function (event) {
			var height = window.innerHeight;
			var buttonHeight = $('#buttons').innerHeight();
		});
	};
});

shoppingListModule.directive('myCheckbox', ['$timeout', function (timeout) {
	return {
		restrict: 'E',
		scope: { item: '=' },
		template: '<div ng-class="{true:\'selection-div-selected\', false:\'\'}[item.selected]"><input id="id{{item.Id}}" type="checkbox" class="item-checkbox" ng-model="item.selected">' +
		'<label for="id{{item.Id}}" class="checkbox-label checkbox-label-device" ng-click="toggle(\'{{item}}\')">{{item.ItemName}}</label></div>',
		replace: true
	};
}])