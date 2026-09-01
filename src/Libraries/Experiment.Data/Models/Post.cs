using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Experiment.Core.Base;
using Experiment.Data.Metadata;

namespace Experiment.Data.Models{
	public class Post : ViewModelBase, IPost
	{
		#region Const

		#endregion

		#region Attributes
		int _Id;
		string _UserId;
		DateTime _Date;
		string _Body;
		string _Author;
		int _Audience;
		Guid? _ImageId;
		string _ImageUrl;
		int _Likes;
		bool _Liked;
		#endregion

		#region Properties
		public int Id
		{
			get => _Id;
			set => SetProperty(ref _Id, value);
		}
		public string UserId
		{
			get => _UserId;
			set => SetProperty(ref _UserId, value);
		}
		public DateTime Date
		{
			get => _Date;
			set => SetProperty(ref _Date, value);
		}
		public string Body
		{
			get => _Body;
			set => SetProperty(ref _Body, value);
		}
		public string Author
		{
			get => _Author;
			set => SetProperty(ref _Author, value);
		}
		public int Audience
		{
			get => _Audience;
			set => SetProperty(ref _Audience, value);
		}
		/// <summary>
		/// Generated from tblPostImage DB table
		/// </summary>
		public Guid? ImageId
		{
			get => _ImageId;
			set => SetProperty(ref _ImageId, value);
		}
		//public ICollection<IPostImage> Images { get; set; }

		//public virtual string ImageUrl
		//{
		//	get => _ImageUrl;
		//	set => SetProperty(ref _ImageUrl, value);
		//}

		/// <summary>
		/// Ammount of likes
		/// </summary>
		public virtual int Likes
		{
			get => _Likes;
			set => SetProperty(ref _Likes, value);
		}

		/// <summary>
		/// Do current user liked specific post
		/// </summary>
		public virtual bool Liked
		{
			get => _Liked;
			set => SetProperty(ref _Liked, value);
		}

		#endregion
	}
}
