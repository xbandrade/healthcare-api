using System.Windows;

namespace Client;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        // Login ok
        LoginGrid.Visibility = Visibility.Collapsed;
        MainGrid.Visibility = Visibility.Visible;
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        // Logout ok
        MainGrid.Visibility = Visibility.Collapsed;
        LoginGrid.Visibility = Visibility.Visible;
    }
}
