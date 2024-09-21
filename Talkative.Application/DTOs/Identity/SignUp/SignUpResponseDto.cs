namespace Talkative.Application.DTOs.Identity.SignUp;

public record SignUpResponseDto(Guid Id, string FirstName, string LastName, string Email, long CreatedAt);