using Microsoft.AspNetCore.Mvc;
using Blog.Api.Controllers;
using Blog.Core.IService;
using Blog.Core.Entities;

namespace Blog.ControllerTests;
public class AuthorControllerTests
{
    private readonly Mock<IPostService> _postServiceMock;
    private readonly Mock<ICommentService> _commentServiceMock;
    private readonly AuthorController _controller;
    private readonly Fixture _fixture = new();

    public AuthorControllerTests()
    {
        _postServiceMock = new Mock<IPostService>();
        _commentServiceMock = new Mock<ICommentService>();
        _controller = new AuthorController(_postServiceMock.Object, _commentServiceMock.Object);

        _fixture.Customize<Post>(
            p => p.Without(x => x.Author)
        );
        _fixture.Customize<Comment>(
            p => p.Without(x => x.Author)
        );
    }

    [Fact]
    public async Task GetPostsByAuthor_ReturnsOkResultWithPosts()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var expectedPosts = _fixture.Build<Post>()
            .With(x => x.AuthorId, authorId)
            .CreateMany(2)
            .ToList();
        _postServiceMock.Setup(x => x.GetPostsByAuthorAsync(authorId)).ReturnsAsync(expectedPosts);

        // Act
        var result = await _controller.GetPostsByAuthor(authorId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var posts = Assert.IsAssignableFrom<IEnumerable<PostResponseDto>>(okResult.Value);
        for(var i = 0; i < expectedPosts.Count; i++)
        {
            Assert.Equal(expectedPosts[i].Id, posts.ElementAt(i).Id);
            Assert.Equal(expectedPosts[i].Title, posts.ElementAt(i).Title);
            Assert.Equal(expectedPosts[i].Content, posts.ElementAt(i).Content);
            Assert.Equal(expectedPosts[i].CreatedDate, posts.ElementAt(i).Created);
            Assert.Equal(expectedPosts[i].UpdatedDate, posts.ElementAt(i).Updated);
            Assert.Equal(expectedPosts[i].AuthorId, posts.ElementAt(i).AuthorId);
            Assert.Equal(expectedPosts[i].Author.UserName, posts.ElementAt(i).AuthorName);
        }
    }
    [Fact]
    public async Task GetPostsByAuthor_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var expectedPosts = new List<Post>();
        _postServiceMock.Setup(x => x.GetPostsByAuthorAsync(authorId)).ReturnsAsync(expectedPosts);

        // Act
        var result = await _controller.GetPostsByAuthor(authorId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var posts = Assert.IsAssignableFrom<IEnumerable<PostResponseDto>>(okResult.Value);
        Assert.Empty(posts);
    }

    [Fact]
    public async Task GetCommentsByAuthor_ReturnsOkResultWithComments()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var expectedComments = _fixture.Build<Comment>()
            .With(x => x.Author, author)
            .CreateMany(2)
            .ToList();
        _commentServiceMock.Setup(x => x.GetCommentsByAuthorIdAsync(author.Id)).ReturnsAsync(expectedComments);

        // Act
        var result = await _controller.GetCommentsByAuthor(author.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsAssignableFrom<IEnumerable<CommentResponseDto>>(okResult.Value);
        
        for(var i = 0; i < expectedComments.Count; i++)
        {
            Assert.Equal(expectedComments[i].Id, comments.ElementAt(i).Id);
            Assert.Equal(expectedComments[i].Content, comments.ElementAt(i).Content);
            Assert.Equal(expectedComments[i].CreatedDate, comments.ElementAt(i).Created);
            Assert.Equal(expectedComments[i].AuthorId, comments.ElementAt(i).AuthorId);
            Assert.Equal(expectedComments[i].PostId, comments.ElementAt(i).PostId);
            Assert.Equal(expectedComments[i].Author.UserName, comments.ElementAt(i).Author);
        }
    }


    [Fact]
    public async Task GetCommentsByAuthor_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var expectedComments = new List<Comment>();
        _commentServiceMock.Setup(x => x.GetCommentsByAuthorIdAsync(authorId)).ReturnsAsync(expectedComments);

        // Act
        var result = await _controller.GetCommentsByAuthor(authorId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsAssignableFrom<IEnumerable<CommentResponseDto>>(okResult.Value);
        Assert.Empty(comments);
    }
}