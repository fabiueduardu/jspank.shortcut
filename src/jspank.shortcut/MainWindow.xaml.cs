using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace jspank.shortcut
{
    public partial class MainWindow : Window
    {
        string key_extension = ".exe";
        RegistryKey RegistryKeys
        {
            get
            {
                return Registry.LocalMachine.OpenSubKey("SOFTWARE", true).OpenSubKey("Microsoft", true).OpenSubKey("Windows", true).OpenSubKey("CurrentVersion", true).OpenSubKey("App Paths", true);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Load_Alias();
            this.Bkp();
        }

        void Bkp()
        {
            var path_bkp = string.Concat(Environment.CurrentDirectory, @"\RegistryKeys.Bkp");
            if (!File.Exists(path_bkp))
            {
                var collection = new Collection<string>();
                collection.Add(Environment.UserName);
                foreach (string key in this.RegistryKeys.GetSubKeyNames())
                {
                    var value = this.RegistryKeys.OpenSubKey(key).GetValue(null) as string;
                    collection.Add(string.Concat(key, "|", value));
                }

                File.AppendAllLines(path_bkp, collection);
            }
        }

        void Load_Alias()
        {
            this.lst_alias.Items.Clear();
            var collections = this.RegistryKeys.GetSubKeyNames();
            foreach (string key in collections.OrderBy(a => a))
                this.lst_alias.Items.Add(key);

            this.txt_program_alias.Text =
           this.txt_program_path.Text = string.Empty;
            this.btn_remove.IsEnabled = false;
        }

        void lst_alias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            if (item.SelectedItem != null)
            {
                var key = item.SelectedItem.ToString();
                var value = this.RegistryKeys.OpenSubKey(key).GetValue(null);
                this.txt_program_alias.Text = key;
                this.txt_program_path.Text = value as string;

                this.btn_remove.IsEnabled = true;
            }

        }

        void btn_add_Click(object sender, RoutedEventArgs e)
        {
            var key = this.txt_program_alias.Text;
            key += !key.EndsWith(key_extension) ? key_extension : string.Empty;

            if (this.RegistryKeys.OpenSubKey(key) != null)
            {
                if (MessageBoxResult.Yes != MessageBox.Show(string.Format("The \"{0}\" key already exists, you want to overwrite it", this.txt_program_alias.Text), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question))
                    return;
            }

            if (string.IsNullOrEmpty(this.txt_program_alias.Text) || string.IsNullOrEmpty(this.txt_program_path.Text))
            {
                MessageBox.Show(string.Format("Enter the key and value", this.txt_program_alias.Text), "Attention");
                return;
            }

            this.RegistryKeys.CreateSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree).SetValue(string.Empty, txt_program_path.Text);
            this.Load_Alias();

        }

        void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes != MessageBox.Show(string.Format("Are you sure you want to delete the \"{0}\" key", this.txt_program_alias.Text), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question))
                return;

            var key = this.txt_program_alias.Text;
            key += !key.EndsWith(key_extension) ? key_extension : string.Empty;

            this.RegistryKeys.DeleteSubKey(key);
            this.Load_Alias();
        }

        void btn_new_Click(object sender, RoutedEventArgs e)
        {
            this.Load_Alias();
        }

    }
}
