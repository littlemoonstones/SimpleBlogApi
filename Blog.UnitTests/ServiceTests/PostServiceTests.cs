using Blog.Core.IRepository;
using Blog.Core.IService;
using Blog.Core.Service;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly IPostService _postService;
    private readonly Fixture _fixture;
    public PostServiceTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _postService = new PostService(_postRepositoryMock.Object);
        _fixture = new Fixture();
        _fixture.Customize<Post>(
            p => p.Without(x => x.Author)
        );
    }
    [Fact]
    public async Task CreatePostAsync_ShouldReturnPost()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.CreatePostAsync(It.IsAny<Post>()))
            .ReturnsAsync(expectedPost);
        
        // Act
        var result = await _postService.CreatePostAsync(expectedPost);

        // Assert
        Assert.Equal(expectedPost, result);

    }
    [Fact]
    public async Task GetPostByIdAsync_ShouldReturnPost()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.GetPostByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedPost);
        
        // Act
        var result = await _postService.GetPostByIdAsync(expectedPost.Id);

        // Assert
        Assert.Equal(expectedPost, result);
    }
    [Fact]
    public async Task UpdatePostAsync_PostExist_ShouldReturnPost()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.PostExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _postRepositoryMock.Setup(x => x.UpdatePostAsync(It.IsAny<Post>()))
            .ReturnsAsync(expectedPost);
            
        
        // Act
        var result = await _postService.UpdatePostAsync(expectedPost, expectedPost.AuthorId);

        // Assert
        Assert.Equal(expectedPost, result);
    }

    [Fact]
    public async Task UpdatePostAsync_PostDoesntExist_ShouldThrowException()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.PostExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);
        
        // Act
        async Task act() => await _postService.UpdatePostAsync(expectedPost, expectedPost.AuthorId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task UpdatePostAsync_UserIsNotAuthor_ShouldThrowException()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.PostExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        
        _postRepositoryMock.Setup(x => x.UpdatePostAsync(It.IsAny<Post>()))
            .ReturnsAsync(expectedPost);
        
        // Act
        async Task act() => await _postService.UpdatePostAsync(expectedPost, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
    }
    
    [Fact]
    public async Task DeletePostAsync_PostExist_ShouldDeletePost()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.GetPostByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedPost);
        
        // Act
        await _postService.DeletePostAsync(expectedPost.Id, expectedPost.AuthorId);

        // Assert
        _postRepositoryMock.Verify(x => x.DeletePostAsync(expectedPost), Times.Once);
    }

    [Fact]
    public async Task DeletePostAsync_PostDoesntExist_ShouldThrowException()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.GetPostByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Post?)null);
        
        // Act
        async Task act() => await _postService.DeletePostAsync(expectedPost.Id, expectedPost.AuthorId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task DeletePostAsync_UserIsNotAuthor_ShouldThrowException()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        _postRepositoryMock.Setup(x => x.GetPostByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedPost);
        
        // Act
        async Task act() => await _postService.DeletePostAsync(expectedPost.Id, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
    }

}