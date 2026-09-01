using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

using Experiment.Core.Base;
using Experiment.Data.Metadata;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Data.Models;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Settings{
    public class ObjectViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(ObjectViewModel);
        const bool DEBUG = true;

        #endregion

        #region Attributes
        ApiServices _ApiServices = new ApiServices();
        M.Object _Item;
        ObservableCollection<VisualObjectPermission> _FriendPermissions = new ObservableCollection<VisualObjectPermission>();
		VisualObjectPermission _SelectedFriendPermission;

		#endregion

		#region Properties
		public M.Object Item
        {
            get => _Item;
            set
            {
				SetProperty(ref _Item, value);

                if(_Item != null)
                {
                    LoadAsync(this);

                    if (_Item.Permissions == null)
                        _Item.Permissions = new List<ObjectPermission>();
                }
			}
        }

        public ObservableCollection<VisualObjectPermission> FriendPermissions
        {
            get => _FriendPermissions;
            set => SetProperty(ref _FriendPermissions, value);
        }

        public VisualObjectPermission SelectedFriendPermission
        {
            get => _SelectedFriendPermission;
            set
            {
				SetProperty(ref _SelectedFriendPermission, value);

				if(_SelectedFriendPermission != null)
                {
                    ShowFriendPermission();
                }
			}
        }

		public override string Title
        {
            get
            {
                var retVal = E.T("undefined");

                if (Item != null)
                {
                    if (Item.IsNewObject)
                    {
						retVal = E.T("newObject");
					}
					else
                    {
						retVal = Item.Name;
                    }
                }

                return retVal;
            }
        }

        public string LabelObjectInfo { get => E.T("objectInfo"); }
        public string LabelDescription { get => E.T("description"); }
        public string LabelGiveTokens { get => E.T("giveToken"); }
        public string LabelShareToFriends { get => E.T("objectShareToFriends"); }
        public string LabelEmail { get => E.T("email"); }
        public string LabelSave { get => E.T("save"); }
        public string LabelDelete { get => E.T("delete"); }
        public string LabelCancel { get => E.T("cancel"); }

        public bool HasValidId { get => Item.Id != 0; }

        public bool CanBeEdited
        {
            get
            {
                var retVal = false;
                if (Item != null && Item is M.Object)
                {
                    retVal = Item.IsOwnedObject;
                }
                return retVal;
            }
        }

        public bool CanDelete
        {
            get
            {
                var retVal = false;
                if (Item != null && Item is M.Object)
                {
                    retVal = !Item.IsNewObject && Item.IsOwnedObject;
                }
                return retVal;
            }
        }

        public bool CanDonate
        {
            get
            {
				var retVal = false;
				if (Item != null && Item is M.Object)
				{
					retVal = !Item.IsOwnedObject;
				}
				return retVal;
			}
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Constructor in case of new friend
        /// </summary>
        public ObjectViewModel()
        {
        }

        #endregion

        #region Helpers
        #endregion

        #region Methods
        public async Task LoadAsync (object sender)
        {
            var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(LoadAsync));
            Debug.WriteLineIf(DEBUG, vLoc);

            if(Item != null)
            {
                var user = await Dictionaries.Instance.GetCurrentUser(false);
                Item.IsOwnedObject = Item.IsNewObject || user.Id.Equals(Item.UserId);

                FriendPermissions.Clear();
                var friends = await _ApiServices.FriendListAsync();
                foreach(var friend in friends)
                {
                    // If specific friend already has permissions
                    var perm = Item.Permissions.Where(p => 
                        friend.RelatedUserId.Equals(p.FriendUserId)).FirstOrDefault();

                    VisualObjectPermission vperm = new VisualObjectPermission()
					{
						Name = friend.Name,
						FriendUserId = friend.RelatedUserId,
					}; 

                    if(perm != null)
                    {
                        vperm.Selected = true;  // If this object available, means selected

                        vperm.PermWrite = perm.PermWrite;
						vperm.PermDevice = perm.PermDevice;
						vperm.PermAlgorithm = perm.PermAlgorithm;
						vperm.PermGroup = perm.PermGroup;
					}

					FriendPermissions.Add(vperm);
                }

				//OnPropertyChanged(nameof(FriendPermissions));
				//Debug.WriteLineIf(DEBUG, vLoc);
			}
		}

		public async Task ShowFriendPermission()
		{
            var item = SelectedFriendPermission;    // Saving selection
			SelectedFriendPermission = null;        // Resetting selection that event was possible to trigger again afetr this

			await Application.Current.MainPage.Navigation.PushAsync(
				new V.Settings.ObjectPermissionPage()
				{
					BindingContext = new ObjectPermissionViewModel
					{
						Item = item,
					},
				});
		}

		#endregion

		#region Commands
		public ICommand SaveCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if(CanBeEdited)
                    {
                        // IEnumrableus su 
                        var perms = FriendPermissions.Where(fp => fp.Selected == true);
                        Item.Permissions = new List<ObjectPermission>(perms);

						if (Item.IsNewObject)
						{
							await _ApiServices.ObjectPostAsync(Item);
						}
						else
						{
							await _ApiServices.ObjectPutAsync(Item);
						}

						await Application.Current.MainPage.Navigation.PopAsync();
					}
				});
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var confirmationResult = await Application.Current.MainPage.DisplayAlert(
                        E.T("question"),
                        string.Format(E.T("sureDelete1"), Item.Name),
                        E.T("yes"),
                        E.T("no"));
                    if (confirmationResult)
                    {
                        await _ApiServices.ObjectDeleteAsync(Item);
                        await Application.Current.MainPage.Navigation.PopAsync();
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

        public ICommand GiveTokenCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var sender = await Dictionaries.Instance.GetCurrentUser(false);
                    if (sender.Id.Equals(Item.UserId))
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
                            ReceiverUserId = Item.UserId,
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
        #endregion
    }
}

