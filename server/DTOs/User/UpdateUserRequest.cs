namespace Server.DTOs.User;

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
}
