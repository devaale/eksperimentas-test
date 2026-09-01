using System;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace Experiment.Maui.Views.Main
{
	public class DashboardGeneralSettingsPopup : Popup
	{
		public DashboardGeneralSettingsPopup()
		{
			CanBeDismissedByTappingOutsideOfPopup = true;

			var mainBg = (Color)(Application.Current?.Resources["Background1"] ?? Colors.Transparent);

			var lblObject = new Label { FontAttributes = FontAttributes.Bold };
			lblObject.SetBinding(Label.TextProperty, "LabelObject");

			var cmbObject = new Picker { HorizontalOptions = LayoutOptions.Fill };
			cmbObject.SetBinding(Picker.TitleProperty, "LabelObject");
			cmbObject.SetBinding(Picker.ItemsSourceProperty, "Objects");
			cmbObject.ItemDisplayBinding = new Binding("Name");
			cmbObject.SetBinding(Picker.SelectedItemProperty, "SelectedObject", BindingMode.TwoWay);
			cmbObject.SetBinding(Picker.IsEnabledProperty, "CanControl");

			var lblDateRange = new Label { FontAttributes = FontAttributes.Bold };
			lblDateRange.SetBinding(Label.TextProperty, "LabelDateRange");

			var cmbDateRange = new Picker { HorizontalOptions = LayoutOptions.Fill };
			cmbDateRange.SetBinding(Picker.TitleProperty, "LabelDateRange");
			cmbDateRange.SetBinding(Picker.ItemsSourceProperty, "DateRanges.List");
			cmbDateRange.ItemDisplayBinding = new Binding("Name");
			cmbDateRange.SetBinding(Picker.SelectedItemProperty, "DateRanges.Selected", BindingMode.TwoWay);
			cmbDateRange.SetBinding(Picker.IsEnabledProperty, "CanControl");

			var btnClose = new Button();
			btnClose.SetBinding(Button.TextProperty, "LabelClose");
			btnClose.Clicked += OnCloseClicked;

			var displayWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
			var popupWidth = displayWidth * 0.9;

			Content = new VerticalStackLayout
			{
				Padding = new Thickness(24),
				Spacing = 16,
				BackgroundColor = mainBg,
				MinimumWidthRequest = popupWidth,
				HorizontalOptions = LayoutOptions.Fill,
				Children = { lblObject, cmbObject, lblDateRange, cmbDateRange, btnClose }
			};
		}

		async void OnCloseClicked(object sender, EventArgs e)
		{
			await CloseAsync();
		}
	}
}
