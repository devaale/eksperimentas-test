using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Timer = System.Timers.Timer;

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
    public class ChatConversationsViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(ChatConversationsViewModel);
        const int TIMER_DELAY = 30 * 1000;

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        Timer _Timer;

        /// <summary>
        /// All loaded items
        /// </summary>
        ObservableCollection<VisualChatConversation> _AllItems = new ObservableCollection<VisualChatConversation>();
        /// <summary>
        /// Only filtered items, which will be bound
        /// </summary>
        ObservableCollection<VisualChatConversation> _Items = new ObservableCollection<VisualChatConversation>();
        VisualChatConversation _SelectedItem;
        bool _IsRefreshing;

        /// <summary>
        /// UI Search Textbox value
        /// </summary>
        string _SearchText = string.Empty;

        /// <summary>
        /// After search clicked active search pattern
        /// </summary>
        string _ActiveSearchText = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Only filtered items, which will be bound
        /// </summary>
        public ObservableCollection<VisualChatConversation> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }

        public VisualChatConversation SelectedItem
        {
            get => _SelectedItem;
            set
            {
                Debug.WriteLine(TYPE_NAME + "::" + nameof(SelectedItem) + " = " + value);
                var changed = !Equals(_SelectedItem, value);

                SetProperty(ref _SelectedItem, value);

                if (changed && _SelectedItem is VisualChatConversation)
                {
                    OpenChat();
                }
            }
        }

        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set => SetProperty(ref _IsRefreshing, value);
        }

        public string SearchText
        {
            get => _SearchText;
            set => SetProperty(ref _SearchText, value);
        }

        public string LabelSearchText { get => E.T("searchText"); }
        public string LabelSearch { get => E.T("search"); }
        public string LabelClear { get => E.T("clear"); }

        #endregion

        #region Ctor
        public ChatConversationsViewModel()
        {
            Title = E.T("conversations");
            _Timer = new Timer(TIMER_DELAY);
            _Timer.Elapsed += new ElapsedEventHandler(TimerHandler);
            _Timer.AutoReset = true;
        }

        ~ChatConversationsViewModel()
        {
            if (_Timer != null)
                _Timer.Enabled = false;
        }

		#endregion

		#region Events
		protected async void TimerHandler(object sender, ElapsedEventArgs e)
        {
            var vLoc = string.Format("{0}::{1}(object sender, ElapsedEventArgs e)", TYPE_NAME, nameof(TimerHandler));
            Debug.WriteLine(vLoc);

            await LoadAsync();
        }

        #endregion

        #region Helpers
        protected async Task OpenChat()
        {
            await Application.Current.MainPage.Navigation.PushAsync(
                new V.ChatPage()
                {
                    BindingContext = new ChatViewModel()
                    {
                        UserId = SelectedItem.IsMyMessage ? SelectedItem.ReceiverUserId : SelectedItem.SenderUserId,
                    }
                });
        }

        protected void PopulateData()
        {
            IsBusy = true;

            IEnumerable<VisualChatConversation> filtered;
            if (string.IsNullOrEmpty(_ActiveSearchText))
            {
                filtered = _AllItems;
            }
            else
            {
                filtered = _AllItems.Where(
                    i => i.IsMyMessage && i.Receiver.ToUpper().Contains(_ActiveSearchText.ToUpper()) ||
                    !i.IsMyMessage && i.Sender.ToUpper().Contains(_ActiveSearchText.ToUpper()));
            }

            Items.Clear();
            foreach (var item in filtered)
            {
                Items.Add(item);
            }

            IsBusy = false;
        }

        #endregion

        #region Methods
        public async Task LoadAsync()
        {
            try
            {
                var vLoc = string.Format("{0}::{1}()",
                    TYPE_NAME, nameof(LoadAsync));
                Debug.WriteLine(vLoc, "Start");

                if (IsRefreshing)
                    return;

                _Timer.Enabled = false;
                IsRefreshing = true;
                //IsBusy = true;

                // Retrieving conversations
                _AllItems = await _ApiServices.ChatConversationsAsync();

                PopulateData();
                _Timer.Enabled = true;
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

        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await LoadAsync();
                });
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new Command(async () =>
                {
                    _ActiveSearchText = SearchText;
                    PopulateData();
                });
            }
        }
        public ICommand ClearCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsBusy = true;

                    SearchText = string.Empty;

                    IsBusy = false;
                });
            }
        }

        #endregion

    }
}

