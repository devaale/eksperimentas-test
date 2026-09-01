using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Newtonsoft.Json;

using Experiment.Data.Models;

namespace Experiment.Maui.Models{
	public class VisualObjectPermission : ObjectPermission
	{
		bool _Selected;
		string _Name;

		[JsonIgnore]
		public virtual string Name { get => _Name; set => SetProperty(ref _Name, value); }

		[JsonIgnore]
		public virtual bool Selected
		{
			get => _Selected;
			set
			{
				SetProperty(ref _Selected, value);

				// If not selected then any permissions can't be selected too, as object itself then it is not shared
				if(!value)
				{
					PermWrite = value;
					PermDevice = value;
					PermAlgorithm = value;
					PermGroup = value; 
				}
			}
		}

		public override bool PermWrite
		{
			get => base.PermWrite;
			set
			{
				base.PermWrite = value;

				// If Anything seleted, then selected is also enabled
				if(value)
					Selected = value;

				OnPropertyChanged(nameof(Detail));
			}
		}

		public override bool PermDevice
		{
			get => base.PermDevice;
			set
			{
				base.PermDevice = value;

				// If Anything seleted, then selected is also enabled
				if (value)
					Selected = value;

				OnPropertyChanged(nameof(Detail));
			}
		}

		public override bool PermAlgorithm
		{
			get => base.PermAlgorithm;
			set
			{
				base.PermAlgorithm = value;

				// If Anything seleted, then selected is also enabled
				if (value)
					Selected = value;

				OnPropertyChanged(nameof(Detail));
			}
		}
		public override bool PermGroup
		{
			get => base.PermGroup;
			set
			{
				base.PermGroup = value;

				// If Anything seleted, then selected is also enabled
				if (value)
					Selected = value;

				OnPropertyChanged(nameof(Detail));
			}
		}

		[JsonIgnore]
		public virtual string Detail
		{
			get
			{
				var retVal = string.Empty;
				
				if(Selected)
				{
					retVal = E.T("selected");
				}

				if(PermWrite)
				{
					if (!string.IsNullOrEmpty(retVal))
						retVal += ", ";
					retVal += E.T("permWrite");
				}

				if (PermDevice)
				{
					if (!string.IsNullOrEmpty(retVal))
						retVal += ", ";
					retVal += E.T("devices");
				}

				if (PermAlgorithm)
				{
					if (!string.IsNullOrEmpty(retVal))
						retVal += ", ";
					retVal += E.T("algorithms");
				}

				if (PermGroup)
				{
					if (!string.IsNullOrEmpty(retVal))
						retVal += ", ";
					retVal += E.T("groups");
				}
				return retVal;
			}
		}
	}
}
