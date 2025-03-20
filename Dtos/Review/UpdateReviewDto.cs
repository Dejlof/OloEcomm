using OloEcomm.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Review
{
    public class UpdateReviewDto
    {
        [MaxLength(200, ErrorMessage = "Comment can not be more than 200 characters")]
        [Required]
        public string Comment { get; set; } = string.Empty;

   
    }
}
