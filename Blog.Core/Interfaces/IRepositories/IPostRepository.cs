namespace Blog.Core.IRepository;
public interface IPostRepository
{
    Task<Post?> GetPostByIdAsync(Guid id);
    Task<IEnumerable<Post>> GetPostsByAuthorAsync(Guid authorId);
    Task<IEnumerable<Post>> GetAllPostsAsync();

    Task<Post> CreatePostAsync(Post post);
    Task<Post> UpdatePostAsync(Post post);
    Task DeletePostAsync(Post post);
    Task<bool> PostExistsAsync(Guid id);
}