using SocialMedia.Core.Entities;

namespace SocialMedia.Core.Interfaces
{
    //public interface IPostRepository
    //{
    //    Task<IEnumerable<Post>> GetAllPostsAsync();
    //    Task<Post> GetPostsByIdAsync(int id);
    //    Task InsertPost(Post post);
    //    Task UpdatePost(Post post);
    //    Task DeletePost(Post post);
    //}

    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<IEnumerable<Post>> GetAllPostsByUserAsync(int userId);
    }
}
