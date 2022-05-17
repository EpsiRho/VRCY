using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using VRChat.API.Model;
using VRCY.Classes;
using VRCY.JSON;
using VRCY.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VRCY
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeedPage : Page
    {
        FeedPageViewModel ViewModel = new FeedPageViewModel();
        public FeedPage()
        {
            var color = (Color)XamlBindingHelper.ConvertValue(typeof(Color), "Purple");
            ViewModel.HeartBrush = new SolidColorBrush(color);
            this.InitializeComponent();
            ViewModel.Dispatcher = this.Dispatcher;
            Thread t = new Thread(ViewModel.OnLoad);
            t.Start();
            ViewModel.LoopForEvents = true;
            Thread l = new Thread(ViewModel.Loop);
            l.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.LoopForEvents = false;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            LimitedUser lu = e.ClickedItem as LimitedUser;
            try
            {
                var usr = VRChatHandler.GetFullUser(lu.Id);
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    ViewModel.SelectedUser = usr;
                });
                if (ViewModel.SelectedUser.WorldId != "private")
                {
                    var wrld = await VRChatHandler.GetWorldInfo(ViewModel.SelectedUser.WorldId);
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        ViewModel.SelectedWorld = wrld;
                        ViewModel.WorldName = ViewModel.SelectedWorld.Name;
                        ViewModel.WorldUrl = ViewModel.SelectedWorld.ThumbnailImageUrl;
                    });
                }
                else
                {
                    if (ViewModel.SelectedWorld != null)
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                        {
                            ViewModel.WorldName = "Private";
                            ViewModel.WorldUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if(ViewModel.SelectedWorld != null)
                {
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        ViewModel.WorldName = "Private";
                        ViewModel.WorldUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                    });
                }
            }
            ViewModel.UserVisibilityClick.Execute(null);

        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(SearchTextBox.Text == "")
            {
                ViewModel.SearchListViewVisibility = Visibility.Collapsed;
                ViewModel.AllUsersViewVisibility = Visibility.Visible;
            }
            else
            {
                List<LimitedUser> users = new List<LimitedUser>();
                users.AddRange(ViewModel.OnlineUsers.Where(u => u.DisplayName.ToLower().Contains(SearchTextBox.Text.ToLower())));
                users.AddRange(ViewModel.ActiveUsers.Where(u => u.DisplayName.ToLower().Contains(SearchTextBox.Text.ToLower())));
                users.AddRange(ViewModel.OfflineUsers.Where(u => u.DisplayName.ToLower().Contains(SearchTextBox.Text.ToLower())));

                ViewModel.SearchUsers.Clear();
                foreach (var user in users)
                {
                    ViewModel.SearchUsers.Add(user);
                }
                ViewModel.SearchListViewVisibility = Visibility.Visible;
                ViewModel.AllUsersViewVisibility = Visibility.Collapsed;
            }
        }
    }
}
