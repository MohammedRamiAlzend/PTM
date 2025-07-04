namespace PTM.Application.DTOs.AuthDTOs;

public class RegisterRequestDto
{
    public required string Username { get; set; }
    public required string  Password { get; set; }
    public required string RoleName { get; set; }
}
