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

        private readonly ILogger<ReviewController> _logger;
        public ReviewController(IReviewRepository reviewRepository, IProductReposity productReposity, UserManager<User> userManager, ILogger<ReviewController> logger)
        {
            _reviewRepository = reviewRepository;
            _productReposity = productReposity;
            _userManager = userManager; 
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews() 
        { 
            _logger.LogInformation("Fetching all reviews"); 
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
                _logger.LogWarning("Unauthorized access attempt to review.");
                return Unauthorized();
            }
            var reviews = await _reviewRepository.GetUserCommentAsync(user);
            var reviewsDto = reviews.Select(s => s.ToReviewDto()).ToList();
            _logger.LogInformation("Fetching reviews for user: {User}", user);
            return Ok(reviewsDto);
        }

        [HttpGet("GetUsersReviews")]
        public async Task<IActionResult> GetUserReviews(string username)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for review request.");
                return BadRequest(ModelState);
            }
            var reviews = await _reviewRepository.GetUserCommentAsync(username);
            var reviewsDto = reviews.Select(s => s.ToReviewDto()).ToList();
            _logger.LogInformation("Fetching reviews for user: {User}", username);
            return Ok(reviewsDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid) 
            { 
                _logger.LogWarning("Invalid model state for review request.");
            return BadRequest(ModelState);
            }
            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
            {
                _logger.LogWarning("Review with id: {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Fetching review with id: {Id}", id);
            return Ok(review.ToReviewDto());
        }

        [HttpPost("{productId:int}")]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto, [FromRoute] int productId, [FromQuery] Rating rating)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for review request.");
                return BadRequest(ModelState);
            }

            if (!await _productReposity.productExists(productId))
            {
                _logger.LogWarning("Product does not exist.");
                return BadRequest("Product does not exist");
            }
            if (!Enum.IsDefined(typeof(Rating), rating))
            {
                _logger.LogWarning("Invalid rating. Please select a value between 1 and 5.");
                return BadRequest("Invalid rating. Please select a value between 1 and 5.");
            }

            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null) 
            { 
                _logger.LogWarning("User  not found.", appUser);
             return Unauthorized("User not permitted");
            }

            var reviewModel = reviewDto.ToCreateReviewDto(productId,rating);
            reviewModel.UserId = appUser.Id;
            reviewModel.CreatedBy = appUser.UserName ?? throw new InvalidOperationException("UserName cannot be null");

           _logger.LogInformation("Creating review for product: {Product}", productId);
            await _reviewRepository.CreateReviewAsync(reviewModel, user);

            return CreatedAtAction(nameof(GetById), new {id  = productId}, reviewModel.ToReviewDto());
        }

        [HttpPut("{id:int}")]
        public async Task <IActionResult> UpdateReview([FromRoute] int id, [FromQuery] Rating rating, [FromBody] UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for review request.");
                return BadRequest(ModelState);
            }

            if (!Enum.IsDefined(typeof(Rating), rating))
            {
                _logger.LogWarning("Invalid rating. Please select a value between 1 and 5.");
                return BadRequest("Invalid rating. Please select a value between 1 and 5.");
            }

            var reviewModel = await _reviewRepository.UpdateReviewAsync(id, updateReviewDto.ToUpdateReviewDto(rating));

            if (reviewModel == null)
            {
                _logger.LogWarning("Review with id: {Id} not found.", id);
                return NotFound();
            }

            var currentUserId = User.GetUsername();
            if (reviewModel.CreatedBy != currentUserId)
            {
                _logger.LogWarning("Unauthorized access attempt to review.");
                return Unauthorized("You are not authorized to update this review.");
            }

       _logger.LogInformation("Updating review with id: {Id}", id);   
            return Ok(reviewModel.ToReviewDto());
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview([FromRoute] int id) 
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for review request.");
                return BadRequest(ModelState);
            }
            var review = await _reviewRepository.DeleteReviewAsync(id);

            if (review == null)
            {
                _logger.LogWarning("Review with id: {Id} not found.", id);
                return NotFound();
            }

             _logger.LogInformation("Deleting review with id: {Id}", id);
            return Ok($"Review with id: {id} sucessfully deleted");
        }

        [HttpDelete("DeleteMyReview{id:int}")]
        public async Task<IActionResult> DeleteMyReview([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for review request.");
                return BadRequest(ModelState);
            }

            var user = User.GetUsername();

            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to review.");
                return Unauthorized();
            }
            var review = await _reviewRepository.DeleteUserReviewAsync(id, user);
            if (review == null)
            {
                _logger.LogWarning("Review with id: {Id} not found.", id);
                return NotFound();
            }

            return Ok($"Review with id: {id} sucessfully deleted");
            _logger.LogInformation("Deleting review with id: {Id}", id);
        }
    }
}
