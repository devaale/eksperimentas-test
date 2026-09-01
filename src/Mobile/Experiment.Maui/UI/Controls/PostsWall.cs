using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Maui.UI.Base;

// MVVM
using M = Experiment.Maui.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;


namespace Experiment.Maui.UI.Controls{
	public class PostsWall : ScrollView
	{
		#region Const
		const string TYPE_NAME = nameof(PostsWall);

		#endregion

		#region Nested types
		public class Post
		{
			/// <summary>
			/// Specific post or item
			/// </summary>
			internal object Item { get; set; }

			/// <summary>
			/// UI representation, container of the post
			/// </summary>
			internal StackLayout Container { get; set; }
			/// <summary>
			/// UI representation, description element of the post
			/// </summary>
			internal Label Description { get; set; }
			/// <summary>
			/// UI representation, image element of the post
			/// </summary>
			internal Image Picture { get; set; }
		}

		#endregion

		#region Attributes
		protected bool _Initialized = false;
		protected StackLayout _MainPanel;
		protected List<PostsWall.Post> PostWallPosts = new List<PostsWall.Post>();

		#endregion

		#region Properties
		protected bool IsMinimumSet
		{
			// Let's say we allow for this control to work without ImageUrlMember, AuthorMember, DateMember
			get => Posts != null && !string.IsNullOrEmpty(PostDescriptionMember);
		}

		#region BindableProperties
		/// <summary>
		/// Bindable PostsProperty
		/// 
		/// Property defines posts IEnumerable array
		/// </summary>
		public static readonly BindableProperty PostsProperty =
			BindableProperty.Create(nameof(Posts), typeof(IEnumerable), typeof(PostsWall), null, propertyChanged: OnPostsChanged);
		/// <summary>
		/// Posts
		/// </summary>
		public IEnumerable Posts
		{
			get => (IEnumerable)GetValue(PostsProperty);
			set => SetValue(PostsProperty, value);
		}
		protected static void OnPostsChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (PostsWall)bindable;
			me.OnPostsChanged((IEnumerable)oldValue, (IEnumerable)newValue);
		}
		protected virtual void OnPostsChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			var vLoc = TYPE_NAME + "::OnPostsChanged";
			Debug.WriteLine(vLoc);

