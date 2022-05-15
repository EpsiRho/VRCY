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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            LimitedUser lu = e.ClickedItem as LimitedUser;
            ViewModel.SelectedUser = VRChatHandler.GetFullUser(lu.Id);
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
