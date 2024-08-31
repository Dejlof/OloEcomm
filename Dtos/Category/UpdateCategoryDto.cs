using System.ComponentModel.DataAnnotations;

namespace OloEcomm.Dtos.Category
{
    public class UpdateCategoryDto
    {
        [Required]
        [MaxLength(30, ErrorMessage = "CategoryName can not be more than 30 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(80, ErrorMessage = "CategoryDescription can not be more than 80 characters")]
        public string Description { get; set; } = string.Empty;
    }
}
