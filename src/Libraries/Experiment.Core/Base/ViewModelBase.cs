using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using Newtonsoft.Json;

namespace Experiment.Core.Base{
    /// <summary>
    /// Class developed based on example:
    /// https://stackoverflow.com/a/36151255
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
		#region Attributes
		protected bool _IsBusy;
        protected string _Title;

        #endregion

        #region Properties
        /// <summary>
        /// IsBusy state for ActivityIndicator
        /// </summary>
        [JsonIgnore]
        public virtual bool IsBusy
		{
            get => _IsBusy;
            set => SetProperty(ref _IsBusy, value);
        }
        /// <summary>
        /// Title for binding of the title
        /// </summary>
        [JsonIgnore]
        public virtual string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }

        #endregion

        #region Property Changed mechanics

        /// <summary>
        /// Property changed event host
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Need to call it on property change
        /// 
        /// the [CallerMemberName] attribute is not required, but it will allow you to write: OnPropertyChanged();
        /// instead of OnPropertyChanged("SomeProperty");,
        /// so you will avoid string constant in your code.
        /// 
        /// @see: https://stackoverflow.com/a/36151255
        /// </summary>
        /// <param name="propertyName"></param>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Used for behavior when property is set like this:
        /// 
        /// public string FirstName
        /// {
        ///     get { return _firstName; }
        ///     set { SetProperty(ref _firstName, value); }
        /// }
        /// 
        /// @see: https://stackoverflow.com/a/36151255
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
