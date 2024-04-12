using Microsoft.Win32;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VNRO_Login
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpClient;
        private readonly string CLIENT_ID = "2aa32a67b771fcab4fd501273ef8b744";
        private readonly string CLIENT_SECRET = "9ecf8255d241f5e702714734e3a93afb";
        private readonly string VTC_LOGIN_API = "http://apisdk.vtcgame.vn/sdk/login?username={0}&password={1}&client_id={2}&client_secret={3}&grant_type=password&authen_type=0&device_type=1";

        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }

        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && txtUsername.Text.Length > 0)
            {
                textUsername.Visibility = Visibility.Collapsed;
            }
            else 
            { 
                textUsername.Visibility = Visibility.Visible; 
            }
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus();
        }


        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password) && txtPassword.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void txtRagexePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRagexePath.Text) && txtRagexePath.Text.Length > 0)
            {
                textRagexePath.Visibility = Visibility.Collapsed;
            }
            else
            {
                textRagexePath.Visibility = Visibility.Visible;
            }
        }

        private async void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Password))
            {
                btnStartGame.IsEnabled = false;
                string username = txtUsername.Text;
                string password = txtPassword.Password;
                

                // Start request to VTC API
                var loginResponse = await VtcService.loginAsync(username, password);
                if (loginResponse == null)
                {
                    MessageBox.Show("Không request được đến API của VTC vui lòng thử lại sau");
                }
                string? accessToken = loginResponse?.info?.accessToken;
                string? billingToken = loginResponse?.info?.billingAccessToken;

                VtcService.StartGame(txtRagexePath.Text, username, accessToken, billingToken);
                btnStartGame.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Chưa nhập username hoặc password");
            }    
        }

        private void btnClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRagexeSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe";
            bool? success = openFileDialog.ShowDialog();
            if (success == true)
            {
                txtRagexePath.Text = openFileDialog.FileName;
            }
        }
    }

}