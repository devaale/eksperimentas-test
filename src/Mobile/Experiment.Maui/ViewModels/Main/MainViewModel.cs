using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core.Base;

using Experiment.Maui.Data;

namespace Experiment.Maui.ViewModels.Main{
	public class MainViewModel : ViewModelBase
	{
		const string TYPE_NAME = nameof(MainViewModel);

		#region Methods
		public async Task LoadAsync(object sender)
		{
			try
			{
				var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(LoadAsync));
				Debug.WriteLine(vLoc);

				IsBusy = true;

				var user = await Dictionaries.Instance.GetCurrentUser(false);
				var currentObject = await Dictionaries.Instance.GetCurrentObject(false);
				var objectName = currentObject?.Name ?? "";
				Title = (user?.Name ?? "") + " / " + objectName;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
			}
			finally
			{
				IsBusy = false;
			}
		}
		#endregion
	}
}
