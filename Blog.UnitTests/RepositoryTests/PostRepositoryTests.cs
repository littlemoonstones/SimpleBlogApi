namespace Blog.UnitTests;
public class PostRepositoryTests
{
    private readonly DbContextMock<BlogDbContext> _dbContextMock;
    private readonly PostRepository _postRepository;
    private readonly Fixture _fixture;
    public PostRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Customize<Post>(p => p
            .Without(x => x.Author));
        _dbContextMock = CreateDbContextMock();
        _postRepository = new PostRepository(_dbContextMock.Object);
    }
    private DbContextMock<BlogDbContext> CreateDbContextMock()
    {
        return new DbContextMock<BlogDbContext>(
            new DbContextOptionsBuilder<BlogDbContext>().Options
        );
    }

    [Fact]
    public async Task GetPostByIdAsync_PostExists_ReturnsPost()
    {
        // Arrange
        var expectedPost = _fixture.Create<Post>();
        var posts = new List<Post>{
            expectedPost
        };

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);
        var postId = expectedPost.Id;

        // Act
        var result = await _postRepository.GetPostByIdAsync(postId);

        // Assert
        Assert.Equivalent(expectedPost, result);
    }

    [Fact]
    public async Task GetPostByIdAsync_PostDoesntExist_ReturnsNull()
    {
        // Arrange
        var posts = new List<Post>();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);
        var postId = Guid.NewGuid();

        // Act
        var result = await _postRepository.GetPostByIdAsync(postId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllPostsAsync_PostsExist_ReturnsPosts()
    {
        // Arrange
        var expectedPosts = _fixture.CreateMany<Post>(10).ToList();
        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, expectedPosts);

        // Act
        var result = await _postRepository.GetAllPostsAsync();

        // Assert
        Assert.Equivalent(expectedPosts, result);
    }


    [Fact]
    // add a post
    public async Task AddPostAsync_PostDoesntExist_AddsPost()
    {
        // Arrange
        var newPost = _fixture.Create<Post>();
        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, new List<Post>());

        // Act
        await _postRepository.CreatePostAsync(newPost);

        // Assert
        Assert.Contains(newPost, _dbContextMock.Object.Posts);
    }

    [Fact]
    // delete a post if it exists
    public async Task DeletePostAsync_PostExists_DeletesPost()
    {
        // Arrange
        var posts = _fixture.CreateMany<Post>(10).ToList();
        var deletedPost = _fixture.Create<Post>();

        posts.Add(deletedPost);

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);

        // Act
        await _postRepository.DeletePostAsync(deletedPost);

        // Assert
        Assert.DoesNotContain(deletedPost, _dbContextMock.Object.Posts);
        // _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }
    [Fact]
    // delete a post if it not exists
    public async Task DeletePostAsync_PostDoesntExist_DoesntDeletePost()
    {
        // Arrange
        var postToDelete = _fixture.Create<Post>();
        var posts = new List<Post>(
            _fixture.CreateMany<Post>(10)
        );

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);

        // Act
        await _postRepository.DeletePostAsync(postToDelete);

        // Assert
        Assert.DoesNotContain(postToDelete, _dbContextMock.Object.Posts);
        // _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    // update a post if it exists
    public async Task UpdatePostAsync_PostExists_UpdatesPost()
    {
        // Arrange
        var originalPost = _fixture.Create<Post>();
        var postToUpdate = _fixture.Build<Post>()
            .With(p => p.Id, originalPost.Id)
            .Create();

        var posts = new List<Post>{
            originalPost
        };

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);

        // Act
        var result = await _postRepository.UpdatePostAsync(postToUpdate);

        // Assert
        Assert.Equal(postToUpdate, result);
    }


    [Fact]
    // get posts by author
    public async Task GetPostsByAuthorAsync_PostsExist_ReturnsPosts()
    {
        // Arrange
        var posts = _fixture.CreateMany<Post>(5).ToList();
        var authorId = Guid.NewGuid();

        var expectedPosts = _fixture.Build<Post>()
            .With(p => p.AuthorId, authorId)
            .CreateMany(5).ToList();

        posts.AddRange(expectedPosts);

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, expectedPosts);

        // Act
        var result = await _postRepository.GetPostsByAuthorAsync(authorId);

        // Assert
        Assert.Equivalent(expectedPosts, result);
    }

    [Fact]
    // // get posts by author, but no posts exist
    public async Task GetPostsByAuthorAsync_PostsDontExist_ReturnsEmptyList()
    {
        // Arrange
        var posts = _fixture.CreateMany<Post>(10).ToList();
        var authorId = Guid.NewGuid();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);

        // Act
        var result = await _postRepository.GetPostsByAuthorAsync(authorId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    // // get posts by author, but the author doesn't exist
    public async Task GetPostsByAuthorAsync_AuthorDoesntExist_ReturnsEmptyList()
    {
        // Arrange
        var posts = _fixture.CreateMany<Post>(10).ToList();
        var authorId = Guid.NewGuid();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);

        // Act
        var result = await _postRepository.GetPostsByAuthorAsync(authorId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    // post exists
    public async Task PostExistsAsync_PostExists_ReturnsTrue()
    {
        // Arrange
        var posts = _fixture.CreateMany<Post>(10).ToList();
        var postToFind = posts.First();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);

        // Act
        var result = await _postRepository.PostExistsAsync(postToFind.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    // post doesn't exist
    public async Task PostExistsAsync_PostDoesntExist_ReturnsFalse()
    {
        // Arrange
        var posts = _fixture.CreateMany<Post>(10).ToList();
        var postToFind = _fixture.Create<Post>();

        _dbContextMock.CreateDbSetMock(tmp => tmp.Posts, posts);

        // Act
        var result = await _postRepository.PostExistsAsync(postToFind.Id);

        // Assert
        Assert.False(result);
    }
}