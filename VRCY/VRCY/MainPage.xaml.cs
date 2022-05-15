using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using VRCY.Classes;
using VRCY.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VRCY
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                UsernameEntry.Text = localSettings.Values["username"] as string;
                PasswordEntry.Password = localSettings.Values["password"] as string;
            }
            catch (Exception ex)
            {

            }
        }

        private void MainNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItem as string;

            switch (item)
            {
                case "Feed":
                    ContentFrame.Navigate(typeof(FeedPage));
                    break;
                case "Search":
                    ContentFrame.Navigate(typeof(SearchPage));
                    break;
                case "Favorites":

                    break;
                case "Profile":

                    break;
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginProgress.Visibility = Visibility.Visible;
            new Thread(async () =>
            {
                string username = "";
                string password = "";
                string tfa = "";
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    username = UsernameEntry.Text;
                    password = PasswordEntry.Password;
                    tfa = TFAEntry.Text;
                });
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["username"] = username;
                localSettings.Values["password"] = password;


                var check = await VRChatHandler.Init(username, password, tfa);

                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    LoginProgress.Visibility = Visibility.Collapsed;
                });
                Thread t = new Thread(VRChatHandler.EventListen);
                t.Start();
                if (check)
                {
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        LoginGrid.Visibility = Visibility.Collapsed;
                        MainNav.SelectedItem = FeedItem;
                        ContentFrame.Navigate(typeof(FeedPage));
                    });
                }
                else
                {
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        LoginStatus.Text = VRChatHandler.LastError;
                    });
                }
            }).Start();
        }
    }
}
