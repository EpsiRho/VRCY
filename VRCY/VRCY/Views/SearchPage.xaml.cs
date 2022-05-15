using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VRChat.API.Model;
using VRCY.Classes;
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

namespace VRCY.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        ObservableCollection<LimitedUser> UsersList = new ObservableCollection<LimitedUser>();
        ObservableCollection<LimitedWorld> WorldsList = new ObservableCollection<LimitedWorld>();
        ObservableCollection<Avatar> AvatarsList = new ObservableCollection<Avatar>();
        public SearchPage()
        {
            this.InitializeComponent();
        }

        private async void UsersSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                var list = await VRChatHandler.SearchUsersFull(UsersSearch.Text);
                UsersList.Clear();
                foreach(var user in list)
                {
                    if (user.ProfilePicOverride == "")
                    {
                        user.ProfilePicOverride = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                    }
                    if (user.CurrentAvatarThumbnailImageUrl == "")
                    {
                        user.CurrentAvatarThumbnailImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Transparent_square.svg/1024px-Transparent_square.svg.png";
                    }
                    UsersList.Add(user);
                }
            }
        }

        private async void WorldsSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var list = await VRChatHandler.SearchWorlds(WorldsSearch.Text);
                WorldsList.Clear();
                foreach (var world in list)
                {
                    WorldsList.Add(world);
                }
            }
        }

    }
}
