namespace Blog.Core.Dtos
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }

    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
        // public string RefreshToken { get; set; }
    }
}
