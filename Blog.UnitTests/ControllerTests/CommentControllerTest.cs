using Microsoft.AspNetCore.Mvc;
using Blog.Api.Controllers;
using Blog.Core.IService;
using Blog.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Blog.ControllerTests;
public class CommentControllerTests
{
    private readonly Fixture _fixture = new();

    public CommentControllerTests()
    {
        _fixture.Customize<Post>(
            p => p.Without(x => x.Author)
        );
        _fixture.Customize<Comment>(
            p => p.Without(x => x.Author)
        );
    }

    private CommentController CreateCommentControllerWithUser(Mock<ICommentService> commentServiceMock, ClaimsPrincipal user)
    {
        var commentController = new CommentController(commentServiceMock.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            }
        };
        return commentController;
    }

    [Fact]
    public async Task GetCommentsByPost_PostExists_ReturnsOkResultWithComments()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var post = _fixture.Build<Post>()
            .With(x => x.Author, author)
            .Create();
        var expectedComments = _fixture.Build<Comment>()
            .With(x => x.Author, author)
            .With(x => x.Post, post)
            .CreateMany(2)
            .ToList();
        var commentServiceMock = new Mock<ICommentService>();
        var controller = new CommentController(commentServiceMock.Object);
        commentServiceMock.Setup(x => x.GetCommentsByPostIdAsync(post.Id)).ReturnsAsync(expectedComments);

        // Act
        var result = await controller.GetCommentsByPostId(post.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsAssignableFrom<IEnumerable<CommentResponseDto>>(okResult.Value);
        for(var i = 0; i < expectedComments.Count; i++)
        {
            Assert.Equal(expectedComments[i].Id, comments.ElementAt(i).Id);
            Assert.Equal(expectedComments[i].Content, comments.ElementAt(i).Content);
            Assert.Equal(expectedComments[i].CreatedDate, comments.ElementAt(i).Created);
            Assert.Equal(expectedComments[i].AuthorId, comments.ElementAt(i).AuthorId);
            Assert.Equal(expectedComments[i].Author.UserName, comments.ElementAt(i).Author);
            Assert.Equal(expectedComments[i].PostId, comments.ElementAt(i).PostId);
        }
    }

    [Fact]
    public async Task GetCommentsByPost_PostDoesNotExist_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var commentServiceMock = new Mock<ICommentService>();
        var controller = new CommentController(commentServiceMock.Object);
        commentServiceMock.Setup(x => x.GetCommentsByPostIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<Comment>());

        // Act
        var result = await controller.GetCommentsByPostId(postId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsAssignableFrom<IEnumerable<CommentResponseDto>>(okResult.Value);
    }

    [Fact]
    public async Task CreateComment_PostExists_ReturnsOkResultWithComment()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var post = _fixture.Build<Post>()
            .With(x => x.Author, author)
            .Create();
        var comment = _fixture.Build<Comment>()
            .With(x => x.Author, author)
            .With(x => x.Post, post)
            .Create();
        var commentRequest = new CommentRequestDto
        {
            Content = comment.Content
        };
        
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>())).ReturnsAsync(comment);
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateUser(author));

        // Act
        var result = await commentController.CreateComment(post.Id, commentRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var commentResponse = Assert.IsAssignableFrom<CommentResponseDto>(okResult.Value);
        Assert.Equal(comment.Id, commentResponse.Id);
        Assert.Equal(comment.Content, commentResponse.Content);
        Assert.Equal(comment.CreatedDate, commentResponse.Created);
        Assert.Equal(comment.AuthorId, commentResponse.AuthorId);
        Assert.Equal(comment.Author.UserName, commentResponse.Author);
        Assert.Equal(comment.PostId, commentResponse.PostId);
    }

    [Fact]
    public async Task CreateComment_PostDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postId = Guid.NewGuid();
        var commentRequest = new CommentRequestDto
        {
            Content = "test"
        };
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>())).ThrowsAsync(new ArgumentException());
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateUser(author));

        // Act
        var result = await commentController.CreateComment(postId, commentRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateComment_InvalidComment_ReturnsBadRequestResult()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postId = Guid.NewGuid();
        var commentRequest = new CommentRequestDto
        {
            Content = null
        };
        var commentServiceMock = new Mock<ICommentService>();
        var commentController = new CommentController(commentServiceMock.Object);
        commentController.ModelState.AddModelError("test", "test");

        // Act
        var result = await commentController.CreateComment(postId, commentRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateComment_UserIsNotAuthenticated_ReturnsUnauthorizedResult()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var commentRequest = _fixture.Create<CommentRequestDto>();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>())).ThrowsAsync(new UnauthorizedAccessException());
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateAnonymous());

        // Act
        var result = await commentController.CreateComment(postId, commentRequest);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task CreateComment_Exception_ReturnsBadRequestResult()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postId = Guid.NewGuid();
        var commentRequest = _fixture.Create<CommentRequestDto>();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>())).ThrowsAsync(new Exception());
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateUser(author));

        // Act
        var result = await commentController.CreateComment(postId, commentRequest);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteComment_CommentExists_ReturnsOkResultWithComment()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var post = _fixture.Build<Post>()
            .With(x => x.Author, author)
            .Create();
        var comment = _fixture.Build<Comment>()
            .With(x => x.Author, author)
            .With(x => x.Post, post)
            .Create();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.DeleteCommentAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateUser(author));

        // Act
        var result = await commentController.DeleteComment(post.Id, comment.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteComment_CommentDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.DeleteCommentAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new ArgumentException());
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateUser(author));

        // Act
        var result = await commentController.DeleteComment(postId, commentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteComment_UserIsNotAuthenticated_ReturnsUnauthorizedResult()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.DeleteCommentAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedAccessException());
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateAnonymous());

        // Act
        var result = await commentController.DeleteComment(postId, commentId);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task DeleteComment_Exception_ReturnsBadRequestResult()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postId = Guid.NewGuid();
        var commentId = Guid.NewGuid();
        var commentServiceMock = new Mock<ICommentService>();
        commentServiceMock.Setup(x => x.DeleteCommentAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new Exception());
        var commentController = CreateCommentControllerWithUser(commentServiceMock, ClaimsPrincipalFactory.CreateUser(author));

        // Act
        var result = await commentController.DeleteComment(postId, commentId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
    
}