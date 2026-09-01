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
using V = Experiment.Maui.Views.Control;
using VM = Experiment.Maui.ViewModels.Control;
using D = Experiment.Maui.Data;

using Experiment.Core.Base;

using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using DevExpress.Utils;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Control{
    public class AlgorithmsViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(AlgorithmsViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        ObservableCollection<VisualAlgorithm> _Items = new ObservableCollection<VisualAlgorithm>();

        bool _IsSelectionMode = false;

        int ObjectId = 0;

        #endregion

        #region Properties
        public string LabelAdd { get => E.T("new"); }
        public string LabelEdit { get => E.T("edit"); }

        public ObservableCollection<VisualAlgorithm> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }

        public VisualAlgorithm SelectedItem
        {
            get => Items.FirstOrDefault(i => i.Id.Equals(D.Settings.Algorithm));
            set
            {
                if (value == null)
                {
                    D.Settings.Algorithm = 0;
                    D.Settings.AlgorithmName = string.Empty;
                }
                else
                {
                    D.Settings.Algorithm = value.Id;
                    D.Settings.AlgorithmName = value.Name;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAlgorithmSelected));
                OnPropertyChanged(nameof(SelectedItemDesc));
            }
        }

        public string SelectedItemDesc
        {
            get => E.T("algorithmSelected") + " " + (string.IsNullOrEmpty(D.Settings.AlgorithmName) ? E.T("none") : D.Settings.AlgorithmName);
        }

        public bool IsSelectionMode
        {
            get => _IsSelectionMode;
            set => SetProperty(ref _IsSelectionMode, value);
        }

        public bool IsAlgorithmSelected
        {
            get
            {
                return SelectedItem != null;
            }
        }

        #endregion

        #region CTOR
        public AlgorithmsViewModel()
        {
            ObjectId = D.Settings.ObjectId;

            Title = E.T("algorithms");
        }

        #endregion

        #region Helpers
        async Task ShowSelectedItem(VisualAlgorithm item)
        {

            if (SelectedItem is VisualAlgorithm)
            {
                item.CanBeEdited = item.ObjectId.Equals(ObjectId);
                item.CanDelete = item.ObjectId.Equals(ObjectId);

                await Application.Current.MainPage.Navigation.PushAsync(
                    new V.AlgorithmPage()
                    {
                        BindingContext = new AlgorithmViewModel()
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
            var vLoc = TYPE_NAME + "::LoadAsync()";
            try
            {
                IsBusy = true;

                Items.Clear();

                var alg = await _ApiServices.AlgortihmsListAsync(D.Settings.ObjectId.ToString());
                foreach (var i in alg)
                {
                    Items.Add(i);
                }
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
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.AlgorithmPage()
                        {
                            BindingContext = new AlgorithmViewModel()
                            {
                                Item = new VisualAlgorithm()
                                {
                                    ObjectId = D.Settings.ObjectId,

                                    CanBeEdited = true,

                                    // Set default values
                                    Type = AlgorithmType.TimeTrigger,
                                    Name = "Test",

                                    DateStart = DateTime.Today,
                                    DateEnd = DateTime.Today,
                                    TimeStart = DateTime.Now.TimeOfDay,
                                    TimeEnd = DateTime.Now.TimeOfDay,

                                    ValueFrom = 0,
                                    ValueTo = 1,

                                    AlarmId = 0,
                                    GroupId = 0,
                                    DatapointId = 0,

                                    ValueOff = 0,
                                    ValueOn = 1,
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

        #endregion
    }
}

