using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data.Enum;
using OloEcomm.Dtos.Review;
using OloEcomm.Interface;
using OloEcomm.Mappers;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductReposity _productReposity;
        public ReviewController(IReviewRepository reviewRepository, IProductReposity productReposity)
        {
            _reviewRepository = reviewRepository;
            _productReposity = productReposity;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews() 
        { 
        var reviews = await _reviewRepository.GetAllAsync();
        var reviewsDto = reviews.Select(s=>s.ToReviewDto()).ToList();
        return Ok(reviewsDto);

        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid) 
            { 
            return BadRequest(ModelState);
            }
            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
            {
                return NotFound();
            }
            return Ok(review.ToReviewDto());
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto, [FromRoute] int productId, [FromQuery] Rating rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _productReposity.productExists(productId))
            {
                return BadRequest("Product does not exist");
            }
            if (!Enum.IsDefined(typeof(Rating), rating))
            {
                return BadRequest("Invalid rating. Please select a value between 1 and 5.");
            }

            var reviewModel = reviewDto.ToCreateReviewDto(productId,rating);
            await _reviewRepository.CreateReviewAsync(reviewModel);

            return CreatedAtAction(nameof(GetById), new {id  = productId}, reviewModel.ToReviewDto());
        }

        [HttpPut("{id:int}")]
        public async Task <IActionResult> UpdateReview([FromRoute] int id, [FromQuery] Rating rating, [FromBody] UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.IsDefined(typeof(Rating), rating))
            {
                return BadRequest("Invalid rating. Please select a value between 1 and 5.");
            }

            var reviewModel = await _reviewRepository.UpdateReviewAsync(id, updateReviewDto.ToUpdateReviewDto(rating));

            if (reviewModel == null)
            {
                return NotFound();
            }

            return Ok(reviewModel.ToReviewDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int id) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var review = await _reviewRepository.DeleteReviewAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return Ok($"Review with id: {id} sucessfully deleted");
        }
    }
}
