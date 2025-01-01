using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OloEcomm.Data.Enum;
using OloEcomm.Dtos.Review;
using OloEcomm.Extensions;
using OloEcomm.Interface;
using OloEcomm.Mappers;
using OloEcomm.Model;

namespace OloEcomm.Controllers
{
    [Route("OloEcomm/Review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductReposity _productReposity;
        private readonly UserManager<User> _userManager;
        public ReviewController(IReviewRepository reviewRepository, IProductReposity productReposity, UserManager<User> userManager)
        {
            _reviewRepository = reviewRepository;
            _productReposity = productReposity;
            _userManager = userManager; 
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews() 
        { 
        var reviews = await _reviewRepository.GetAllAsync();
        var reviewsDto = reviews.Select(s=>s.ToReviewDto()).ToList();
        return Ok(reviewsDto);
        }

        [HttpGet("GetMyReviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }
            var reviews = await _reviewRepository.GetUserCommentAsync(user);
            var reviewsDto = reviews.Select(s => s.ToReviewDto()).ToList();
            return Ok(reviewsDto);
        }

        [HttpGet("GetUsersReviews")]
        public async Task<IActionResult> GetUserReviews(string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviews = await _reviewRepository.GetUserCommentAsync(username);
            var reviewsDto = reviews.Select(s => s.ToReviewDto()).ToList();
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
        [Authorize]
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

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null) 
            { 
             return Unauthorized("User not permitted");
            }

            var reviewModel = reviewDto.ToCreateReviewDto(productId,rating);
            reviewModel.UserId = appUser.Id;


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

            var currentUserId = User.GetUsername();
            if (reviewModel.CreatedBy != currentUserId)
            {
                return Unauthorized("You are not authorized to update this review.");
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

        [HttpDelete("DeleteMyReview{id:int}")]
        public async Task<IActionResult> DeleteMyReview([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.GetUsername();

            if (user == null)
            {
                return Unauthorized();
            }
            var review = await _reviewRepository.DeleteUserReviewAsync(id, user);
            if (review == null)
            {
                return NotFound();
            }

            return Ok($"Review with id: {id} sucessfully deleted");
        }
    }
}
