using System;
using CommunityToolkit.Maui.Views;
using Experiment.Data.Models;
using Experiment.Maui.Models;
using Experiment.Maui.ViewModels.Devices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;

namespace Experiment.Maui.Views.Devices
{
	/// <summary>
	/// Searchable datapoint list for one formula chain row (replaces native Picker dialog).
	/// </summary>
	public class RelatedDatapointPickerPopup : Popup
	{
		readonly VisualDatapointFormulaChain _chain;

		public RelatedDatapointPickerPopup(VisualDatapointFormulaChain chain, DatapointViewModel viewModel)
		{
			_chain = chain;
			BindingContext = viewModel;
			CanBeDismissedByTappingOutsideOfPopup = true;

			var mainBg = GetResource<Color>("Background1", Colors.White);
			var entryBg = GetResource<Color>("Background4", Color.FromArgb("#F0F0F0"));
			var fg = GetResource<Color>("Foreground1", Colors.Black);
			var ph = GetResource<Color>("Foreground3", Colors.Gray);

			var searchEntry = new Entry
			{
				Placeholder = "Search",
				ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
				HorizontalOptions = LayoutOptions.Fill,
				Margin = new Thickness(0),
				BackgroundColor = entryBg,
				TextColor = fg,
				PlaceholderColor = ph,
			};
			searchEntry.SetBinding(Entry.TextProperty, new Binding(
				nameof(DatapointViewModel.RelatedDatapointPickerFilterText),
				BindingMode.TwoWay));

			var list = new CollectionView
			{
				SelectionMode = SelectionMode.None,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = mainBg,
			};
			list.SetBinding(ItemsView.ItemsSourceProperty, new Binding(nameof(DatapointViewModel.RelatedDatapoints)));
			list.ItemTemplate = new DataTemplate(() =>
			{
				var row = new Button
				{
					BackgroundColor = Colors.Transparent,
					TextColor = fg,
					HorizontalOptions = LayoutOptions.Fill,
					Padding = new Thickness(12, 10),
					BorderWidth = 0,
					BorderColor = Colors.Transparent,
				};
				row.SetBinding(Button.TextProperty, nameof(Datapoint.Name));
				row.Clicked += OnRowClicked;
				return row;
			});

			var cancel = new Button
			{
				BorderWidth = 0,
				BorderColor = Colors.Transparent,
				BackgroundColor = mainBg,
				TextColor = fg,
			};
			cancel.SetBinding(Button.TextProperty, new Binding(nameof(DatapointViewModel.LabelCancel)));
			cancel.Clicked += async (_, _) => await CloseAsync();

			var density = DeviceDisplay.MainDisplayInfo.Density;
			var displayW = DeviceDisplay.MainDisplayInfo.Width / density;
			var displayH = DeviceDisplay.MainDisplayInfo.Height / density;
			Size = new Size(displayW * 0.92, displayH * 0.72);
			var listHeight = Math.Min(520, displayH * 0.55);
			list.HeightRequest = listHeight;

			Content = new VerticalStackLayout
			{
				Padding = new Thickness(16),
				Spacing = 12,
				BackgroundColor = mainBg,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				Children =
				{
					searchEntry,
					list,
					cancel,
				},
			};
		}

		async void OnRowClicked(object sender, EventArgs e)
		{
			if (sender is not BindableObject b || b.BindingContext is not Datapoint dp)
				return;

			_chain.RelatedDatapoint = dp;
			await CloseAsync();
		}

		static T GetResource<T>(string key, T fallback)
		{
			if (Application.Current?.Resources.TryGetValue(key, out var v) == true && v is T t)
				return t;
			return fallback;
		}
	}
}
