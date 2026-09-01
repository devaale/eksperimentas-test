using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
using Experiment.Core.Ui;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;

namespace Experiment.Maui.ViewModels.Settings{
    public class SettingsViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(SettingsViewModel);

        #endregion

        #region Attributes
        // Maybe will be needed later
        readonly ApiServices _ApiServices = new ApiServices();
        //protected ObservableCollection<M.Language> _Languages = new ObservableCollection<M.Language>();
        bool _IsDebug;
        KeyValuePair<string, string> _CurrentServer;
        //string _Language;
        PickerHandler<M.Language> _Languages;

        string _LabelLanguage;
        string _LabelServer;
        string _LabelFriendsAndBLocked;
        string _LabelObjects;
        string _LabelMyProfile;

        #endregion

        #region Properties

        public bool IsDebug
        {
            get => _IsDebug;
            set => SetProperty(ref _IsDebug, value);
        }

        public KeyValuePair<string, string> CurrentServer
        {
            get => D.Settings.CurrentServerKvp;
            set
            {
                ChangeServerAsync(value);
                //SetProperty(ref _CurrentServer, newlyChangedServer);
            }
        }

        public string Language
        {
            get => D.Settings.Language;
            set
            {
                if (value != null)
                {
                    var previousLang = D.Settings.Language;
					D.Settings.Language = value;

                    if (!previousLang.Equals(D.Settings.Language))
                    {
                        UpdateLanguage();
                    }
                }
            }
        }
        public PickerHandler<M.Language> Languages
        {
            get
            {
                if (_Languages == null)
                {
                    _Languages = new PickerHandler<M.Language>(
                        this, nameof(Language), nameof(M.Language.Code));
                }

                return _Languages;
            }
        }

        public string LabelLanguage
        {
            get => _LabelLanguage;
            set => SetProperty(ref _LabelLanguage, value);
        }
        public string LabelServer
        {
            get => _LabelServer;
            set => SetProperty(ref _LabelServer, value);
        }
        public string LabelFriendsAndBLocked
        {
            get => _LabelFriendsAndBLocked;
            protected set => SetProperty(ref _LabelFriendsAndBLocked, value);
        }
        public string LabelObjects
        {
            get => _LabelObjects;
            protected set => SetProperty(ref _LabelObjects, value);
        }

        public string LabelMyProfile
        {
            get => _LabelMyProfile;
            protected set => SetProperty(ref _LabelMyProfile, value);
        }

        #endregion

        #region Ctor

        public SettingsViewModel()
        {
#if DEBUG
            // Debug build
            IsDebug = true;
#else
			// Release build
			IsDebug = false;
#endif

            UpdateWords();
            LoadAsync();
        }

        #endregion

        #region Helpers

        async Task ChangeServerAsync(KeyValuePair<string, string> pretendingServer)
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(ChangeServerAsync));

            // If current and newly selected servers are different
            if (!string.Equals(D.Settings.Server, pretendingServer.Key))
            {
                // Making question MSG with servers names in it
                var questionMsg = string.Format(
                    E.T("confirmServerChange"),
					D.Settings.CurrentServerKvp.Value,
                    pretendingServer.Value);

                // Asking for server change confirmation
                var agreed = await Application.Current.MainPage.DisplayAlert(
                    E.T("changeServer"), questionMsg, E.T("yes"), E.T("no"));
                if (agreed)
                {
                    // Full logout
                    await D.Settings.Logout();

					// Now changing local settings to the new server
					D.Settings.Server = pretendingServer.Key;
                    _CurrentServer = pretendingServer;

                    // Close the dialogue
                    await Application.Current.MainPage.Navigation.PopAsync();
                }

                OnPropertyChanged(nameof(CurrentServer));

            }
        }

        async Task LoadAsync()
        {
            //var vLoc = TYPE_NAME + "::" + nameof(LoadAsync) + "()";
            //Debug.WriteLine(vLoc + ", GetLanguages()");
            var languages = await Dictionaries.Instance.GetLanguages(false);
            //Debug.WriteLine(vLoc + ", Assigninng language...");
            Language = D.Settings.Language;
            //Debug.WriteLine(vLoc + ", Cleaning up languages...");
            Languages.Clear();
            //Debug.WriteLine(vLoc + ", Adding real languages...");
            Languages.AddRange(languages);
        }

        async Task UpdateLanguage()
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(UpdateLanguage));

            // Get current user
            var user = await Dictionaries.Instance.GetCurrentUser(false);

            // Update its language locally
            user.Language = Language;

            // Update user's info on back-end
            await _ApiServices.UserPutAsync(user);

            // Loading words of newly selected language
            await _ApiServices.UpdateWordsAsync();

            // Updating retrieved multilanguage words
            UpdateWords();
        }

        /// <summary>
        /// Here this implemented differently, should be like this only in this viewmodel, 
        /// since language can be changed here, and we need reload of it. 
        /// 
        /// In all other places/VMs should be used bindings.
        /// </summary>
        void UpdateWords()
        {
            // Labels
            Title = E.T("settings");
            LabelServer = E.T("server");
            LabelLanguage = E.T("language");
            LabelFriendsAndBLocked = E.T("friendsAndBlocked");
            LabelObjects = E.T("objects");
            LabelMyProfile = E.T("myProfile");
        }

        #endregion

        #region Commands
        public ICommand FriendsOrFamilyCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.Settings.FriendsOrBlockedPage()
                        {
                            BindingContext = new FriendsOrBlockedViewModel(),
                        });
                });
            }
        }

        public ICommand ObjectsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.Settings.ObjectsPage()
                        {
                            BindingContext = new ObjectsViewModel()
                            {
                                IsSelectionMode = false,
                            },
                        });
                });
            }
        }

        public ICommand MyProfileCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var currentUser = await Dictionaries.Instance.GetCurrentUser(true);
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.Ecosystem.UserProfilePage()
                        {
                            BindingContext = new VM.Ecosystem.UserProfileViewModel()
                            {
                                Item = currentUser,
                            },
                        });
                });
            }
        }

        #endregion

    }
}

