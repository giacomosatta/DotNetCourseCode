namespace DotnetAPI.Models.Dtos;

public partial class UserForLoginConfirmationDto
{
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordHSalt{ get; set; } = [];
}