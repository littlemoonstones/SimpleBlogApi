using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blog.Core.IService;
using static Blog.Api.Helpers.UserClaimsHelper;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    public PostController(
        IPostService postRService
    )
    {
        _postService = postRService;
    }

    private PostResponseDto MapToDto(Post post, string authorName)
    {
        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Created = post.CreatedDate,
            Updated = post.UpdatedDate,
            AuthorName = authorName,
            AuthorId = post.AuthorId
        };
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost(PostRequestDto post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var (userId, authorName) = GetUserDetails(User);
            var postEntity = new Post
            {
                Title = post.Title,
                Content = post.Content,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                AuthorId = userId
            };

            await _postService.CreatePostAsync(postEntity);
            return Ok(MapToDto(postEntity, authorName));
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null || post.Author == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(post, post.Author.UserName!));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePostById(Guid id)
    {
        try
        {
            var (userId, author) = GetUserDetails(User);
            await _postService.DeletePostAsync(id, userId);

            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(Guid id, PostRequestDto post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var (userId, authorName) = GetUserDetails(User);
            var postEntity = await _postService.UpdatePostAsync(
                new Post
                {
                    Id = id,
                    Title = post.Title,
                    Content = post.Content,
                    UpdatedDate = DateTime.Now,
                    AuthorId = userId
                }, userId);

            return Ok(MapToDto(postEntity, authorName));
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return BadRequest();
        }

    }


}
