using FlickrNet;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SocialPhotoFrame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public const string APIKEY = "b2731623eb48a83a1ccd4ebf762d52a5";
        public const string SHAREDSECRET = "ee64f2535327c2d6";
        private readonly Flickr flickrClient;
        private ApplicationView view;

        public MainPage()
        {
            this.InitializeComponent();
            this.view = ApplicationView.GetForCurrentView();
            this.AppSettings = new AppSettingsManager();
            this.flickrClient = new Flickr(APIKEY, SHAREDSECRET);
            if (!string.IsNullOrEmpty(this.AppSettings.FlickrAccessToken) && !string.IsNullOrEmpty(this.AppSettings.FlickrAccessTokenSecret))
            {
                this.flickrClient.OAuthAccessToken = this.AppSettings.FlickrAccessToken;
                this.flickrClient.OAuthAccessTokenSecret = this.AppSettings.FlickrAccessTokenSecret;
            }

            this.DataContext = this;
            Loaded += MainPage_Loaded;
        }

        public AppSettingsManager AppSettings { get; private set; }

        public bool IsFullscreen => this.view.IsFullScreenMode;

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.AppSettings.SearchByUserName)
            {
                var user = await flickrClient.PeopleFindByUserNameAsync(this.AppSettings.FlickrUsername);
                this.PictureGrid.ItemsSource = await flickrClient.PeopleGetPhotosAsync(user.UserId);
            }
            else if (!string.IsNullOrEmpty(this.AppSettings.FlickrTags))
                this.PictureGrid.ItemsSource = await flickrClient.PhotosSearchAsync(new PhotoSearchOptions { Tags = this.AppSettings.FlickrTags, PerPage = 10, SortOrder = PhotoSearchSortOrder.InterestingnessDescending });

            this.SizeChanged += (s, ea) =>
            {
                if (this.view.IsFullScreenMode)
                    this.BottomAppBar.Visibility = Visibility.Collapsed;
                else
                    this.BottomAppBar.Visibility = Visibility.Visible;

                //TODO: temp fix for updating size of items
                var tempItems = PictureGrid.ItemsSource;
                this.PictureGrid.ItemsSource = null;
                this.PictureGrid.ItemsSource = tempItems;
            };
        }

        private void FullscreenButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.view.IsFullScreenMode)
                this.view.ExitFullScreenMode();
            else
                this.view.TryEnterFullScreenMode();
        }

        private void SettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!SettingsPopup.IsOpen)
            {
                RootPopupBorder.Width = ActualWidth * 0.9;
                RootPopupBorder.Height = ActualHeight * 0.9;
                SettingsPopup.HorizontalOffset = Window.Current.Bounds.Width - RootPopupBorder.Width;
                SettingsPopup.IsOpen = true;
            }
        }

        private async void SaveSettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.AppSettings.SearchByUserName)
            {
                var user = await this.flickrClient.PeopleFindByUserNameAsync(this.AppSettings.FlickrUsername);
                PictureGrid.ItemsSource = await flickrClient.PeopleGetPhotosAsync(user.UserId);
            }
            else if (!string.IsNullOrEmpty(this.AppSettings.FlickrTags))
                PictureGrid.ItemsSource = await this.flickrClient.PhotosSearchAsync(new PhotoSearchOptions { Tags = this.AppSettings.FlickrTags, PerPage = 10, SortOrder = PhotoSearchSortOrder.InterestingnessDescending });

            SettingsPopup.IsOpen = false;
        }

        private async void AuthorizeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var requestToken = await this.flickrClient.OAuthRequestTokenAsync("oob");
            string url = this.flickrClient.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);
            var uri = new Uri(url);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            var dialog = new AuthorizationCodeInputDialog();
            var verifier = await dialog.ShowAsync();
            if (verifier == ContentDialogResult.Primary)
            {
                var accessToken = await this.flickrClient.OAuthAccessTokenAsync(requestToken.Token, requestToken.TokenSecret, dialog.Code);
                this.AppSettings.FlickrAccessToken = accessToken.Token;
                this.AppSettings.FlickrAccessTokenSecret = accessToken.TokenSecret;
            }
        }
    }
}
