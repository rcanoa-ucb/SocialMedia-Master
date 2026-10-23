using SocialMedia.Core.Entities;

namespace SocialMedia.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post> GetPostsByIdAsync(int id);
        Task InsertPost(Post post);
        Task UpdatePost(Post post);
        Task DeletePost(int id);
    }

}
