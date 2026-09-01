using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Experiment.Maui.ViewModels.Control;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

namespace Experiment.Maui.Views.Control{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlgorithmsPage : ContentPage
    {
        protected AlgorithmsViewModel Vm
        {
            get => (BindingContext is AlgorithmsViewModel ? BindingContext as AlgorithmsViewModel : null);
        }

        public AlgorithmsPage()
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
