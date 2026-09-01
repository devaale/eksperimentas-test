using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

using Experiment.Core.Base;
using Experiment.Data.Models;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views.Ecosystem;
using VM = Experiment.Maui.ViewModels.Ecosystem;

using SH = Experiment.Data.Services.SuperHow;

using Experiment.Maui.Services;
using Experiment.Maui.Enums;
using Experiment.Maui.Data;
using Experiment.Maui.Models;
using Experiment.Maui.Views;

namespace Experiment.Maui.ViewModels.Ecosystem{
    public class UserProfileViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(UserProfileViewModel);
        readonly Style NegativeButtonStyle = (Style)Application.Current.Resources["negativeButton"];

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        VisualUser _Item;
        ObservableCollection<VisualLicense> _VisualLicenses = new ObservableCollection<VisualLicense>();
        //string _Description;
        #endregion

        #region Properties

        /// <summary>
        /// Should be set UserId or PostId to load user data
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Should be set UserId or PostId to load user data
        /// </summary>
        public int? PostId { get; set; }

        public VisualUser Item
        {
            get => _Item;
            set
            {
                var changed = !Equals(_Item, value);
                SetProperty(ref _Item, value);

                OnPropertyChanged(nameof(IsLoaded));
                OnPropertyChanged(nameof(IsNotMe));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(LabelAddFriend));
                OnPropertyChanged(nameof(LabelBlock));
                OnPropertyChanged(nameof(CanGiveToken));
                OnPropertyChanged(nameof(HasAddress));
                OnPropertyChanged(nameof(FriendshipButtonStyle));

                if (changed)
                {
                    VisualLicenses.Clear();
                    if (_Item != null)
                    {
                        foreach (var lic in _Item.Licenses)
                        {
                            VisualLicenses.Add(VisualLicense.FromLicense(lic));
                        }
                    }
                }
            }
        }

        public bool IsLoaded
        {
            get => Item != null;
        }
        public bool IsNotMe
        {
            get
            {
                if (Item != null)
                {
                    return !Item.IsMe;
                }
                return true;
            }
        }
        public bool CanGiveToken
        {
            get => IsLoaded && IsNotMe;
        }
        public string Description
        {
            get
            {
                if (Item != null)
                {
                    if (Item.IsMe)
                    {
                        return E.T("yourProfile");
                    }
                }
                return string.Empty;
            }
        }

        public bool HasAddress
        {
            get
            {
                if (Item != null)
                {
                    return !string.IsNullOrEmpty(Item.Address);

                }
                return false;
            }
        }
        public ObservableCollection<VisualLicense> VisualLicenses
        {
            get => _VisualLicenses;
            set => SetProperty(ref _VisualLicenses, value);
        }

        public string LabelContact { get => E.T("contact"); }
        public string LabelAddFriend
        {
            get
            {
                if (IsLoaded)
                {
                    if (Item.IsFriend)
                    {
                        return E.T("unfriend");
                    }
                }
                return E.T("addFriend");
            }
        }
        public string LabelBlock
        {
            get
            {
                if (IsLoaded)
                {
                    if (Item.IsBlocked)
                    {
                        return E.T("unblock");
                    }
                }
                return E.T("block"); ;
            }
        }
        public string LabelGiveTokens { get => E.T("giveToken"); }
        public string LabelLicenses { get => E.T("licenses"); }
        public string LabelValidFrom { get => E.T("validFrom"); }
        public string LabelValidUntil { get => E.T("validUntil"); }
        public string LabelAboutLicenses { get => E.T("aboutLicenses"); }
        public string LabelBlockchainAccount { get => E.T("blockchainAccount"); }

        public Style FriendshipButtonStyle
        {
            get
            {
                if(IsLoaded)
                {
                    if(Item.IsFriend)
                    {
                        return NegativeButtonStyle;
					}
                }
                return null;
            }
        }

		#endregion

		#region Ctor

		public UserProfileViewModel()
        {
            Title = E.T("userProfile");
        }

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
            if (!string.IsNullOrEmpty(UserId))
            {
                Item = await _ApiServices.UserInfoAsync(
                    UserInfoType.User, UserId);
            }
            else if (PostId.HasValue)
            {
                Item = await _ApiServices.UserInfoAsync(
                    UserInfoType.Post, PostId.ToString());
            }

        }

		#endregion

		#region Commands
		public ICommand LicensesCommand
		{
			get
			{
				return new Command(async () =>
				{
					await Application.Current.MainPage.Navigation.PushAsync(
						new V.UserLicensePage()
						{
							BindingContext = this
						});

				});
			}
		}
		
        public ICommand AddFriendCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        if (IsLoaded)
                        {
                            var isFriend = Item.IsFriend;
                            var result = await Application.Current.MainPage.DisplayAlert(
                                E.T("question"),
                                isFriend ? E.T("unfriendConfirm") : E.T("addFriendConfirm"),
                                E.T("yes"), E.T("no"));

                            if (result)
                            {
                                IsBusy = true;

                                var success = false;
                                var currentUser = await Dictionaries.Instance.GetCurrentUser(false);

                                if (isFriend)
                                {
                                    var friend = new Friend()
                                    {
                                        UserId = currentUser.Id,
                                        Id = Item.FriendId.Value,
                                    };
                                    var response = await _ApiServices.FriendDeleteAsync(friend);
                                    success = response.IsSuccessStatusCode;
                                    IsBusy = false;
                                }
                                else
                                {
                                    var friend = new Friend()
                                    {
                                        UserId = currentUser.Id,
                                        RelatedUserId = Item.Id,
                                    };
                                    var response = await _ApiServices.FriendPostAsync(friend);
                                    success = response.IsSuccessStatusCode;
                                    IsBusy = false;

                                }

                                IsBusy = true;
                                await LoadAsync();
                                IsBusy = false;

                                if (success)
                                {
                                    await Application.Current.MainPage.DisplayAlert(
                                        E.T("attention"),
                                        isFriend ? E.T("unfriendDone") : E.T("addFriendDone"),
                                        E.T("ok"));
                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert(
                                        E.T("attention"),
                                        E.T("operationFailed"),
                                        E.T("cancel"));
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            E.T("attention"),
                            ex.Message,
                            E.T("cancel"));
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                });
            }
        }

        public ICommand BlockCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        if (IsLoaded)
                        {
                            var isBlocked = Item.IsBlocked;
                            var result = await Application.Current.MainPage.DisplayAlert(
                                E.T("question"),
                                isBlocked ? E.T("unblockConfirm") : E.T("blockConfirm"),
                                E.T("yes"), E.T("no"));
                            if (result)
                            {
                                IsBusy = true;

                                var success = false;
                                var currentUser = await Dictionaries.Instance.GetCurrentUser(false);

                                if (isBlocked)
                                {
                                    var blocked = new Blocked()
                                    {
                                        UserId = currentUser.Id,
                                        Id = Item.BlockedId.Value,
                                    };
                                    var response = await _ApiServices.BlockedDeleteAsync(blocked);
                                    success = response.IsSuccessStatusCode;
                                    IsBusy = false;
                                }
                                else
                                {
                                    var blocked = new Blocked()
                                    {
                                        UserId = currentUser.Id,
                                        RelatedUserId = Item.Id,
                                    };
                                    var response = await _ApiServices.BlockedPostAsync(blocked);
                                    success = response.IsSuccessStatusCode;
                                    IsBusy = false;
                                }

                                IsBusy = true;
                                await LoadAsync();
                                IsBusy = false;

                                if (success)
                                {
                                    await Application.Current.MainPage.DisplayAlert(
                                        E.T("warning"),
                                        isBlocked ? E.T("unblockDone") : E.T("blockDone"),
                                        E.T("ok"));
                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert(
                                        E.T("attention"),
                                        E.T("operationFailed"),
                                        E.T("cancel"));
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            E.T("attention"),
                            ex.Message,
                            E.T("cancel"));
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                });
            }
        }

        public ICommand ContactCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.ChatPage()
                        {
                            BindingContext = new ChatViewModel()
                            {
                                UserId = Item.Id,
                            }
                        });

                });
            }
        }

        public ICommand GiveTokenCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var sender = await Dictionaries.Instance.GetCurrentUser(false);
                    if (sender.Id.Equals(Item.Id))
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            E.T("warning"),
                            E.T("sameSenderReceiver"),
                            E.T("cancel"));
                    }
                    else
                    {
                        var tokenTransaction = new TokenTransaction()
                        {
                            SenderUserId = sender.Id,
                            ReceiverUserId = Item.Id,
                            Tokens = 1,
                        };

                        var result = await _ApiServices.TokenTransactionPostAsync(tokenTransaction);
                        if (result.IsSuccessStatusCode)
                        {
                            await Application.Current.MainPage.DisplayAlert(
                                E.T("warning"),
                                E.T("operationSuccess"),
                                E.T("ok"));
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert(
                                E.T("warning"),
                                E.T("operationFailed"),
                                E.T("cancel"));
                        }
                    }
                });
            }
        }

		public ICommand BlockchainAccountCommand
		{
			get
			{
				return new Command(async () =>
				{
                    if(HasAddress)
                    {
                        // Creating user blockchaina ccount string url
                        var url = string.Format(SH.Defaults.BLOCKCHAIN_ACCOUNT_PATTERN, Item.Address);
						// @see https://stackoverflow.com/a/7581824
						Uri uri;
						if (Uri.TryCreate(url, UriKind.Absolute, out uri) &&
							(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
						{
							await Launcher.OpenAsync(new Uri(url));
						}
					}
				});
			}
		}



		public ICommand AboutLicensesCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.LicensePurchaseListPage()
                        {
                        });

                });
            }
        }

        #endregion
    }
}

