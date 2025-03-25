using Microsoft.EntityFrameworkCore;
using OloEcomm.Data;
using OloEcomm.Interface;
using OloEcomm.Model;
using System.Reflection.Metadata;

namespace OloEcomm.Repository
{
    public class ReviewRepository : IReviewRepository
    {

        private readonly ApplicationDbContext _context;

        public ReviewRepository( ApplicationDbContext context) 
        {
        _context = context;
        }   
        public async Task<Review> CreateReviewAsync(Review review ,string username)
        {
            var order = await _context.Orders.Include(s=>s.OrderDetails).FirstOrDefaultAsync(x=>x.OrderedBy  == username && x.OrderDetails.Any(s=>s.ProductId == review.ProductId));
            if (order == null)
            {
                throw new InvalidOperationException("You have not purchased this product.");
            }

            if(order.OrderDetails.Any(s=> s.OrderStatus != "Delivered"))
            {
                throw new InvalidOperationException("You can only review a product after it has been delivered.");
            }
            var reviewModel = new Review
            {
                ProductId = review.ProductId,
                Comment = review.Comment,
                Rating = review.Rating,
                ReviewDate = review.ReviewDate,
            };
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (review == null)
            {
                return null;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> DeleteUserReviewAsync (int id, string username)
        {
            var review = await _context.Reviews.Where(s => s.User.UserName == username).FirstOrDefaultAsync(x => x.Id == id);

            if (review == null)
            {
                return null;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return review;
        }
    

        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews.Include(r=>r.User).OrderByDescending(s=>s.ReviewDate).ToListAsync();
        }

        public async Task<List<Review>> GetUserCommentAsync(string username)
        {
            return await _context.Reviews.Include(r => r.User).Where(s => s.User.UserName == username).OrderByDescending(s=>s.ReviewDate).ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (review == null) 
            { 
                return null;
            }
            return review;
        }

        public async Task<Review?> UpdateReviewAsync(int id, Review reviewModel)
        {
            var existingReview = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (existingReview == null)
            {
                return null;
            }
            
            existingReview.Comment = reviewModel.Comment;
            existingReview.Rating = reviewModel.Rating;
          

            await _context.SaveChangesAsync();
            return existingReview;
        }
    }
}
