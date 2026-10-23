using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Services.Interfaces;

namespace SocialMedia.Services.Services
{
    public class PostService : IPostService
    {
        //public readonly IPostRepository _postRepository;
        //public readonly IUserRepository _userRepository;
        public readonly IBaseRepository<Post> _postRepository;
        public readonly IBaseRepository<User> _userRepository;

        public PostService(
            IBaseRepository<Post> postRepository,
            IBaseRepository<User> userRepository
            //IPostRepository postRepository, IUserRepository userRepository
            )
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _postRepository.GetAll();
        }

        public async Task<Post> GetPostsByIdAsync(int id)
        {
            return await _postRepository.GetById(id);
        }

        public async Task InsertPost(Post post)
        {
            var user = await _userRepository.GetById(post.UserId);
            if (user == null)
            {
                throw new Exception("El usuario no existe");
            }

            if (ContainsForbiddenContent(post.Description))
            {
                throw new Exception("El contenido no es permitido");
            }

            await _postRepository.Add(post);
        }

        public async Task UpdatePost(Post post)
        {
            await _postRepository.Update(post);
        }

        public async Task DeletePost(int id)
        {
            await _postRepository.Delete(id);
        }

        // Lista de palabras o expresiones no permitidas
        public readonly string[] ForbiddenWords =
        {
            "violencia",
            "odio",
            "grosería",
            "discriminación"
        };

        public bool ContainsForbiddenContent(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            foreach (var word in ForbiddenWords)
            {
                if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

    }

}
