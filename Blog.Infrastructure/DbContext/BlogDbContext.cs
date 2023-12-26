using Blog.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class BlogDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }
}
