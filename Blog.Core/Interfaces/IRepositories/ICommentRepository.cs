namespace Blog.Core.IRepository;
public interface ICommentRepository
{
    Task<Comment?> GetCommentByIdAsync(Guid id);

    Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid postId);
    
    Task<IEnumerable<Comment>> GetCommentsByAuthorAsync(Guid authorId);
    Task<Comment> CreateCommentAsync(Comment comment);
    Task DeleteCommentAsync(Comment comment);
    Task<bool> CommentExistsAsync(Guid id);

}