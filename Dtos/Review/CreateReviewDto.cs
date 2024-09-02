using OloEcomm.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Review
{
    public class CreateReviewDto
    {
        [MaxLength(20, ErrorMessage = "Comment can not be more than 50 characters")]
        [Required]
        public string Comment { get; set; } = string.Empty;

     
    }
}
