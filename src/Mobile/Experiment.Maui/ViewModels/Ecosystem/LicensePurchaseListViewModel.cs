using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using Microsoft.Maui.Controls;

using Experiment.Core.Base;
using Experiment.Data.Enums;

using V=Experiment.Maui.Views.Ecosystem;

using Experiment.Maui.Enums;
using Experiment.Maui.Models;
using Experiment.Maui.Services;

namespace Experiment.Maui.ViewModels.Ecosystem{
    public class LicensePurchaseListViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(LicensePurchaseListViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        VisualLicenseProduct _SelectedItem;
        LicensePurchaseViewModel _CurrentPurchase;

        #endregion

        #region Properties
        public ObservableCollection<VisualLicenseProduct> Items { get; set; }
        public VisualLicenseProduct SelectedItem
        {
            get => _SelectedItem;
            set
            {
                var changed = !Equals(_SelectedItem, value);
                SetProperty(ref _SelectedItem, value);

                if (changed)
                {
                    OpenLicenseDetails();
                }
            }
        }

        public string LabelBack { get => E.T("back"); }
        public string LabelNext { get => E.T("aboutLicense"); }
        public string LabelLicenses { get => E.T("licenses"); }
        public string LabelPrice { get => E.T("price"); }

        #endregion

        #region Ctor
        public LicensePurchaseListViewModel()
        {
            Items = new ObservableCollection<VisualLicenseProduct>();
        }

        #endregion

        #region Events

        #endregion

        #region Helpers

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
            try
            {
                var vLoc = string.Format("{0}::{1}()",
                    TYPE_NAME, nameof(LoadAsync));
                Debug.WriteLine("Start", vLoc);

                IsBusy = true;
                Items.Clear();

                var products = await _ApiServices.LicenseProductsAsync();
                foreach (var item in products)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task OpenLicenseDetails()
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(OpenLicenseDetails));
            Debug.WriteLine(vLoc);

            // The idea is to allow for user backward and forward navigation that his options were saved
            // Same LicensePurchaseViewModel manages whole purchase process, same instane of it, and it used with all purchase process dialogues for this cause
            //
            // If it is not initialized
            if (_CurrentPurchase == null)
            {
                // Just initialize it and reuse until purchase is undone
                _CurrentPurchase = new LicensePurchaseViewModel();
            }
            else
            {
                // It was initilized, but maybe this is an old already purchased one?
                if (_CurrentPurchase.Posted)
                {
                    // Initialize new one only in this case
                    _CurrentPurchase = new LicensePurchaseViewModel();
                }
            }

            // Set user's sepected license product for purchase
            // Same _CurrentPurchase will be reused in case of next; back; next; navigation;
            _CurrentPurchase.SelectedProduct = _SelectedItem;

            await Application.Current.MainPage.Navigation.PushAsync(
                new V.LicensePurchaseDetailsPage()
                {
                    BindingContext = _CurrentPurchase,
                });
        }

        #endregion

        #region Commands
        public ICommand BackCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PopAsync();
                });
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OpenLicenseDetails();
                });
            }
        }

        #endregion
    }
}

