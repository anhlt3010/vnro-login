using Microsoft.Win32;
using System.Net.Http;
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
        public MainWindow()
        {
            InitializeComponent();
            txtUsername.Text = VNROLogin.Default.Username;
            txtRagexePath.Text = VNROLogin.Default.RagexePath;
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

                // Save username and exe path to config
                VNROLogin.Default.Username = txtUsername.Text;
                VNROLogin.Default.RagexePath = txtRagexePath.Text;
                VNROLogin.Default.Save();

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
                VNROLogin.Default.RagexePath = txtRagexePath.Text;
                VNROLogin.Default.Save();
            }
        }
    }

}