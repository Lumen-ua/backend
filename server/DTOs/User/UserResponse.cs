namespace Server.DTOs.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}