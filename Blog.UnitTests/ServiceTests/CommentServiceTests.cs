using Blog.Core.IRepository;
using Blog.Core.IService;
using Blog.Core.Service;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly ICommentService _commentService;
    private readonly Fixture _fixture;

    public CommentServiceTests()
    {
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _commentService = new CommentService(_commentRepositoryMock.Object, _postRepositoryMock.Object);
        
        _fixture = new Fixture();
        _fixture.Customize<Post>(p => p
            .Without(x => x.Author));
        _fixture.Customize<Comment>(p => p
            .Without(x => x.Author)
            .Without(x => x.Post));
    }

    [Fact]
    public async Task CreateCommentAsync_PostExists_ShouldReturnComment()
    {
        // Arrange
        var expectedComment = _fixture.Create<Comment>();
        _postRepositoryMock.Setup(x => x.PostExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _commentRepositoryMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>()))
            .ReturnsAsync(expectedComment);
        
        // Act
        var result = await _commentService.CreateCommentAsync(expectedComment);
        
        // Assert
        Assert.Equal(expectedComment, result);
    }

    [Fact]
    public async Task CreateCommentAsync_PostDoesntExist_ShouldThrowException()
    {
        // Arrange
        var expectedComment = _fixture.Create<Comment>();
        _postRepositoryMock.Setup(x => x.PostExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);
        
        // Act
        async Task act() => await _commentService.CreateCommentAsync(expectedComment);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task GetCommentByIdAsync_CommentExist_ShouldReturnComment()
    {
        // Arrange
        var expectedComment = _fixture.Create<Comment>();
        _commentRepositoryMock.Setup(x => x.GetCommentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedComment);
        
        // Act
        var result = await _commentService.GetCommentByIdAsync(expectedComment.Id);
        
        // Assert
        Assert.Equal(expectedComment, result);
    }

    [Fact]
    public async Task GetCommentByIdAsync_CommentDoesntExist_ShouldReturnNull()
    {
        // Arrange
        _commentRepositoryMock.Setup(x => x.GetCommentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Comment?)null);
        
        // Act
        var result = await _commentService.GetCommentByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteCommentAsync_CommentExists_DeletesComment()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var commentToDelete = comments.First();

        _commentRepositoryMock.Setup(x => x.GetCommentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(commentToDelete);
        
        // Act
        await _commentService.DeleteCommentAsync(commentToDelete.Id, commentToDelete.AuthorId);

        // Assert
        _commentRepositoryMock.Verify(x => x.DeleteCommentAsync(commentToDelete), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_CommentDoesntExist_ShouldThrowException()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var commentToDelete = _fixture.Create<Comment>();

        _commentRepositoryMock.Setup(x => x.GetCommentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Comment?)null);
        
        // Act
        async Task act() => await _commentService.DeleteCommentAsync(commentToDelete.Id, commentToDelete.AuthorId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task DeleteCommentAsync_UserIsNotAuthor_ShouldThrowException()
    {
        // Arrange
        var comments = _fixture.CreateMany<Comment>(10).ToList();
        var commentToDelete = comments.First();

        _commentRepositoryMock.Setup(x => x.GetCommentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(commentToDelete);
        
        // Act
        async Task act() => await _commentService.DeleteCommentAsync(commentToDelete.Id, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
    }
}