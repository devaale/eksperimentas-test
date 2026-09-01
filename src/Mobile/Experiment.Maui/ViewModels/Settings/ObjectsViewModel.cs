using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;
using D = Experiment.Maui.Data;

using Experiment.Core.Base;
using Experiment.Data.Metadata;

using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Data;

namespace Experiment.Maui.ViewModels.Settings{
    public class ObjectsViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(ObjectsViewModel);


        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        ObservableCollection<M.Object> _Items = new ObservableCollection<M.Object>();
        M.Object _SelectedItem;
        bool _IsSelectionMode = false;

        #endregion

        #region Properties
        public string LabelAdd { get => E.T("new"); }
        public string LabelEdit { get => E.T("edit"); }
        public string LabelContinue { get => E.T("continue"); }

        public ObservableCollection<M.Object> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }

        public M.Object SelectedItem
        {
            get => Items.FirstOrDefault(i => i.Id.Equals(D.Settings.ObjectId));
            set
            {
                if (value == null)
                {
                    D.Settings.ObjectId = 0;
					Dictionaries.Instance.CurrentObject = null;
				}
				else
                {
                    D.Settings.ObjectId = value.Id;
					Dictionaries.Instance.CurrentObject = value;
				}
				OnPropertyChanged();
                OnPropertyChanged(nameof(IsObjectSelected));
                OnPropertyChanged(nameof(SelectedItemDesc));
            }
        }

        public string SelectedItemDesc
        {
            get
            {
				var currentObject = E.T("none");
				if (SelectedItem != null)
				{
					currentObject = SelectedItem.Name;
				}
				return string.Format("{0} {1}", E.T("objectSelected"), currentObject);
			}
        }

        public bool IsSelectionMode
        {
            get => _IsSelectionMode;
            set => SetProperty(ref _IsSelectionMode, value);
        }

        public bool IsObjectSelected
        {
            get
            {
                return SelectedItem != null;
            }
        }

        #endregion

        #region CTOR
        public ObjectsViewModel()
        {
            Title = E.T("objects");
        }

        #endregion

        #region Helpers
        async Task ShowSelectedItem(M.Object item)
        {
            if (SelectedItem is IObject)
            {
                await Application.Current.MainPage.Navigation.PushAsync(
                    new V.Settings.ObjectPage()
                    {
                        BindingContext = new ObjectViewModel()
                        {
                            Item = item,
                        },
                    });
            }
        }

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
            var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LoadAsync));
            try
            {
                IsBusy = true;
                Items = await D.Dictionaries.Instance.GetObjects(true);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    vLoc,
                    E.T("err-list-load") + Environment.NewLine + Environment.NewLine + ex.Message,
                    E.T("ok"));
            }
            finally
            {
                IsBusy = false;
                SelectedItem = SelectedItem;
            }
        }

        #endregion

        #region Commands

        public ICommand NewRecordCommand
        {
            get
            {
                return new Command(async () =>
                {

                    //var item = await _ApiServices.ObjectNewAsync();
                    var user = await Dictionaries.Instance.GetCurrentUser(false);

                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.Settings.ObjectPage()
                        {
                            BindingContext = new ObjectViewModel()
                            {
                                Item = new M.Object()
                                {
                                    UserId = user.Id,
                                    IsOwnedObject= true,
                                },
                            },
                        });
                });
            }
        }

        public ICommand EditRecordCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await ShowSelectedItem(SelectedItem);
                });
            }
        }

        public ICommand ContinueCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (SelectedItem == null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            E.T("warning"),
                            E.T("objectSelect"),
                            E.T("ok"));
                    }
                    else
                    {
                        await D.Workflow.Startup();
                    }
                });
            }
        }

        #endregion
    }
}

