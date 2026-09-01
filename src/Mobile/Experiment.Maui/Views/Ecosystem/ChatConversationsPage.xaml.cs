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

using Experiment.Maui.Data;
using Experiment.Maui.Services;

namespace Experiment.Maui.Views.Ecosystem{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatConversationsPage : ContentPage
	{
		protected VM.ChatConversationsViewModel Vm { get => BindingContext as VM.ChatConversationsViewModel; }
		public ChatConversationsPage()
		{
			InitializeComponent();
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (Vm != null)
			{
				await Vm.LoadAsync();
			}
		}
	}
}
