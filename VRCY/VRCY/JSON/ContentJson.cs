using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCY.JSON
{

    public class MessageJson
    {
        public string displayName { get; set; }
        public string detail { get; set; }
        public DateTime time { get; set; }
        public string type { get; set; }
        public string content { get; set; }
        public ContentJson contentJson { get; set; }
    }

    public class ContentJson
    {
        public string userId { get; set; }
        public User user { get; set; }
        public string location { get; set; }
        public string travelingToLocation { get; set; }
        public World world { get; set; }
        public bool canRequestInvite { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public string displayName { get; set; }
        public string userIcon { get; set; }
        public string bio { get; set; }
        public string[] bioLinks { get; set; }
        public string profilePicOverride { get; set; }
        public string statusDescription { get; set; }
        public string currentAvatarImageUrl { get; set; }
        public string currentAvatarThumbnailImageUrl { get; set; }
        public string state { get; set; }
        public string[] tags { get; set; }
        public string developerType { get; set; }
        public DateTime last_login { get; set; }
        public string last_platform { get; set; }
        public bool allowAvatarCopying { get; set; }
        public string status { get; set; }
        public string date_joined { get; set; }
        public bool isFriend { get; set; }
        public string friendKey { get; set; }
        public DateTime last_activity { get; set; }
    }

    public class World
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool featured { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public int capacity { get; set; }
        public string[] tags { get; set; }
        public string releaseStatus { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string assetUrl { get; set; }
        public Asseturlobject assetUrlObject { get; set; }
        public string pluginUrl { get; set; }
        public Pluginurlobject pluginUrlObject { get; set; }
        public string unityPackageUrl { get; set; }
        public Unitypackageurlobject unityPackageUrlObject { get; set; }
        public string _namespace { get; set; }
        public Unitypackage[] unityPackages { get; set; }
        public int version { get; set; }
        public string organization { get; set; }
        public object previewYoutubeId { get; set; }
        public int favorites { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime publicationDate { get; set; }
        public DateTime labsPublicationDate { get; set; }
        public int visits { get; set; }
        public int popularity { get; set; }
        public int heat { get; set; }
    }

    public class Asseturlobject
    {
    }

    public class Pluginurlobject
    {
    }

    public class Unitypackageurlobject
    {
    }

    public class Unitypackage
    {
        public string id { get; set; }
        public string assetUrl { get; set; }
        public Asseturlobject1 assetUrlObject { get; set; }
        public string pluginUrl { get; set; }
        public Pluginurlobject1 pluginUrlObject { get; set; }
        public string unityVersion { get; set; }
        public long unitySortNumber { get; set; }
        public int assetVersion { get; set; }
        public string platform { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Asseturlobject1
    {
    }

    public class Pluginurlobject1
    {
    }


}
