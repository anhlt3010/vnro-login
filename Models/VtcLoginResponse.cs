namespace VNRO_Login.Models
{
    public class VtcLoginResponse
    {
        public int error { get; set; }
        public string? message { get; set; }
        public AccountInfo? info { get; set; }
    }
}
