using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using System.Threading.Tasks;
using VRCY.JSON;
using System.Threading;
using System.Net.WebSockets;
using Websocket.Client;
using Newtonsoft.Json;
using Windows.Storage;

namespace VRCY.Classes
{
    internal class VRChatHandler
    {
        public static List<MessageJson> Pipeline;
        public static CurrentUser CUser { get; set; }
        public static AuthenticationApi AuthApi { get; set; }
        public static UsersApi UserApi { get; set; }
        public static FriendsApi FriendApi { get; set; }
        public static AvatarsApi AvatarApi { get; set; }
        public static WorldsApi WorldApi { get; set; }
        private static string AuthKey { get; set; }
        public static string LastError { get; set; }
        public static async Task<bool> Init(string user, string pass, string tfa = "")
        {
            // Authentication credentials
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
           
            var Config = new Configuration();
            Config.Username = user;
            Config.Password = pass;
            Config.AddApiKey("auth", localSettings.Values["authcookie"] as string);
            Config.AddApiKey("twoFactorAuth", localSettings.Values["tfaauthcookie"] as string);
            AuthApi = new AuthenticationApi(Config);
            UserApi = new UsersApi(Config);
            FriendApi = new FriendsApi(Config);
            WorldApi = new WorldsApi(Config);
            AvatarApi = new AvatarsApi(Config);
            try
            {
                var result = AuthApi.GetCurrentUserWithHttpInfo();
                AuthKey = result.Cookies.First().Value;
                CUser = result.Content as CurrentUser;
                if (CUser == null)
                {

                    var res = AuthApi.Verify2FAWithHttpInfo(new TwoFactorAuthCode(tfa));
                    if (!(res.Content as Verify2FAResult).Verified)
                    {
                        LastError = "2FA Invalid or on timeout";
                        return false;
                    }
                    localSettings.Values["tfaauthcookie"] = res.Cookies[1].Value;
                }
                localSettings.Values["authcookie"] = result.Cookies[0].Value;

                // Try and get the current user, if not check if 2fa is needed
                CUser = await AuthApi.GetCurrentUserAsync();
                if (CUser == null)
                {
                    LastError = "Login Info Invalid";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public static async Task<string> SearchUsers(string search)
        {
            var user = UserApi.GetUser(search);


            return user.DisplayName;
        }
        public static async Task<List<LimitedUser>> SearchUsersFull(string search)
        {
            return UserApi.SearchUsers(search);
        }

        public static async Task<List<LimitedWorld>> SearchWorlds(string search)
        {
            return WorldApi.SearchWorlds(search:search);
        }


        public static List<LimitedUser> GetFriends(bool offline)
        {
            if (!offline)
            {
                return FriendApi.GetFriends(0, 50, false);
            }
            else
            {
                List<LimitedUser> users = new List<LimitedUser>();
                int count = 0;
                while (true)
                {
                    var temp = FriendApi.GetFriends(count, 50, true);
                    users.AddRange(temp);
                    if(temp.Count() < 50)
                    {
                        break;
                    }
                    count+=50;
                }
                return users;
            }

        }

        public static VRChat.API.Model.User GetFullUser(string userid)
        {
            return UserApi.GetUser(userid);
        }

        public static async void EventListen()
        {
            try
            {
                var exitEvent = new ManualResetEvent(false);
                var url = new Uri($"wss://pipeline.vrchat.cloud/?authToken={AuthKey}");

                Console.WriteLine("Making Client");

                var factory = new Func<ClientWebSocket>(() =>
                {
                    var client = new ClientWebSocket
                    {
                        Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(5),
                        // Proxy = ...
                        // ClientCertificates = ...
                    }
                    };
                    client.Options.SetRequestHeader("User-Agent", "VRCY/v1.0 (By EpsilonRho)");
                    return client;
                });

                Pipeline = new List<MessageJson>();
                using (var client = new WebsocketClient(url, factory))
                {
                    client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                    client.DisconnectionHappened.Subscribe(info =>
                        Console.WriteLine($"[-] Disconnection happened, type: {info.Type}"));
                    client.ReconnectionHappened.Subscribe(info =>
                        Console.WriteLine($"[+] Reconnection happened, type: {info.Type}"));

                    client.MessageReceived.Subscribe(async msg => {

                        try
                        {
                            MessageJson mJson = JsonConvert.DeserializeObject<MessageJson>(msg.Text);
                            Console.WriteLine($"[~] Message received: {mJson.type}");

                            ContentJson cJson = JsonConvert.DeserializeObject<ContentJson>(mJson.content);
                            var username = await SearchUsers(cJson.userId);
                            mJson.displayName = username;
                            mJson.contentJson = cJson;
                            mJson.time = DateTime.Now;
                            switch (mJson.type)
                            {
                                case "friend-offline":
                                    mJson.type = "Offline";
                                    break;
                                case "friend-active":
                                    mJson.type = "Active";
                                    break;
                                case "friend-online":
                                    mJson.type = "Online";
                                    break;
                                case "friend-update":
                                    mJson.type = "Update";
                                    // TODO: get details about update
                                    break;
                                case "friend-location":
                                    mJson.type = "Location";
                                    if (cJson.location == "private")
                                    {
                                        mJson.detail = "private";
                                    }
                                    else
                                    {
                                        //string[] split = cJson.location.Split(":");
                                        mJson.detail = cJson.location;
                                        //DiscordBot.SendMessage(ctx.Channel, "", $"{cJson.user.displayName} moved to https://vrchat.com/home/launch?worldId={split[0]}&instanceId={split[1]}", DiscordColor.Blue);
                                    }
                                    break;
                            }
                            if (Pipeline.Count > 0)
                            {
                                if (Pipeline.Last().content != mJson.content)
                                {
                                    Pipeline.Add(mJson);
                                }
                            }
                            else
                            {
                                Pipeline.Add(mJson);
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"[!] Deserialization err");
                        }
                    });
                    client.Start();

                    exitEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
        }
    }
}
