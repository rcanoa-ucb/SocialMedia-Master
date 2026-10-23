using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Services.Interfaces;
using SocialMedia.Services.Validators;

namespace SocialMedia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        //private readonly IPostRepository _postRepository;
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly CrearPostDtoValidator _crearValidator;
        private readonly ActualizarPostDtoValidator _actualizarValidator;

        public PostController(IMapper mapper, 
            //IPostRepository postRepository,
            IPostService postService,
            CrearPostDtoValidator crearValidator,
            ActualizarPostDtoValidator actualizarValidator
)
        {
            //_postRepository = postRepository;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _postService = postService;
        }

        #region Sin DTOs
        //[HttpGet]
        //public async Task<IActionResult> GetPosts()
        //{
        //    var posts = await _postRepository.GetAllPostsAsync();
        //    return Ok(posts);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetPostById(int id)
        //{
        //    var post = await _postRepository.GetPostsByIdAsync(id);
        //    return Ok(post);
        //}

        //[HttpPost]
        //public async Task<IActionResult> InsertPost(Post newPost)
        //{
        //    await _postRepository.InsertPost(newPost);
        //    return Created($"api/post/{newPost.Id}", newPost);
        //}

        //[HttpDelete]
        //public async Task<IActionResult> DeletePost(Post post)
        //{
        //    await _postRepository.DeletePost(post);
        //    return NoContent();
        //}

        //[HttpPut]
        //public async Task<IActionResult> UpdatePost(Post post)
        //{
        //    await _postRepository.UpdatePost(post);
        //    return NoContent();
        //}
        #endregion

        #region Con DTOs
        //[HttpGet("dto")]
        //public async Task<IActionResult> GetPostsDto()
        //{
        //    var posts = await _postRepository.GetAllPostsAsync();
        //    var postsDto = posts.Select(p => new PostDto
        //    {
        //        Id = p.Id,
        //        UserId = p.UserId,
        //        Date = p.Date.ToString("dd-MM-yyyy"),
        //        Description = p.Description,
        //        Image = p.Imagen
        //    });
        //    return Ok(postsDto);
        //}

        //[HttpGet("dto/{id}")]
        //public async Task<IActionResult> GetPostByIdDto(int id)
        //{
        //    var post = await _postRepository.GetPostsByIdAsync(id);
        //    var postDto = new PostDto
        //    {
        //        Id = post.Id,
        //        UserId = post.UserId,
        //        Date = post.Date.ToString("dd-MM-yyyy"),
        //        Description = post.Description,
        //        Image = post.Imagen
        //    };
        //    return Ok(postDto);
        //}

        //[HttpPost("dto")]
        //public async Task<IActionResult> InsertPostDto(PostDto postDto)
        //{
        //    var post = new Post
        //    {
        //        Id = postDto.Id,
        //        UserId = postDto.UserId,
        //        Date = Convert.ToDateTime(postDto.Date),
        //        Description = postDto.Description,
        //        Imagen = postDto.Image
        //    };
        //    await _postRepository.InsertPost(post);
        //    return Ok(post);
        //}

        //[HttpPut("dto/{id}")]
        //public async Task<IActionResult> UpdatePostDto(int id, [FromBody] PostDto postDto)
        //{
        //    if (id != postDto.Id)
        //        return BadRequest("El ID del post no coincide.");

        //    var post = await _postRepository.GetPostsByIdAsync(id);
        //    if (post == null)
        //        return NotFound("Post no encontrado.");

        //    // Mapear valores del DTO a la entidad
        //    post.UserId = postDto.UserId;
        //    post.Date = Convert.ToDateTime(postDto.Date);
        //    post.Description = postDto.Description;
        //    post.Imagen = postDto.Image;

        //    await _postRepository.UpdatePost(post);

        //    return Ok(post);
        //}

        //[HttpDelete("dto/{id}")]
        //public async Task<IActionResult> DeletePostDto(int id)
        //{
        //    var post = await _postRepository.GetPostsByIdAsync(id);
        //    if (post == null)
        //        return NotFound("Post no encontrado.");

        //    await _postRepository.DeletePost(post);

        //    return NoContent(); // 204 sin contenido
        //}

        #endregion

        #region Con Mapper
        [HttpGet("dto/mapper/")]
        public async Task<IActionResult> GetPostsDtoMapper()
        {
            var posts = await _postService.GetAllPostsAsync();
            var postsDto = _mapper.Map<IEnumerable<PostDto>>(posts);
            return Ok(postsDto);
        }

        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetPostByIdDtoMapper(int id)
        {
            var post = await _postService.GetPostsByIdAsync(id);
            if (post == null)
                return NotFound("Post no encontrado.");

            var postDto = _mapper.Map<PostDto>(post);
            return Ok(postDto);
        }

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertPostDtoMapper(PostDto postDto)
        {
            // Validar el DTO
            var validationResult = await _crearValidator.ValidateAsync(postDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Error de validación",
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            try
            {
                var post = _mapper.Map<Post>(postDto);
                await _postService.InsertPost(post);

                var resultDto = _mapper.Map<PostDto>(post);
                return CreatedAtAction(nameof(GetPostByIdDtoMapper), new { id = post.Id }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el post", error = ex.Message });
            }
        }

        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdatePostDtoMapper(int id, [FromBody] PostDto postDto)
        {
            if (id != postDto.Id)
                return BadRequest("El ID del post no coincide.");

            // Validar el DTO
            var validationResult = await _actualizarValidator.ValidateAsync(postDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Error de validación",
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            var post = await _postService.GetPostsByIdAsync(id);
            if (post == null)
                return NotFound(new { message = "Post no encontrado." });

            try
            {
                _mapper.Map(postDto, post);
                await _postService.UpdatePost(post);

                var resultDto = _mapper.Map<PostDto>(post);
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el post", error = ex.Message });
            }

        }

        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeletePostDtoMapper(int id)
        {
            // Validar que el ID sea válido
            if (id <= 0)
                return BadRequest(new { message = "El ID debe ser mayor que cero." });

            var post = await _postService.GetPostsByIdAsync(id);
            if (post == null)
                return NotFound(new { message = "Post no encontrado." });

            try
            {
                await _postService.DeletePost(id);
                return NoContent(); // 204 sin contenido
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el post", error = ex.Message });
            }

        }
        #endregion

    }
}
