using System.ComponentModel.DataAnnotations;

namespace AiComp.Application.DTOs.RequestModel
{
    public class UserRequestModel
    {
        //[Required]
        public required string? Email { get; set; }

        //[Required]
        public required string? Password { get; set; }

        //[Required]
        //[Compare("Password")]  
        public required string? ComparePassword { get; set; }

    }
}
