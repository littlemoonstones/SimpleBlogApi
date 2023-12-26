using Microsoft.AspNetCore.Mvc;
using Blog.Core.IService;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ICommentService _commentService;

    public AuthorController(
        IPostService postService,
        ICommentService commentService
    )
    {
        _postService = postService;
        _commentService = commentService;
    }

    [HttpGet("{authorId}/posts")]
    public async Task<IActionResult> GetPostsByAuthor(Guid authorId)
    {
        var posts = await _postService.GetPostsByAuthorAsync(authorId);

        return Ok(posts.Select(post => new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Created = post.CreatedDate,
            Updated = post.UpdatedDate,
            AuthorName = post.Author.UserName!,
            AuthorId = post.AuthorId
        }));
    }

    [HttpGet("{authorId}/comments")]
    public async Task<IActionResult> GetCommentsByAuthor(Guid authorId)
    {
        var comments = await _commentService.GetCommentsByAuthorIdAsync(authorId);

        return Ok(comments.Select(comment => new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            Created = comment.CreatedDate,
            Author = comment.Author.UserName!,
            AuthorId = comment.AuthorId,
            PostId = comment.PostId
        }));
    }
}
