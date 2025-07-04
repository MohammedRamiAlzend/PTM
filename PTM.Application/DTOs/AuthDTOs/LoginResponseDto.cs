namespace PTM.Application.DTOs.AuthDTOs;

public class LoginResponseDto
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
}
