using Blog.Core.IRepository;
using Microsoft.EntityFrameworkCore;
namespace Blog.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly BlogDbContext _dbContext;
    public CommentRepository(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    async public Task<Comment> CreateCommentAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        return comment;
    }

    async public Task<Comment?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _dbContext.Comments.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == id);
        return comment;
    }

    async public Task DeleteCommentAsync(Comment comment)
    {
        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }

    async public Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid postId)
    {
        return await _dbContext.Comments.Include(c => c.Author).Where(c => c.PostId == postId).ToListAsync();
    }

    async public Task<IEnumerable<Comment>> GetCommentsByAuthorAsync(Guid authorId)
    {
        return await _dbContext.Comments.Include(c => c.Author).Where(c => c.AuthorId == authorId).ToListAsync();
    }

    async public Task<bool> CommentExistsAsync(Guid id)
    {
        return await _dbContext.Comments.AnyAsync(c => c.Id == id);
    }
}