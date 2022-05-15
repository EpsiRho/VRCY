using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VRChat.API.Model;
using VRCY.Classes;
using VRCY.JSON;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace VRCY.ViewModels
{
    public class FeedPageViewModel : ObservableObject
    {
        // Init
        public FeedPageViewModel()
        {
            OnlineHeaderClick = new RelayCommand(ChangeOnlineVisibility);
            ActiveHeaderClick = new RelayCommand(ChangeActiveVisibility);
            OfflineHeaderClick = new RelayCommand(ChangeOfflineVisibility);
            UserVisibilityClick = new RelayCommand(ChangeUserPopoutVisibility);
            UserPopoutVisibility = Visibility.Collapsed;
            SearchListViewVisibility = Visibility.Collapsed;
            AllUsersViewVisibility = Visibility.Visible;
        }

        // Lists
        public ObservableCollection<LimitedUser> OfflineUsers = new ObservableCollection<LimitedUser>();
        public ObservableCollection<LimitedUser> OnlineUsers = new ObservableCollection<LimitedUser>();
        public ObservableCollection<LimitedUser> ActiveUsers = new ObservableCollection<LimitedUser>();
        public ObservableCollection<LimitedUser> SearchUsers = new ObservableCollection<LimitedUser>();
        public ObservableCollection<MessageJson> Events = new ObservableCollection<MessageJson>();

        // Checks
        public bool LoopForEvents;

        // Imports
        public CoreDispatcher Dispatcher;
        
        // Commands
        public ICommand OnlineHeaderClick { get; }
        public ICommand ActiveHeaderClick { get; }
        public ICommand OfflineHeaderClick { get; }
        public ICommand UserVisibilityClick { get; }

        // Binds
        private Visibility _OnlineListViewVisibility;
        public Visibility OnlineListViewVisibility 
        {
            get => _OnlineListViewVisibility;
            set => SetProperty(ref _OnlineListViewVisibility, value);
        }

        private Visibility _ActiveListViewVisibility;
        public Visibility ActiveListViewVisibility
        {
            get => _ActiveListViewVisibility;
            set => SetProperty(ref _ActiveListViewVisibility, value);
        }

        private Visibility _OfflineListViewVisibility;
        public Visibility OfflineListViewVisibility
        {
            get => _OfflineListViewVisibility;
            set => SetProperty(ref _OfflineListViewVisibility, value);
        }

        private Visibility _UserPopoutVisibility;
        public Visibility UserPopoutVisibility
        {
            get => _UserPopoutVisibility;
            set => SetProperty(ref _UserPopoutVisibility, value);
        }
        private Visibility _SearchListViewVisibility;
        public Visibility SearchListViewVisibility
        {
            get => _SearchListViewVisibility;
            set => SetProperty(ref _SearchListViewVisibility, value);
        }
        private Visibility _AllUsersViewVisibility;
        public Visibility AllUsersViewVisibility
        {
            get => _AllUsersViewVisibility;
            set => SetProperty(ref _AllUsersViewVisibility, value);
        }

        private VRChat.API.Model.User _SelectedUser;
        public VRChat.API.Model.User SelectedUser
        {
            get => _SelectedUser;
            set=> SetProperty(ref _SelectedUser, value);
        }

        // Funcs
        private void ChangeOnlineVisibility()
        {
            if (OnlineListViewVisibility == Visibility.Collapsed)
            {
                OnlineListViewVisibility = Visibility.Visible;
            }
            else
            {
                OnlineListViewVisibility = Visibility.Collapsed;
            }
        }

        private void ChangeActiveVisibility()
        {
            if (ActiveListViewVisibility == Visibility.Collapsed)
            {
                ActiveListViewVisibility = Visibility.Visible;
            }
            else
            {
                ActiveListViewVisibility = Visibility.Collapsed;
            }
        }

        private void ChangeOfflineVisibility()
        {
            if (OfflineListViewVisibility == Visibility.Collapsed)
            {
                OfflineListViewVisibility = Visibility.Visible;
            }
            else
            {
                OfflineListViewVisibility = Visibility.Collapsed;
            }
        }
        private void ChangeUserPopoutVisibility()
        {
            if (UserPopoutVisibility == Visibility.Collapsed)
            {
                UserPopoutVisibility = Visibility.Visible;
            }
            else
            {
                UserPopoutVisibility = Visibility.Collapsed;
            }
        }

        public async void Loop()
        {
            while (LoopForEvents)
            {
                foreach (var ev in VRChatHandler.Pipeline)
                {
                    if (!Events.Contains(ev))
                    {
                        if(ev.type == "Offline")
                        {
                            var onusers = OnlineUsers.Where(o => o.DisplayName == ev.displayName).ToList();
                            var acusers = ActiveUsers.Where(o => o.DisplayName == ev.displayName).ToList();
                            foreach(var user in onusers)
                            {
                                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    OnlineUsers.Remove(user);
                                    OfflineUsers.Add(user);
                                });
                            }
                            foreach(var user in acusers)
                            {
                                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    ActiveUsers.Remove(user);
                                    OfflineUsers.Add(user);
                                });
                            }
                        }
                        else if (ev.type == "Online")
                        {
                            var offusers = OfflineUsers.Where(o => o.DisplayName == ev.displayName).ToList();
                            var acusers = ActiveUsers.Where(o => o.DisplayName == ev.displayName).ToList();
                            foreach (var user in offusers)
                            {
                                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    OfflineUsers.Remove(user);
                                    OnlineUsers.Add(user);
                                });
                            }
                            foreach (var user in acusers)
                            {
                                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    ActiveUsers.Remove(user);
                                    OnlineUsers.Add(user);
                                });
                            }
                        }
                        else if (ev.type == "Active")
                        {
                            var offusers = OfflineUsers.Where(o => o.DisplayName == ev.displayName).ToList();
                            var onusers = OnlineUsers.Where(o => o.DisplayName == ev.displayName).ToList();
                            foreach (var user in offusers)
                            {
                                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    OfflineUsers.Remove(user);
                                    ActiveUsers.Add(user);
                                });
                            }
                            foreach (var user in onusers)
                            {
                                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    OnlineUsers.Remove(user);
                                    ActiveUsers.Add(user);
                                });
                            }
                            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                            });
                        }
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            Events.Insert(0, ev);
                        });
                    }
                }
                Thread.Sleep(100);
            }
        }

        public async void OnLoad()
        {
            try
            {
                foreach (var ev in VRChatHandler.Pipeline)
                {
                    Events.Add(ev);
                }

                var friends = VRChatHandler.GetFriends(false);
                foreach (var friend in friends)
                {
                    if (friend.ProfilePicOverride == "")
                    {
                        friend.ProfilePicOverride = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                    }
                    if (friend.CurrentAvatarThumbnailImageUrl == "")
                    {
                        friend.CurrentAvatarThumbnailImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                    }
                    if (friend.Status == UserStatus.Active)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                        {
                            ActiveUsers.Add(friend);
                        });
                    }
                    else
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                        {
                            OnlineUsers.Add(friend);
                        });

                    }
                }

                var oFriends = VRChatHandler.GetFriends(true);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    OfflineUsers.Clear();
                });
                foreach (var friend in oFriends)
                {
                    if (friend.ProfilePicOverride == "")
                    {
                        friend.ProfilePicOverride = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                    }
                    if (friend.CurrentAvatarThumbnailImageUrl == "")
                    {
                        friend.CurrentAvatarThumbnailImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                    }
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        try
                        {
                            OfflineUsers.Add(friend);
                        }
                        catch(Exception e)
                        {

                        }
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
