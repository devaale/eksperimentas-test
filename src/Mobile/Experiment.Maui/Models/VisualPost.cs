using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Experiment.Maui.Models{
	public class VisualPost : Experiment.Data.Models.Post
	{
		public string Reaction
		{
			get => Likes.ToString();
			//{ return String.Format("{0} {1}", Likes, E.T("likes")); }
		}

		public Color ReactionColor
		{
			get
			{
				return Liked ? Colors.SlateBlue : Colors.Black;
			}
		}

		public override bool Liked
		{
			get => base.Liked;
			set
			{
				base.Liked = value;
				OnPropertyChanged(nameof(ReactionColor));
			}
		}

		public override int Likes
		{
			get => base.Likes; 
			set
			{
				base.Likes = value;
				OnPropertyChanged(nameof(Reaction));
			}
		}

		public virtual UriImageSource ImageUrl { get; set; }

		// Might be used if in XAML to bind like this:
		// Command="{Binding LikeCommand}" CommandParameter="{Binding .}"
		//
		// Then it will go not to Page.BindingContext or ViewModel but to instance of this class
		// But problem is that according MVVM architecture this class is part of Model, while commants are part of ViewModel
		//
		//public ICommand LikeCommand
		//{
		//	get
		//	{
		//		//return new Command(async () =>
		//		return new Command(async (e) =>
		//		{
		//			if(e is Post)
		//			{
		//				var post = e as Post;
		//				// shows the same.
		//				// I so struggled how to pass post item as parameter, but before decided to move this command to Post class itself.
		//				// As it seems that specific Listview Item template is bound via BindingContext to specific class instance, what simplify everything.
		//
		//				Debug.WriteLine(post.Body + " AND " + this.Body);	
		//			}
		//		});
		//	}
		//}

	}
}

