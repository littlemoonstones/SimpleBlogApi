using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blog.Core.IService;
using static Blog.Api.Helpers.UserClaimsHelper;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentController(
        ICommentService commentService
    )
    {
        _commentService = commentService;
    }

    private CommentResponseDto MapToDto(Comment comment, string author)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            Created = comment.CreatedDate,
            Author = author,
            AuthorId = comment.AuthorId,
            PostId = comment.PostId
        };
    }

    [HttpGet("post/{postId}/comments")]
    public async Task<IActionResult> GetCommentsByPostId(Guid postId)
    {
        var comments = await _commentService.GetCommentsByPostIdAsync(postId);

        return Ok(comments.Select(comment => MapToDto(comment, comment.Author.UserName!)));
    }

    [HttpPost("post/{postId}/[controller]")]
    [Authorize]
    public async Task<IActionResult> CreateComment(Guid postId, CommentRequestDto comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var (userId, author) = GetUserDetails(User);

            var commentEntity = new Comment
            {
                Content = comment.Content,
                CreatedDate = DateTime.Now,
                AuthorId = userId,
                PostId = postId
            };
            commentEntity = await _commentService.CreateCommentAsync(commentEntity);
            return Ok(MapToDto(commentEntity, author));
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    // delete a comment
    [HttpDelete("post/{postId}/[controller]/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid postId, Guid commentId)
    {
        try
        {
            var (userId, author) = GetUserDetails(User);

            await _commentService.DeleteCommentAsync(commentId, userId);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }


}