			if(oldValue != newValue)
			{
				if(newValue is INotifyCollectionChanged)
				{
					var ncc = newValue as INotifyCollectionChanged;
					ncc.CollectionChanged -= PostsCollectionChanged;
					ncc.CollectionChanged += PostsCollectionChanged;
				}

				BuildPosts(true);
			}
		}

		protected virtual void PostsCollectionChanged(object o, EventArgs e)
		{
			BuildPosts(false);
		}


		/// <summary>
		/// Bindable PostDescriptionMemberProperty
		/// 
		/// Property defines which property member of Posts array items should be shown as description
		/// </summary>
		public static readonly BindableProperty PostDescriptionMemberProperty =
			BindableProperty.Create(nameof(PostDescriptionMember), typeof(string), typeof(PostsWall), null, propertyChanged: OnPostDescriptionMemberChanged);
		/// <summary>
		/// Posts
		/// </summary>
		public string PostDescriptionMember
		{
			get => (string)GetValue(PostDescriptionMemberProperty);
			set => SetValue(PostDescriptionMemberProperty, value);
		}
		protected static void OnPostDescriptionMemberChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (PostsWall)bindable;
			me.OnPostDescriptionMemberChanged((string)oldValue, (string)newValue);
		}
		protected virtual void OnPostDescriptionMemberChanged(string oldValue, string newValue)
		{
			var vLoc = TYPE_NAME + "::OnPostDescriptionMemberChanged";
			Debug.WriteLine(vLoc);

			if(string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(newValue))
			{
				BuildPosts(false);
			}

		}

		/// <summary>
		/// Bindable PostImageUrlMemberProperty
		/// 
		/// Property defines which property member of Posts array items should be shown as description
		/// </summary>
		public static readonly BindableProperty PostImageUrlMemberProperty =
			BindableProperty.Create(nameof(PostImageUrlMember), typeof(string), typeof(PostsWall), null, propertyChanged: OnPostImageUrlMemberChanged);
		/// <summary>
		/// Posts
		/// </summary>
		public string PostImageUrlMember
		{
			get => (string)GetValue(PostImageUrlMemberProperty);
			set => SetValue(PostImageUrlMemberProperty, value);
		}
		protected static void OnPostImageUrlMemberChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (PostsWall)bindable;
			me.OnPostImageUrlMemberChanged((string)oldValue, (string)newValue);
		}
		protected virtual void OnPostImageUrlMemberChanged(string oldValue, string newValue)
		{
			var vLoc = TYPE_NAME + "::OnPostImageUrlMemberChanged";
			Debug.WriteLine(vLoc);

		}

		/// <summary>
		/// Bindable PostAuthorMemberProperty
		/// 
		/// Property defines which property member of Posts array items should be shown as description
		/// </summary>
		public static readonly BindableProperty PostAuthorMemberProperty =
			BindableProperty.Create(nameof(PostAuthorMember), typeof(string), typeof(PostsWall), null, propertyChanged: OnPostAuthorMemberChanged);
		/// <summary>
		/// Posts
		/// </summary>
		public string PostAuthorMember
		{
			get => (string)GetValue(PostAuthorMemberProperty);
			set => SetValue(PostAuthorMemberProperty, value);
		}
		protected static void OnPostAuthorMemberChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (PostsWall)bindable;
			me.OnPostAuthorMemberChanged((string)oldValue, (string)newValue);
		}
		protected virtual void OnPostAuthorMemberChanged(string oldValue, string newValue)
		{
			var vLoc = TYPE_NAME + "::OnPostAuthorMemberChanged";
			Debug.WriteLine(vLoc);

		}

		/// <summary>
		/// Bindable PostDateMemberProperty
		/// 
		/// Property defines which property member of Posts array items should be shown as description
		/// </summary>
		public static readonly BindableProperty PostDateMemberProperty =
			BindableProperty.Create(nameof(PostDateMember), typeof(string), typeof(PostsWall), null, propertyChanged: OnPostDateMemberChanged);
		/// <summary>
		/// Posts
		/// </summary>
		public string PostDateMember
		{
			get => (string)GetValue(PostDateMemberProperty);
			set => SetValue(PostDateMemberProperty, value);
		}
		protected static void OnPostDateMemberChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var me = (PostsWall)bindable;
			me.OnPostDateMemberChanged((string)oldValue, (string)newValue);
		}
		protected virtual void OnPostDateMemberChanged(string oldValue, string newValue)
		{
			var vLoc = TYPE_NAME + "::OnPostDateMemberChanged";
			Debug.WriteLine(vLoc);

		}

		#endregion

		#endregion

		#region Ctor
		public PostsWall()
			: base()
		{
			_MainPanel = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
			};
			Content = _MainPanel;
		}

		#endregion

		#region Helpers

		protected void BuildPosts(bool purgePrevious)
		{
			if(purgePrevious)
			{
				PostWallPosts.Clear();
				_MainPanel.Children.Clear();
			}

			if (IsMinimumSet)
			{
				foreach (var item in Posts)
				{
					PostsWall.Post post = PostWallPosts.Find(p => p.Item == item);
					if(post == null)
					{
						post = new PostsWall.Post()
						{
							Item = item,
							Container = new StackLayout() { Orientation = StackOrientation.Vertical },
							Description = new Label(),
						};

						// Description binding
						post.Description.SetBinding(Label.TextProperty, PostDescriptionMember);
						post.Description.BindingContext = post.Item;
						// Adding to UI container
						post.Container.Children.Add(post.Description);

						PostWallPosts.Add(post);
						_MainPanel.Children.Add(post.Container);
					}

					// If post image URL binding is set and for this specific post not yet intialized
					if(!string.IsNullOrEmpty(PostImageUrlMember) && post.Picture == null)
					{
						post.Picture = new Image();
						post.Picture.SetBinding(Image.SourceProperty, PostImageUrlMember);
						post.Picture.BindingContext = post.Item;
						post.Container.Children.Add(post.Picture);
					}


					// Adding image if available
					if (post.Picture == null)
					{

						post.Container.Children.Add(post.Picture);
					}
						



				}
			}

		}

		#endregion

		#region Methods

		#endregion
	}
}

