using Experiment.Core.Base;
using Experiment.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Models{
	public class PostNew : ViewModelBase
	{
		#region Const
		const int MIN_BODY_LENGTH = 3;

		#endregion

		#region Attributes
		string _Body = string.Empty;
		int _Audience = 2;

		public IItemOwner Owner;
		#endregion

		#region Properties
		public string Body
		{
			get => _Body;
			set
			{
				SetProperty(ref _Body, value);
				OnPropertyChanged(nameof(CanBePosted));
			}
		}

		public int Audience
		{
			get => _Audience;
			set
			{
				SetProperty(ref _Audience, value);
				OnPropertyChanged(nameof(CanBePosted));
			} 
		}

		/// <summary>
		/// Validation, do post corresponds all requirements for posting.
		/// 
		/// Add various other validation routines here. Expand it if needed.
		/// </summary>
		public bool CanBePosted { get => Body.Length >= MIN_BODY_LENGTH;  }

		public ICollection<PostImageNew> Images { get; set; }

		#endregion

		#region Ctor

		public PostNew()
			: base()
		{
			Images = new List<PostImageNew>();
		}

		#endregion
	}
}
