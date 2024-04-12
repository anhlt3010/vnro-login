using System.Windows;
using VNRO_Login.Models;

namespace VNRO_Login
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Lấy các tham số truyền vào từ dòng lệnh
            string[] args = e.Args;

            if (args.Length == 3) 
            {
                VtcLoginResponse? response = await VtcService.loginAsync(args[0], args[1]);
                if (response != null && response.info != null)
                {
                    VtcService.StartGame(args[2], args[0], response?.info?.accessToken, response?.info?.billingAccessToken);
                    Current.Shutdown();
                }
            }
            else
            {
                // Khởi tạo MainWindow và truyền các tham số vào
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    }

}
