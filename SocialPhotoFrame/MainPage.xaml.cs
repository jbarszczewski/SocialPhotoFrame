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
            this.Loaded += MainPage_Loaded;
        }

        public AppSettingsManager AppSettings { get; private set; }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateControls();
            await LoadPhotosAsync();
            this.SizeChanged += (s, ea) => UpdateControls();
        }

        private void UpdateControls()
        {
            if (this.view.IsFullScreenMode)
            {
                this.BottomAppBar.Visibility = Visibility.Collapsed;
                this.FullScreenSymbolIcon.Symbol = Symbol.BackToWindow;
            }
            else
            {
                this.BottomAppBar.Visibility = Visibility.Visible;
                this.FullScreenSymbolIcon.Symbol = Symbol.FullScreen;
            }

            //TODO: temp fix for updating size of items
            if(this.PictureGrid.ItemsSource == null)
                return;
            var tempItems = PictureGrid.ItemsSource;
            this.PictureGrid.ItemsSource = null;
            this.PictureGrid.ItemsSource = tempItems;
        }

        private async Task LoadPhotosAsync()
        {
            try
            {
                if (this.AppSettings.SearchByUserName)
                {
                    var user = await flickrClient.PeopleFindByUserNameAsync(this.AppSettings.FlickrUsername);
                    this.PictureGrid.ItemsSource = await flickrClient.PeopleGetPhotosAsync(user.UserId);
                }
                else if (!string.IsNullOrEmpty(this.AppSettings.FlickrTags))
                    this.PictureGrid.ItemsSource = await flickrClient.PhotosSearchAsync(new PhotoSearchOptions { Tags = this.AppSettings.FlickrTags, PerPage = 10, SortOrder = PhotoSearchSortOrder.InterestingnessDescending });
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
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
                SettingsPopup.VerticalAlignment = VerticalAlignment.Stretch;
                SettingsPopup.HorizontalOffset = Window.Current.Bounds.Width - RootPopupBorder.Width;
                SettingsPopup.IsOpen = true;
            }
        }

        private async void SaveSettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await LoadPhotosAsync();
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
