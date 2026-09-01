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

using Experiment.Core.Base;
using Experiment.Data.Metadata;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Control{
    public class ControlViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(ControlViewModel);

		#endregion

		#region Attributes


		#endregion

		#region Properties
		VisualUser _CurrentUser;
		M.Object _CurrentObject;

		public string LabelGroups { get => E.T("groups"); }
        public string LabelAlgorithms { get => E.T("algorithms"); }

        #endregion

        #region CTOR
        public ControlViewModel()
        {
            Title = E.T("control");

            LoadaAsync(this);
        }

        #endregion

        #region Helpers
        async Task LoadaAsync(object sender)
        {
			// Pre-Load current user
			_CurrentUser = await Dictionaries.Instance.GetCurrentUser(false);
			// Pre-Load current object
			_CurrentObject = await Dictionaries.Instance.GetCurrentObject(false);
		}

		#endregion

		#region Methods

		#endregion

		#region Commands

		public ICommand GroupsListCommand
        {
            get
            {
                return new Command(async () =>
                {
					if (_CurrentObject == null)
					{
						await Application.Current.MainPage.DisplayAlert(
							E.T("err-op"),
							E.T("noObjectData"),
							E.T("cancel"));
					}
					else
					{
						// User always has access to own object
						var hasAccess = _CurrentObject.IsOwnedObject;
						if (!hasAccess)
						{
							// But if it is not own object, check permissions
							var hasPermissions = _CurrentObject.Permissions != null;
							if (hasPermissions)
							{
								var perm = _CurrentObject.Permissions.Where(p => p.FriendUserId == _CurrentUser.Id).FirstOrDefault();
								if (perm != null)
								{
									hasAccess = perm.PermGroup;
								}
							}
						}

						if (hasAccess)
						{
							// Open Page
							await Application.Current.MainPage.Navigation.PushAsync(
								new V.GroupsPage());
						}
						else
						{
							await Application.Current.MainPage.DisplayAlert(
								E.T("accessDenied"),
								E.T("accessDeniedFeature"),
								E.T("cancel"));
						}
					}
                });
            }
        }

        public ICommand AlgorithmsListCommand
        {
            get
            {
                return new Command(async () =>
                {
					if (_CurrentObject == null)
					{
						await Application.Current.MainPage.DisplayAlert(
							E.T("err-op"),
							E.T("noObjectData"),
							E.T("cancel"));
					}
					else
					{
						// User always has access to own object
						var hasAccess = _CurrentObject.IsOwnedObject;
						if (!hasAccess)
						{
							// But if it is not own object, check permissions
							var hasPermissions = _CurrentObject.Permissions != null;
							if (hasPermissions)
							{
								var perm = _CurrentObject.Permissions.Where(p => p.FriendUserId == _CurrentUser.Id).FirstOrDefault();
								if (perm != null)
								{
									hasAccess = perm.PermAlgorithm;
								}
							}
						}

						if (hasAccess)
						{
							// Open Page
							await Application.Current.MainPage.Navigation.PushAsync(
								new V.AlgorithmsPage());
						}
						else
						{
							await Application.Current.MainPage.DisplayAlert(
								E.T("accessDenied"),
								E.T("accessDeniedFeature"),
								E.T("cancel"));
						}
					}
                });
            }
        }

		#endregion
	}
}

