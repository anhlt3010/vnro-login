namespace VNRO_Login.Models
{
    public class AccountInfo
    {
        public string? accessToken { get; set; }
        public string? billingAccessToken { get; set; }
        public long? expiration { get; set; }
        public string? accountId { get; set; }
        public string? accountName { get; set; }
        public string? userStatus { get; set; }
        public string? avatarUrl { get; set; }
        public string? gameVersion { get; set; }
        public string? gameOrientation { get; set; }
        public string? syntax { get; set; }
        public string? errorCode { get; set; }
        public string? errorMessage { get; set; }
    }
}
