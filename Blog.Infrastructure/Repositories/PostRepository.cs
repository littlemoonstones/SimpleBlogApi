using Blog.Core.IRepository;
using Microsoft.EntityFrameworkCore;
namespace Blog.Infrastructure.Repositories;
public class PostRepository : IPostRepository
{
    private readonly BlogDbContext _dbContext;
    public PostRepository(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async public Task<Post> CreatePostAsync(Post post)
    {
        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();
        return post;
    }

    async public Task DeletePostAsync(Post post)
    {
        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
    }

    async public Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        return await _dbContext.Posts.Include(p => p.Author).ToListAsync();
    }

    async public Task<Post?> GetPostByIdAsync(Guid id)
    {
        var posts = await _dbContext.Posts.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == id);
        return posts;
    }

    async public Task<IEnumerable<Post>> GetPostsByAuthorAsync(Guid authorId)
    {
        return await _dbContext.Posts.Include(p => p.Author).Where(p => p.AuthorId == authorId).ToListAsync();
    }

    async public Task<Post> UpdatePostAsync(Post post)
    {
        // if (await _dbContext.Posts.FindAsync(post.Id) is Post existingPost)
        // {
            // update existing post
            // https://stackoverflow.com/questions/46657813/how-to-update-record-using-entity-framework-core
        // originalPost.Title = post.Title;
        // originalPost.Content = post.Content;
        // originalPost.UpdatedDate = post.UpdatedDate;
        // var existingPost = await _dbContext.Posts.FindAsync(post.Id);
        // post.Id = existingPost.Id;
        // _dbContext.Entry(existingPost).CurrentValues.SetValues(post);
        _dbContext.Posts.Update(post);
        await _dbContext.SaveChangesAsync();
        
        return post;
        // }
        // return null;
    }

    async public Task<bool> PostExistsAsync(Guid id)
    {
        return await _dbContext.Posts.AnyAsync(p => p.Id == id);
    }
}