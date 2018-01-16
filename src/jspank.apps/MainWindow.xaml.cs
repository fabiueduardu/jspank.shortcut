using System;
using System.Windows;

namespace jspank.apps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

     
            var args = Environment.GetCommandLineArgs();

            if (args != null && args.Length >= 1)
            {
                switch (args[1])
                {
                    case "texteditencodedecodebase64":
                        this.ShowWindow<TextEditEncodeDecodeBase64>();
                        break;
                }
            }
          
        }

        private void btn_base64_Click(object sender, RoutedEventArgs e)
        {
            this.ShowWindow<TextEditEncodeDecodeBase64>();
        }

        private void ShowWindow<T>() where T : Window, new()
        {
            var win = new T();
            win.Show();
            this.Close();
        }

    }
}
