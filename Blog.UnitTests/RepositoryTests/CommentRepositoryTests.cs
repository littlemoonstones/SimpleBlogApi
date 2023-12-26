namespace Blog.UnitTests;
public class CommentRepositoryTests
{
    private readonly DbContextMock<BlogDbContext> _dbContextMock;
    private readonly CommentRepository _commentRepository;
    private readonly Fixture _fixture;
    public CommentRepositoryTests()
    {
        _fixture = new Fixture();

        _fixture.Customize<Comment>(p => p
            .Without(x => x.Author)
            .Without(x => x.Post));

        _dbContextMock = CreateDbContextMock();
        _commentRepository = new CommentRepository(_dbContextMock.Object);
    }
    private DbContextMock<BlogDbContext> CreateDbContextMock()
    {
        return new DbContextMock<BlogDbContext>(
            new DbContextOptionsBuilder<BlogDbContext>().Options
        );
    }

    [Fact]
    public async Task GetCommentByIdAsync_CommentExists_ReturnsComment()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();

        var expectedComment = comments.First();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);
        var commentId = expectedComment.Id;

        // Act
        var result = await _commentRepository.GetCommentByIdAsync(commentId);

        // Assert
        Assert.Equivalent(expectedComment, result);
    }

    [Fact]
    public async Task GetCommentByIdAsync_CommentDoesntExist_ReturnsNull()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);
        var commentId = Guid.NewGuid();

        // Act
        var result = await _commentRepository.GetCommentByIdAsync(commentId);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task GetCommentsByPostAsync_CommentsExist_ReturnsComments()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var expectedComments = comments.Where(c => c.PostId == comments.First().PostId);
        var postId = comments.First().PostId;

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);

        // Act
        var result = await _commentRepository.GetCommentsByPostAsync(postId);

        // Assert
        Assert.Equivalent(expectedComments, result);
    }

    [Fact]
    public async Task GetCommentsByPostAsync_CommentsDontExist_ReturnsEmptyList()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var postId = Guid.NewGuid();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);

        // Act
        var result = await _commentRepository.GetCommentsByPostAsync(postId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateCommentAsync_AddsComment()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var expectedComment = _fixture.Create<Comment>();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);

        // Act
        var result = await _commentRepository.CreateCommentAsync(expectedComment);

        // Assert
        Assert.Contains(expectedComment, _dbContextMock.Object.Comments);
    }

    [Fact]
    public async Task DeleteCommentAsync_CommentDoesExist_DeletesComment()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var expectedComment = comments.First();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);

        // Act
        await _commentRepository.DeleteCommentAsync(expectedComment);

        // Assert
        Assert.DoesNotContain(expectedComment, _dbContextMock.Object.Comments);
    }

    [Fact]
    public async Task CommentExistsAsync_CommentDoesExist_ReturnsTrue()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var expectedComment = comments.First();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);

        // Act
        var result = await _commentRepository.CommentExistsAsync(expectedComment.Id);

        // Assert
        Assert.True(result);
    }
    [Fact]
    public async Task CommentExistsAsync_CommentDoesNotExist_ReturnsTrue()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var expectedComment = comments.First();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Comments, comments);

        // Act
        var result = await _commentRepository.CommentExistsAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}