namespace OloEcomm.Dtos.Account
{
    public class NewUserDto
    {
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }   = string.Empty ;

        public string? Role { get; set; }
        public string? Token { get; set; } = string.Empty;

        public string? RefreshToken { get; set; } = string.Empty;

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
