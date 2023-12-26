namespace Blog.Core.IService;
public interface IPostService
{
    Task<Post> CreatePostAsync(Post post);
    Task<Post?> GetPostByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetPostsByAuthorAsync(Guid authorId);
    Task<IEnumerable<Post>> GetAllPostsAsync();
    Task DeletePostAsync(Guid postId, Guid userId);
    Task<Post> UpdatePostAsync(Post post, Guid userId);
}