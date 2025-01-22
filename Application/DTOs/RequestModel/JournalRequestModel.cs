using System.ComponentModel.DataAnnotations;

namespace AiComp.Application.DTOs.RequestModel
{
    public class JournalRequestModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string? Content { get; set; }
    }
}
