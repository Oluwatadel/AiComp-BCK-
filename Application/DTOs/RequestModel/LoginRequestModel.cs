namespace AiComp.Application.DTOs.RequestModel
{
    public class LoginRequestModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
