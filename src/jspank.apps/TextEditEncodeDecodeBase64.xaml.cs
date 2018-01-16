using jspank.core.Services;
using System.Windows;

namespace jspank.apps
{
    /// <summary>
    /// Interaction logic for TextEditEncodeDecodeBase64.xaml
    /// </summary>
    public partial class TextEditEncodeDecodeBase64 : Window
    {
        public TextEditEncodeDecodeBase64()
        {
            InitializeComponent();

            var value = Clipboard.GetText();
            this.btn_from_clipboard.IsEnabled = !string.IsNullOrWhiteSpace(value);
        }

        private void btn_encode_Click(object sender, RoutedEventArgs e)
        {
            var value = Base64Service.Encode(this.txt_text.Text);
            Clipboard.SetText(value);
            this.Close();
        }

        private void btn_decode_Click(object sender, RoutedEventArgs e)
        {
            var value = Base64Service.Decode(this.txt_text.Text);
            Clipboard.SetText(value);
            this.Close();
        }

        private void btn_from_clipboard_Click(object sender, RoutedEventArgs e)
        {
            var value = Clipboard.GetText();
            this.txt_text.Text = value;
        }
    }
}
