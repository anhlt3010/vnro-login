using Microsoft.Win32;
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
        private static readonly string LAUNCHER_FILE = "Ragnarok.exe";
        private static readonly string GAME_FILE = "Ragexe.exe";

        public MainWindow()
        {
            InitializeComponent();
            txtUsername.Text = VNROLogin.Default.Username;
            txtRagexePath.Text = VNROLogin.Default.RagexePath;
            if (IsSavedTokenValid())
            {
                ShowPanelInfo();
            }
            else
            {
                ShowPanelLogin();
            }
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
            if (e.ChangedButton == MouseButton.Left)
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
            StartGame(GAME_FILE);
        }

        private async void btnStartLauncher_Click(object sender, RoutedEventArgs e)
        {
            StartGame(LAUNCHER_FILE);
        }

        private void btnClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRagexeSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            // openFileDialog.Filter = "Executable files (*.exe)|*.exe";
            bool? success = openFolderDialog.ShowDialog();
            if (success == true)
            {
                txtRagexePath.Text = openFolderDialog.FolderName;
                VNROLogin.Default.RagexePath = txtRagexePath.Text;
                VNROLogin.Default.Save();
            }
        }

        private void textLogout_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearUserLoginInfo();
            ShowPanelLogin();
        }

        private async void StartGame(string file)
        {
            try
            {
                // If token still valid
                if (this.IsSavedTokenValid())
                {
                    VtcService.StartGame(VNROLogin.Default.RagexePath + "\\" + file,
                        VNROLogin.Default.Username,
                        VNROLogin.Default.AccessToken,
                        VNROLogin.Default.BillingAccessToken);
                    return;
                }
                else
                {
                    // Show login panel
                    ShowPanelLogin();
                }

                if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Password))
                {
                    btnStartGame.IsEnabled = false;

                    string username = txtUsername.Text;
                    string password = txtPassword.Password;


                    // Start request to VTC API
                    var loginResponse = await VtcService.loginAsync(username, password);
                    if (loginResponse == null)
                    {
                        MessageBox.Show("Không request được đến API của VTC vui lòng thử lại sau");
                        return;
                    }

                    if (loginResponse.error != 200)
                    {
                        MessageBox.Show(loginResponse.message);
                        return;
                    }

                    string? accessToken = loginResponse?.info?.accessToken;
                    string? billingToken = loginResponse?.info?.billingAccessToken;

                    // Save username and exe path to config
                    VNROLogin.Default.Username = txtUsername.Text;
                    VNROLogin.Default.RagexePath = txtRagexePath.Text;

                    // Save token
                    VNROLogin.Default.AccessToken = accessToken;
                    VNROLogin.Default.BillingAccessToken = billingToken;
                    VNROLogin.Default.TokenExpTime = (long)(loginResponse?.info?.expiration);
                    VNROLogin.Default.Save();

                    VtcService.StartGame(txtRagexePath.Text + "\\" + file, username, accessToken, billingToken);
                    ShowPanelInfo();
                }
                else
                {
                    MessageBox.Show("Chưa nhập username hoặc password");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi không xác định, vui lòng thử lại");
            }
            finally
            {
                btnStartGame.IsEnabled = true;
            }
        }

        private Boolean IsSavedTokenValid()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var TokenExp = VNROLogin.Default.TokenExpTime;
            return now < TokenExp &&
                    !string.IsNullOrEmpty(VNROLogin.Default.RagexePath) &&
                    !string.IsNullOrEmpty(VNROLogin.Default.Username) &&
                    !string.IsNullOrEmpty(VNROLogin.Default.AccessToken) &&
                    !string.IsNullOrEmpty(VNROLogin.Default.BillingAccessToken);
        }

        private void ShowPanelLogin()
        {
            panelLogin.Visibility = Visibility.Visible;
            panelInfo.Visibility = Visibility.Collapsed;
        }

        private void ShowPanelInfo()
        {
            panelInfo.Visibility = Visibility.Visible;
            panelLogin.Visibility = Visibility.Collapsed;
            lblAccountName.Content = VNROLogin.Default.Username;
        }

        private void ClearUserLoginInfo()
        {
            VNROLogin.Default.AccessToken = "";
            VNROLogin.Default.BillingAccessToken = "";
            VNROLogin.Default.TokenExpTime = 0;
        }
    }

}