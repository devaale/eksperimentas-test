using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Maui.Controls;

using Experiment.Core;
using Experiment.Core.Base;

namespace Experiment.Maui.ViewModels.Ecosystem{
    public class ImageViewModel : ViewModelBase
    {
        #region Attributes
        UriImageSource _ImageSource;

        #endregion

        #region Properties

        public UriImageSource ImageSource
        {
            get => _ImageSource;
            set => SetProperty(ref _ImageSource, value);
        }

        #endregion

        #region Ctor

        public ImageViewModel()
        {

        }

        public ImageViewModel(string imageSource)
            : this()
        {
            if (string.IsNullOrEmpty(imageSource))
                throw new ArgumentException(nameof(imageSource));

            ImageSource = new UriImageSource()
            {
                CachingEnabled = Defaults.IMAGE_CACHING,
                Uri = new Uri(imageSource),
            };
        }

        public ImageViewModel(UriImageSource imageSource)
            : this()
        {
            if (imageSource == null)
                throw new ArgumentException(nameof(imageSource));

            ImageSource = imageSource;
        }

        #endregion

    }
}

