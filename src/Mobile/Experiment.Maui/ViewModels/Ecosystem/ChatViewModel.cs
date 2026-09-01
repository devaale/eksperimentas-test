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
using Experiment.Maui.Enums;
using Experiment.Maui.Models;
using Experiment.Maui.Services;
using Experiment.Maui.Views;

namespace Experiment.Maui.ViewModels.Ecosystem{
    /// <summary>
    /// Chat with specific user viewModel, which requires UserId or PostId for its work
    /// </summary>
    public class ChatViewModel : ViewModelBase
    {
        #region Const
        const string TYPE_NAME = nameof(ChatViewModel);
        const int TIMER_DELAY = 30 * 1000;

        #endregion

        #region Attributes
        readonly ApiServices _ApiServices = new ApiServices();
        Timer _Timer;
        ObservableCollection<VisualChatMessage> _Messages = new ObservableCollection<VisualChatMessage>();
        M.Message _SelectedItem;
        bool _IsRefreshing;
        VisualUser _Receiver = null;
        string _NewMessage;

        #endregion

        #region Properties

        public string UserId { get; set; }
        public int? PostId { get; set; }

        /// <summary>
        /// UserId or PostId are this ViewModel parameters, via which it loading.
        /// One of them must be defined in order that this worked.
        /// </summary>
        public UserInfoType DataLoadType
        {
            get
            {
                if (!string.IsNullOrEmpty(UserId))
                    return UserInfoType.User;

                if (PostId.HasValue)
                    return UserInfoType.Post;

                return UserInfoType.Unknown;
            }
        }
        public string DataLoadId { get => DataLoadType == UserInfoType.User ? UserId : PostId.ToString(); }

        public ObservableCollection<VisualChatMessage> Messages
        {
            get => _Messages;
            set => SetProperty(ref _Messages, value);
        }

        public M.Message SelectedItem
        {
            get => _SelectedItem;
            set
            {
                Debug.WriteLine(string.Format("{0}::{1} = {2}", TYPE_NAME, nameof(SelectedItem), value));
                SetProperty(ref _SelectedItem, value);
            }
        }

        public M.ChatMessage LastLoadedItem
        {
            get => Messages[Messages.Count - 1];
        }

        public bool IsRefreshing
        {
            get => _IsRefreshing;
            set => SetProperty(ref _IsRefreshing, value);
        }

        public VisualUser Receiver
        {
            get => _Receiver;
            set
            {
                SetProperty(ref _Receiver, value);

                OnPropertyChanged(nameof(IsLoaded));
                OnPropertyChanged(nameof(ShowReceiverPanel));
            }
        }

        public bool IsLoaded { get => Receiver != null; }
        //public bool IsNotMe { get => true; }
        public bool ShowReceiverPanel { get => IsLoaded; }

        public string NewMessage
        {
            get => _NewMessage;
            set => SetProperty(ref _NewMessage, value);
        }

        public string LabelMessageText { get => E.T("messageText"); }
        public string LabelSend { get => E.T("send"); }
        public string LabelUserProfile { get => E.T("userProfile"); }


        #endregion

        #region Ctor
        public ChatViewModel()
        {
            _Timer = new Timer(TIMER_DELAY);
            _Timer.Elapsed += new ElapsedEventHandler(TimerHandler);
            _Timer.AutoReset = true;
        }

        #endregion

        #region Events
        protected async void TimerHandler(object sender, ElapsedEventArgs e)
        {
            var vLoc = string.Format("{0}::{1}(object sender, ElapsedEventArgs e)", TYPE_NAME, nameof(TimerHandler));
            Debug.WriteLine(vLoc);

            await LoadAsync(ListLoadMode.Newest);
        }

        #endregion

        #region Helpers

        #endregion

