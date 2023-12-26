using System.ComponentModel.DataAnnotations;

public class PostRequestDto
{
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Content { get; set; } = null!;
}

public class PostResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    // public List<CommentResponseDto> Comments { get; set; } = null!;
}

public class CommentRequestDto
{
    [Required]
    public string Content { get; set; } = null!;
}
public class CommentResponseDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public string Author { get; set; } = null!;
    public DateTime Created { get; set; }
    public Guid PostId { get; set; }
}