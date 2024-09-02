using OloEcomm.Model;

namespace OloEcomm.Interface
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();

        Task<Review?> GetByIdAsync(int id);

        Task <Review> CreateReviewAsync (Review review);    

        Task <Review?> UpdateReviewAsync (int id, Review reviewModel);

        Task<Review?> DeleteReviewAsync (int id);
    }
}
