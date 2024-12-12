using AiComp.Domain.Entities;

namespace AiComp.Application.DTOs.RequestModel
{
    public class ProfileCreateModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Age { get; set; }
        public string? Gender { get; set; }
        public string? Occupation { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullNameOfNextOfKin { get; set; }
        public string? ContactOfNextOfKin { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
