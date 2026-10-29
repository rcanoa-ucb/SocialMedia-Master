using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Helpers;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using SocialMedia.Services.Interfaces;

namespace SocialMedia.Services.Services
{
    public class PostService : IPostService
    {
        //public readonly IPostRepository _postRepository;
        //public readonly IUserRepository _userRepository;
        //public readonly IBaseRepository<Post> _postRepository;
        //public readonly IBaseRepository<User> _userRepository;

        public readonly IUnitOfWork _unitOfWork;

        //public PostService(
        //    IBaseRepository<Post> postRepository,
        //    IBaseRepository<User> userRepository
        //    //IPostRepository postRepository, IUserRepository userRepository
        //    )
        //{
        //    _postRepository = postRepository;
        //    _userRepository = userRepository;
        //}

        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<Post>> GetAllPostsAsync(PostQueryFilter? filters)
        {
            //var posts = await _postRepository.GetAll();
            var posts = await _unitOfWork.PostRepository.GetAll();

            if (filters.userId != null)
            {
                posts = posts.Where(x => x.UserId == filters.userId);
            }
            if (filters.Date != null)
            {
                string fechaAux = Procesos.ParseFechaFlexible(filters.Date);
                if (fechaAux != null)
                    posts = posts.Where(x => x.Date.ToShortDateString() == Convert.ToDateTime(fechaAux).ToShortDateString());
            }
            if (filters.Description != null)
            {
                posts = posts.Where(x => x.Description.ToLower().Contains(filters.Description.ToLower()));
            }

            return posts;

        }

        public async Task<Post> GetPostsByIdAsync(int id)
        {
            //return await _postRepository.GetById(id);
            return await _unitOfWork.PostRepository.GetById(id);
        }

        public async Task InsertPost(Post post)
        {
            //var user = await _userRepository.GetById(post.UserId);
            var user = await _unitOfWork.UserRepository.GetById(post.UserId);
            if (user == null)
            {
                throw new BusinessException("El usuario no existe");
            }

            if (ContainsForbiddenContent(post.Description))
            {
                throw new BusinessException("El contenido no es permitido");
            }

            var userPost = await _unitOfWork.PostRepository.GetAllPostsByUserAsync(post.UserId);
            if (userPost.Count() < 10)
            {
                var lastPost = userPost.OrderByDescending(x => x.Date).FirstOrDefault();
                if ((DateTime.Now - lastPost.Date).TotalDays < 7)
                {
                    throw new Exception("No puedes publicar este post");
                }
            }

            await _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePost(Post post)
        {
            await _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePost(int id)
        {
            await _unitOfWork.PostRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
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
