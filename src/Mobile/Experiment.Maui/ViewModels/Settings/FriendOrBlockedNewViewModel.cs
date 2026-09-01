using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
//using VM = Experiment.Maui.ViewModels;

using Experiment.Core.Base;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Models;
using DevExpress.Maui.Charts;

namespace Experiment.Maui.ViewModels.Settings{
    public class FriendOrBlockedNewViewModel : ViewModelBase
    {
        #region Constants
        const string TYPE_NAME = nameof(FriendOrBlockedNewViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        UserRelationType _CurrentActivityType = UserRelationType.Friend;
        bool _FirstAppearance = true;
        string _SearchText = string.Empty;
        ObservableCollection<User> _Items = new ObservableCollection<User>();
        User _SelectedItem = null;

        #endregion

        #region Properties
        public UserRelationType CurrentActivityType
        {
            get => _CurrentActivityType;
            set
            {
                SetProperty(ref _CurrentActivityType, value);
                OnPropertyChanged(nameof(Title));
            }
        }

        public ObservableCollection<User> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }

        public User SelectedItem
        {
            get => _SelectedItem;
            set
            {
                SetProperty(ref _SelectedItem, value);
                OnPropertyChanged(nameof(IsAnythingSelected));
            }
        }

        public bool IsAnythingSelected
        {
            get => _SelectedItem != null && _SelectedItem is User;
        }

        public string SearchText
        {
            get => _SearchText;
            set
            {
                SetProperty(ref _SearchText, value);
                OnPropertyChanged(nameof(IsSearchTextAvailable));
            }
        }

        public bool IsSearchTextAvailable
        {
            get => !string.IsNullOrEmpty(_SearchText);
        }

        public override string Title
        {
            get => CurrentActivityType == UserRelationType.Friend ? E.T("addFriend") : E.T("block");
        }

        public string LabelPublicName { get => E.T("publicName"); }
        public string LabelSearch { get => E.T("search"); }
        public string LabelAdd { get => E.T("add"); }
        public string LabelCancel { get => E.T("cancel"); }

        #endregion

        #region Ctor
        public FriendOrBlockedNewViewModel()
        {
        }

        #endregion

        #region Helpers
        async Task WarningNothingSelected()
        {
            await Application.Current.MainPage.DisplayAlert(
                E.T("warning"),
                E.T("nothing-selected"),
                E.T("ok"));
        }

        async Task WarningEnterSearchText()
        {
            await Application.Current.MainPage.DisplayAlert(
                E.T("warning"),
                E.T("enterSearchText"),
                E.T("ok"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called from View.
        /// 
        /// This was done to handle buttons lock, as somehow without it even with bindings they were still enabled.
        /// </summary>
        public void OnAppearing()
        {
            if (_FirstAppearance)
            {
                _FirstAppearance = false;

                // This was solved putting IsEnabled after Command in XAML
                // @see https://stackoverflow.com/a/46817216
                //OnPropertyChanged(nameof(IsAnythingSelected));
                //OnPropertyChanged(nameof(IsSearchTextAvailable));
            }
        }

        #endregion

        #region Commands
        public ICommand SearchCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (IsSearchTextAvailable)
                    {
                        SelectedItem = null;
                        Items.Clear();
                        var items = await _ApiServices.UserSearchAsync(CurrentActivityType, SearchText);
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }
                    }
                    else
                    {
                        OnPropertyChanged(nameof(IsSearchTextAvailable));
                        await WarningEnterSearchText();
                    }
                });
            }
        }
        public ICommand AddCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(AddCommand));

                    if (IsAnythingSelected)
                    {
                        var answer = await Application.Current.MainPage.DisplayAlert(
                            E.T("question"),
                            string.Format("{0}\r\n\r\n{1}", E.T("sureAdd"), SelectedItem.Name),
                            E.T("yes"),
                            E.T("no"));

                        if (answer)
                        {
                            var currentUser = await Dictionaries.Instance.GetCurrentUser(false);

                            switch (CurrentActivityType)
                            {
                                case UserRelationType.Friend:

                                    var friend = new Friend()
                                    {
                                        UserId = currentUser.Id,
                                        RelatedUserId = SelectedItem.Id,
                                    };
                                    await _ApiServices.FriendPostAsync(friend);
                                    break;

                                case UserRelationType.Blocked:

                                    var blocked = new Blocked()
                                    {
                                        UserId = currentUser.Id,
                                        RelatedUserId = SelectedItem.Id,
                                    };
                                    await _ApiServices.BlockedPostAsync(blocked);
                                    break;

                                default:
                                    Debug.WriteLine(vLoc, "Something went wrong!");
                                    break;
                            }

                            await Application.Current.MainPage.Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        OnPropertyChanged(nameof(IsSearchTextAvailable));
                        await WarningNothingSelected();
                    }
                });
            }
        }
        public ICommand CancelCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PopAsync();
                });
            }
        }

        #endregion
    }
}


