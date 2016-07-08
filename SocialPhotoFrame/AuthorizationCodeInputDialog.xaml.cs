using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SocialPhotoFrame
{
    public sealed partial class AuthorizationCodeInputDialog : ContentDialog
    {
        public string Code { get; private set; }
        public AuthorizationCodeInputDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Code = this.AuthorizationCodeTextBox.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Code = string.Empty;
        }
    }
}
