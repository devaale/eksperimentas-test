using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.Maui.ViewModels.Settings;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

namespace Experiment.Maui.Views.Settings{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ObjectsPage : ContentPage
    {
        protected ObjectsViewModel Vm
        {
            get => BindingContext is ObjectsViewModel ? BindingContext as ObjectsViewModel : null;
        }

        public ObjectsPage()
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
