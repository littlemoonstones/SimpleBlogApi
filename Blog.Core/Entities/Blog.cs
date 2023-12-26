using Blog.Core.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    
    // Foreign key relationship to User (Author)
    public Guid AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;

    public override string ToString()
    {
        return $"Id: {Id},\n"
               + $"Title: {Title},\n" 
               + $"Content: {Content},\n" 
               + $"CreatedDate: {CreatedDate},\n" 
               + $"UpdatedDate: {UpdatedDate},\n" 
               + $"AuthorId: {AuthorId}";
    }
    
}

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    
    // Foreign key relationship to User (Author)
    public Guid AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;
    
    // Foreign key relationship to Post
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;

    public override string ToString()
    {
        return $"Id: {Id}, Content: {Content}, CreatedDate: {CreatedDate}, AuthorId: {AuthorId}, PostId: {PostId}";
    }
}