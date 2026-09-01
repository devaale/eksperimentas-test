using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels.Ecosystem;

using Experiment.Data.Enums;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Models;

namespace Experiment.Maui.Views.Ecosystem{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatPage : ContentPage
	{
		protected VM.ChatViewModel Vm { get => BindingContext as VM.ChatViewModel; }
		public ChatPage()
		{
			InitializeComponent();
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (Vm != null)
			{
				await Vm.LoadAsync(ListLoadMode.Full);
			}
		}

		private async void LvwItems_ItemAppearing(object sender, ItemVisibilityEventArgs e)
		{
			if (e.Item is VisualChatMessage)
			{
				await Vm.ItemAppearing(e.Item as VisualChatMessage);
			}
		}
	}
}
