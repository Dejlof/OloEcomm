using OloEcomm.Dtos.Category;
using OloEcomm.Model;

namespace OloEcomm.Mappers
{
    public static class CategoryMappers
    {
       
        public static CategoryDto ToCategoryDto (this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
               

            };
        }

        public static Category ToCreateCategoryDto(this CreateCategoryDto categoryDto)
        {
            return new Category 
            { 
                Name = categoryDto.Name,
                Description = categoryDto.Description,
            };

        }

        public static Category ToUpdateCategoryDto(this UpdateCategoryDto categoryDto)
        {
            return new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
            };

        }

    }
}