        #region Methods
        public async Task LoadAsync(ListLoadMode loadMode)
        {
			var vLoc = string.Format("{0}::{1}(ListLoadMode loadMode={2})",
				TYPE_NAME, nameof(LoadAsync), loadMode);
			Debug.WriteLine(string.Format("{0}, Start...", vLoc));

            try
			{
                if (IsRefreshing || DataLoadType == UserInfoType.Unknown)
                    return;

                IsRefreshing = true;
                _Timer.Enabled = false;
                //IsBusy = true;

                // Receiver processing
                await LoadReceiverData(false);

                switch (loadMode)
                {
                    case ListLoadMode.Full:
                        // In case of FULL we destroying already loaded messages as this is full reload
                        Messages.Clear();
                        break;

                    case ListLoadMode.Newest:
                        break;

                    case ListLoadMode.Older:
                        break;

                    default:
                        break;
                }

                // Those dates not always take into account SQL procedures, all depends on loadMode
                // So we initialize them in same way for every mode
                DateTime? firstDate = null, lastDate = null;
                // If we have any messages
                if (Messages.Count > 0)
                {
                    firstDate = Messages.First().Date;  // Using LINQ
                    lastDate = Messages.Last().Date;
                }

                // Retrieving messages
                var messages = await _ApiServices.ChatConversationAsync(
                    DataLoadType, DataLoadId, loadMode, firstDate, lastDate);

                switch (loadMode)
                {
					case ListLoadMode.Newest:
						// May impact performance
						var filteredMsgs = messages.Where(m => !Messages.Any(em => em.Id.Equals(m.Id))).ToList();
						for (var i = 0; i < filteredMsgs.Count; i++)
						{
							Messages.Insert(i, filteredMsgs[i]);
						}
						break;

                    // Adding to existing at the end, valid in case of full as list was cleaned and adding older
                    default:
                        foreach (var message in messages)
                        {
                            if (!Messages.Any(p => p.Id.Equals(message.Id)))
                            {
                                Messages.Add(message);
                            }
                        }
                        break;
                }

                _Timer.Enabled = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}, {1}\r\n{2}", vLoc, ex.Message, ex.StackTrace));
            }
            finally
            {
                IsRefreshing = false;
                //IsBusy = false;
            }
        }

        public async Task LoadReceiverData(bool reload)
        {
            if (Receiver == null || reload)
            {
                Receiver = await _ApiServices.UserInfoAsync(DataLoadType, DataLoadId);
            }
        }

        /// <summary>
        /// For lazy load mechanism
        /// </summary>
        /// <returns></returns>
        public async Task ItemAppearing(VisualChatMessage message)
        {
            if (LastLoadedItem.Equals(message))
            {
                await LoadAsync(ListLoadMode.Older);
            }

            if (!message.Read.HasValue && !message.IsMyMessage)
            {
                var result = await _ApiServices.MessageReadAsync(message.Id);
                if (result)
                {
                    message.Read = DateTime.Now;
                }
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
                    await LoadAsync(ListLoadMode.Full);
                });
            }
        }

        public ICommand PostMessageCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(PostMessageCommand));

                    var user = await Dictionaries.Instance.GetCurrentUser(false);
                    var msg = new M.Message()
                    {
                        Date = DateTime.Now,
                        SenderUserId = user.Id,
                        ReceiverUserId = Receiver.Id,
                        Body = NewMessage,
                    };

                    var result = await _ApiServices.MessagePostAsync(msg);
                    NewMessage = string.Empty;  // purge already sent text from textbox

                    if (result.IsSuccessStatusCode)
                    {
                        await LoadAsync(ListLoadMode.Newest);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            vLoc,
                            E.T("operationFailed"),
                            E.T("cancel"));

                    }
                });
            }
        }

        public ICommand UserProfileCommand
        {
            get
            {
                //return new Command(async () =>
                return new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new V.UserProfilePage()
                        {
                            BindingContext = new UserProfileViewModel()
                            {
                                UserId = Receiver.Id,
                            },
                        });
                });
            }
        }

        #endregion
    }
}

