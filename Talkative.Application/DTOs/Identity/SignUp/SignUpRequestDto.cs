namespace Talkative.Application.DTOs.Identity.SignUp;

public record SignUpRequestDto
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}