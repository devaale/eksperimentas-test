using System;
using System.Diagnostics;
using System.Collections;
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
using Experiment.Core.Ui;

// MVVM
using M = Experiment.Data.Models;
using V = Experiment.Maui.Views;
using VM = Experiment.Maui.ViewModels;

using Experiment.Maui.Data;
using Experiment.Maui.Services;
using Experiment.Maui.Views;
using Microsoft.Maui.Media;
using Experiment.Data.Metadata;
using Experiment.Data.Models;

namespace Experiment.Maui.ViewModels.Ecosystem{
    public class PostNewViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(PostNewViewModel);

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        string _ImageSource;
        PostNew _Item;
        PickerHandler<Audience> _Audiences;

        #endregion

        #region Properties
        public PostNew Item
        {
            get => _Item;
            set => SetProperty(ref _Item, value);
        }
        public string ImageSource
        {
            get => _ImageSource;
            set => SetProperty(ref _ImageSource, value);
        }

        public PickerHandler<Audience> Audiences
        {
            get
            {
                if (_Audiences == null)
                    _Audiences = new PickerHandler<Audience>(
                        Item, nameof(Audience), nameof(Audience.Id));

                return _Audiences;
            }
        }

        public string LabelDescription { get => E.T("description"); }
        public string LabelAddImages { get => E.T("addImage"); }
        public string LabelPublish { get => E.T("publish"); }
        public string LabelCancel { get => E.T("cancel"); }
        public string LabelAudience { get => E.T("audience"); }

        #endregion

        #region Ctor
        public PostNewViewModel()
        {
            Title = E.T("newContent");
            Item = new PostNew();
            //if(Item != null)
            //{
            //	Item.Owner = this;
            //}
        }

        #endregion


        #region Methods
        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;

                Audiences.Clear();
                Audiences.AddRange(new Audience[]
                {
                    new Audience() { Id = 2, Name = E.T("public") },
                    new Audience() { Id = 1, Name = E.T("friendsOrFamily") },
                    new Audience() { Id = 0, Name = E.T("private") },
                });

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }

        }

        #endregion

        #region Commands

        public ICommand AddPictureCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var vLoc = TYPE_NAME + "::AddPictureCommand";

                    var file = await MediaPicker.PickPhotoAsync(new MediaPickerOptions());
                    if (file != null)
                    {

                        Debug.WriteLine(string.Format("{0}: Content type: {1}, File name: {2}, Full path: {3}",
                            vLoc, file.ContentType, file.FileName, file.FullPath));

                        var pi = new PostImageNew(file.FullPath)
                        {
                            Name = file.FileName,
                            ContentType = file.ContentType,
                        };

                        Item.Images.Clear();
                        Item.Images.Add(pi);

                        ImageSource = file.FullPath;
                    }
                    else
                    {
                        Debug.WriteLine(
                            string.Format("{0}: File wasn't selected!", vLoc));
                        ImageSource = string.Empty;
                    }

                    Debug.WriteLine(
                        string.Format("{0}: {1}", vLoc, file));
                });
            }
        }


        public ICommand PublishCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var vLoc = TYPE_NAME + "::PublishCommand";

                    try
                    {
                        IsBusy = true;
                        var result = await _ApiServices.PostNewAsync(Item);
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            vLoc,
                            E.T("err-op") + Environment.NewLine + Environment.NewLine + ex.Message,
                            E.T("cancel"));
                    }
                    finally
                    {
                        IsBusy = false;
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

        #endregion
    }
}

