using System.Windows;
using System.Windows.Controls;
using Client.Services;

namespace Client;
public partial class MainWindow : Window
{
    private ApiClient _apiClient;
    public MainWindow()
    {
        InitializeComponent();
        _apiClient = new();
    }

    private async void LoginButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        LoginButton.IsEnabled = false;
        UsernameTextBox.IsEnabled = false;
        PasswordBox.IsEnabled = false;
        try
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            InvalidLoginTextBlock.Visibility = Visibility.Collapsed;
            LoggingTextBlock.Visibility = Visibility.Visible;
            var loginOk = await _apiClient.Login(username, password);
            LoggingTextBlock.Visibility = Visibility.Collapsed;
            if (loginOk is null)
            {
                InvalidLoginTextBlock.Visibility = Visibility.Visible;
                return;
            }
            if (loginOk == "Exception")
            {
                MessageBox.Show("The API service is unreachable");
                return;
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                InvalidLoginTextBlock.Visibility = Visibility.Visible;
                return;
            }
            InvalidLoginTextBlock.Visibility = Visibility.Collapsed;
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
            LoginGrid.Visibility = Visibility.Collapsed;
            MainGrid.Visibility = Visibility.Visible;
        }
        finally
        {
            LoginButton.IsEnabled = true;
            UsernameTextBox.IsEnabled = true;
            PasswordBox.IsEnabled = true;
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        _apiClient = new();
        MainGrid.Visibility = Visibility.Collapsed;
        LoginGrid.Visibility = Visibility.Visible;
    }
}
