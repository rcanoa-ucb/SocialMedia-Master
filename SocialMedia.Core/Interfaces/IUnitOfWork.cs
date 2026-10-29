using SocialMedia.Core.Entities;

namespace SocialMedia.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //IBaseRepository<Post> PostRepository { get; }
        IPostRepository PostRepository { get; }
        IBaseRepository<User> UserRepository { get; }
        IBaseRepository<Comment> CommentRepository { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
