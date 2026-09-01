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
using Experiment.Data.Models;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Models;

namespace Experiment.Maui.ViewModels.Settings{
	public class ObjectPermissionViewModel : ViewModelBase
	{
		VisualObjectPermission _Item;
		public VisualObjectPermission Item
		{
			get => _Item;
			set 
			{
				SetProperty(ref _Item, value);

				if(_Item != null)
				{
					Title = _Item.Name;
				}
			}
		}

		public string LabelSelected { get => E.T("selected"); }
		public string LabelPermWrite { get => E.T("permWrite"); }
		public string LabelDevices { get => E.T("devices"); }
		public string LabelAlgorithms { get => E.T("algorithms"); }
		public string LabelGroups { get => E.T("groups"); }

	}
}

