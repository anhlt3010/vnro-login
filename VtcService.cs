using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using VNRO_Login.Models;

namespace VNRO_Login
{
    public static class VtcService
    {
        private static readonly string CLIENT_ID = "2aa32a67b771fcab4fd501273ef8b744";
        private static readonly string CLIENT_SECRET = "9ecf8255d241f5e702714734e3a93afb";
        private static readonly string VTC_LOGIN_API = "http://apisdk.vtcgame.vn/sdk/login?username={0}&password={1}&client_id={2}&client_secret={3}&grant_type=password&authen_type=0&device_type=1";

        public static async Task<VtcLoginResponse?> loginAsync(string username, string password)
        {
            HttpClient httpClient = new HttpClient();
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                return null;
            }

            string hashedPassword = GetMD5Hash(password);

            // Start request to VTC API
            string requestUrl = String.Format(VTC_LOGIN_API, username, hashedPassword, CLIENT_ID, CLIENT_SECRET);
            var response = await httpClient.GetFromJsonAsync<VtcLoginResponse>(requestUrl);
            
            return response;
        }

        public static void StartGame(string exePath, string username, string? accessToken, string? billingAccesstoken)
        {
            Boolean isRagexe = exePath.Contains("\\Ragexe.exe");
            string gamePath = isRagexe ? exePath.Replace("\\Ragexe.exe", "") : exePath.Replace("\\Ragnarok.exe", "");
            string startRagexe = String.Format(" 1rag1User={0} Token={1} BToken={2}", username, accessToken, billingAccesstoken);
            string startRagnarok = String.Format(" User={0} Token={1} BToken={2}", username, accessToken, billingAccesstoken);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe"; // Chạy lệnh từ cmd
            startInfo.Arguments = "/c start " + exePath + (isRagexe ? startRagexe : startRagnarok); // Đặt lệnh của bạn vào đây

            // Bật cửa sổ cmd ẩn và không hiển thị cửa sổ cmd
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = gamePath;

            // Khởi tạo Process với thông tin đã cấu hình
            using (Process cmdProcess = Process.Start(startInfo)!)
            {
                // Chờ cho quá trình cmd kết thúc
                cmdProcess.WaitForExit();
            }
        }

        private static string GetMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
