using FlickrNet;
using Windows.Storage;

namespace SocialPhotoFrame
{
    public class AppSettingsManager : BindableBase
    {
        private const string FLICKRTAGS = @"FlickrTags";
        private const string FLICKRUSERNAME = @"FlickrUsername";
        private const string SEARCHBYUSERNAME = @"SearchByUsername";
        private const string PHOTODISPLAYINTERVAL = @"PhotoDisplayInterval";
        private const string FLICKRACCESSTOKEN = @"FlickrAccessToken";
        private const string FLICKRACCESSTOKENSECRET = @"FlickrAccessTokenSecret";

        private ApplicationDataContainer localSettings;
        private ApplicationDataContainer roamingSettings;
        private string flickrTags;
        private string flickrUsername;
        private bool searchByUsername;
        private int photoDisplayInterval;
        private string flickrAccessToken;
        private string flickrAccessTokenSecret;

        public AppSettingsManager()
        {
            this.localSettings = ApplicationData.Current.LocalSettings;
            this.roamingSettings = ApplicationData.Current.RoamingSettings;
            this.flickrTags = this.localSettings.Values[FLICKRTAGS]?.ToString();
            this.flickrUsername = this.localSettings.Values[FLICKRUSERNAME]?.ToString();
            var tempSearchByUsername = (bool?)this.localSettings.Values[SEARCHBYUSERNAME];
            this.searchByUsername = tempSearchByUsername.HasValue ? tempSearchByUsername.Value : false;
            var tempPhotoDisplayInterval = this.localSettings.Values[PHOTODISPLAYINTERVAL] as int?;
            this.photoDisplayInterval = tempPhotoDisplayInterval.HasValue ? tempPhotoDisplayInterval.Value : 10;
            this.flickrAccessToken = this.localSettings.Values[FLICKRACCESSTOKEN]?.ToString();
            this.flickrAccessTokenSecret = this.localSettings.Values[FLICKRACCESSTOKENSECRET]?.ToString();
        }

        public string FlickrTags
        {
            get
            {
                return this.flickrTags;
            }
            set
            {
                SetProperty<string>(ref this.flickrTags, value);
                this.localSettings.Values[FLICKRTAGS] = value;
            }
        }

        public string FlickrUsername
        {
            get
            {
                return this.flickrUsername;
            }
            set
            {
                SetProperty<string>(ref this.flickrUsername, value);
                this.localSettings.Values[FLICKRUSERNAME] = value;
            }
        }

        public bool SearchByUserName
        {
            get
            {
                return this.searchByUsername;
            }
            set
            {
                SetProperty<bool>(ref this.searchByUsername, value);
                this.localSettings.Values[SEARCHBYUSERNAME] = value;
            }
        }

        public int PhotoDisplayInterval
        {
            get
            {
                return this.photoDisplayInterval;
            }
            set
            {
                SetProperty<int>(ref this.photoDisplayInterval, value);
                this.localSettings.Values[PHOTODISPLAYINTERVAL] = value;
            }
        }

        public string FlickrAccessToken
        {
            get
            {
                return this.flickrAccessToken;
            }
            set
            {
                SetProperty<string>(ref this.flickrAccessToken, value);
                this.localSettings.Values[FLICKRACCESSTOKEN] = value;
            }
        }

        public string FlickrAccessTokenSecret
        {
            get
            {
                return this.flickrAccessTokenSecret;
            }
            set
            {
                SetProperty<string>(ref this.flickrAccessTokenSecret, value);
                this.localSettings.Values[FLICKRACCESSTOKENSECRET] = value;
            }
        }
    }
}
