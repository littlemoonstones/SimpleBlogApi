namespace Blog.Core.IService;
public interface ICommentService
{
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<Comment?> GetCommentByIdAsync(Guid id);
    Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId);
    Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(Guid authorId);
    Task DeleteCommentAsync(Guid commentId, Guid userId);
}