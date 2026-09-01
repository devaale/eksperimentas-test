using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using Experiment.Core;
using Experiment.Core.Base;
using Experiment.Data.Enums;
using Experiment.Core.Ui;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views.Ecosystem;
using VM = Experiment.Maui.ViewModels.Ecosystem;

using Experiment.Maui.Data;
using Experiment.Maui.Models;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Experiment.Maui.Enums;

namespace Experiment.Maui.ViewModels.Ecosystem{
    public class PostsViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(PostsViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        ObservableCollection<VisualPost> _Posts = new ObservableCollection<VisualPost>();
        VisualPost _SelectedItem;
        bool _IsRefreshing;

        PickerHandler<M.NamedDbItem<int>> _PostFeedPrefs;
        int _PostFeedPref;

        public string LabelFeedPrefs { get => E.T("feedPrefs"); }

        #endregion

        #region Properties


        public int PostFeedPref
        {
            get => _PostFeedPref;
            set
            {
                var changed = !Equals(_PostFeedPref, value);
                SetProperty(ref _PostFeedPref, value);

                // If value really changed
                if (changed)
                {
                    LoadAsync(false);
                }
            }
        }

        public PickerHandler<M.NamedDbItem<int>> PostFeedPrefs
        {
            get
            {
                if (_PostFeedPrefs == null)
                {
                    _PostFeedPrefs = new PickerHandler<M.NamedDbItem<int>>(
                        this, nameof(PostFeedPref), nameof(M.NamedDbItem<int>.Id));

                    _PostFeedPrefs.AddRange(new M.NamedDbItem<int>[]
                    {
                        new M.NamedDbItem<int>()
                        {
                            Id = (int)PostFeedType.Chronological,
                            Name = E.T("chronological"),
                        },
                        new M.NamedDbItem<int>()
                        {
                            Id = (int)PostFeedType.Popularity,
                            Name = E.T("popularity"),
                        }
                    });
                }

                return _PostFeedPrefs;
            }
        }

        public ObservableCollection<VisualPost> Posts
        {
            get => _Posts;
            set => SetProperty(ref _Posts, value);
        }
        public VisualPost SelectedItem
        {
            get => _SelectedItem;
            set
            {
                Debug.WriteLine(TYPE_NAME + "::" + nameof(SelectedItem) + " = " + value);
                SetProperty(ref _SelectedItem, value);
            }
        }

        public VisualPost LastLoadedItem
        {
            get => Posts[Posts.Count - 1];
        }

        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set => SetProperty(ref _IsRefreshing, value);
        }

        public string LabelNewPost { get => E.T("new_M"); }
        public string LabelContact { get => E.T("contact"); }
        public string LabelChat { get => E.T("chat"); }

        #endregion

        #region Ctor
        public PostsViewModel()
        {
        }

        #endregion

        #region Methods
        public async Task LoadAsync(bool more)
        {
            try
            {
                var vLoc = string.Format("{0}::{1}(bool more={2})", TYPE_NAME, nameof(LoadAsync), more);
                Debug.WriteLine(vLoc, "Start");

                if (IsRefreshing)
                    return;

                IsRefreshing = true;
                //IsBusy = true;

                // Retrieving user name
                var user = await Dictionaries.Instance.GetCurrentUser(false);
                Title = user.Name;

                DateTime? sinceDate = null;
                if (more)
                {
                    sinceDate = LastLoadedItem.Date;
                }
                else
                {
                    Posts.Clear();
                }

                // Retrieving posts
                var posts = await _ApiServices.PostListAsync(
                    (PostFeedType)PostFeedPref, sinceDate);

                foreach (var post in posts)
                {
                    if (!Posts.Any(p => p.Id.Equals(post.Id)))
                    {
                        Posts.Add(post);
                    }

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
            finally
            {
                IsRefreshing = false;
                //IsBusy = false;
            }

        }

        /// <summary>
        /// For lazy load mechanism
        /// </summary>
        /// <returns></returns>
        public async Task ItemAppearing(M.Post post)
        {
            if (LastLoadedItem.Equals(post))
            {
                await LoadAsync(true);
            }
        }

        #endregion

        #region Commands
        public ICommand NewPostCommand
        {
            get
            {
                return new Command(async () =>
                {
                    // Open PostNewPage
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.PostNewPage());
                });
            }
        }

        public ICommand ChatCommand
        {
            get
            {
                return new Command(async () =>
                {
                    // Open conversations page
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.ChatConversationsPage());
                });
            }
        }


        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await LoadAsync(false);
                });
            }
        }

        public ICommand LikeCommand
        {
            get
            {
                //return new Command(async () =>
                return new Command(async (e) =>
                {
                    if (e is VisualPost)
                    {
                        var post = e as VisualPost;
                        //Debug.WriteLine(post.Body);
                        var result = await _ApiServices.PostReactionAsync(new M.PostReaction()
                        {
                            PostId = post.Id,
                            Reaction = PostReactionType.Like,
                        });

                        if (result.IsSuccessStatusCode)
                        {
                            if (post.Liked)
                            {
                                post.Liked = false;
                                post.Likes--;
                            }
                            else
                            {
                                post.Liked = true;
                                post.Likes++;
                            }
                        }
                    }
                });
            }
        }

        public ICommand TapImageCommand
        {
            get
            {
                //return new Command(async () =>
                return new Command(async (e) =>
                {
                    if (e is VisualPost)
                    {
                        var post = e as VisualPost;

                        if (post.ImageId.HasValue)
                        {
                            //Debug.WriteLine(post.Body);
                            await Application.Current.MainPage.Navigation.PushAsync(
                                new V.ImagePage()
                                {
                                    BindingContext = new ImageViewModel()
                                    {
                                        ImageSource = Utils.CreateImageUrl(post.ImageId.Value, ImageType.Normal),
                                    }
                                });
                        }
                    }
                });
            }
        }

        public ICommand UserProfileCommand
        {
            get
            {
                //return new Command(async () =>
                return new Command(async (e) =>
                {
                    if (e is VisualPost)
                    {
                        var post = e as VisualPost;
                        await Application.Current.MainPage.Navigation.PushAsync(
                            new V.UserProfilePage()
                            {
                                BindingContext = new UserProfileViewModel()
                                {
                                    PostId = post.Id,
                                },
                            });
                    }
                });
            }
        }

        public ICommand ContactCommand
        {
            get
            {
                //return new Command(async () =>
                return new Command(async (e) =>
                {
                    if (e is VisualPost)
                    {
                        var post = e as VisualPost;
                        await Application.Current.MainPage.Navigation.PushAsync(
                            new V.ChatPage()
                            {
                                BindingContext = new ChatViewModel()
                                {
                                    PostId = post.Id,
                                },
                            });
                    }
                });
            }
        }

        #endregion
    }
}

