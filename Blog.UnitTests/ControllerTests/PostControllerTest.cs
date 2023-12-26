using Microsoft.AspNetCore.Mvc;
using Blog.Api.Controllers;
using Blog.Core.IService;
using Blog.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Blog.ControllerTests;
public class PostControllerTests
{
    private readonly Fixture _fixture = new();

    public PostControllerTests()
    {
        _fixture.Customize<Post>(
            p => p.Without(x => x.Author)
        );
    }

    private PostController CreatePostControllerWithUser(Mock<IPostService> postServiceMock, ClaimsPrincipal user)
    {
        var postController = new PostController(postServiceMock.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            }
        };
        return postController;
    }

    [Fact]
    public async Task CreatePost_ValidPost_ReturnsOkResult()
    {
        // Arrange
        var postRequest = _fixture.Create<PostRequestDto>();
        var author = _fixture.Create<ApplicationUser>();
        var authorClaimsPrincipal = ClaimsPrincipalFactory.CreateUser(author);

        var postServiceMock = new Mock<IPostService>();
        var postController = CreatePostControllerWithUser(postServiceMock, authorClaimsPrincipal);

        postServiceMock.Setup(service => service.CreatePostAsync(It.IsAny<Post>()))
            .ReturnsAsync(new Post
            {
                Id = Guid.NewGuid(),
                Title = postRequest.Title,
                Content = postRequest.Content,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                AuthorId = author.Id,
                Author = author
            });

        // Act
        var result = await postController.CreatePost(postRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(postRequest.Title, returnValue.Title);
        Assert.Equal(postRequest.Content, returnValue.Content);
        Assert.Equal(author.UserName, returnValue.AuthorName);
        Assert.Equal(author.Id, returnValue.AuthorId);
    }

    [Fact]
    public async Task CreatePost_InvalidPost_ReturnsBadRequest()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        postController.ModelState.AddModelError("test", "test");

        // Act
        var result = await postController.CreatePost(It.IsAny<PostRequestDto>());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreatePost_Unauthorized_ReturnsForbid()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        postController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = ClaimsPrincipalFactory.CreateAnonymous() }
        };
        var postRequest = _fixture.Create<PostRequestDto>();

        // Act
        var result = await postController.CreatePost(postRequest);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task CreatePost_Exception_ReturnsBadRequestResult()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        var author = _fixture.Create<ApplicationUser>();
        var authorClaimsPrincipal = ClaimsPrincipalFactory.CreateUser(author);
        postController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = authorClaimsPrincipal }
        };
        var postRequest = _fixture.Create<PostRequestDto>();

        postServiceMock.Setup(service => service.CreatePostAsync(It.IsAny<Post>()))
            .ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await postController.CreatePost(postRequest);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetPostById_ValidId_ReturnsOkResult()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var post = _fixture.Build<Post>()
            .With(x => x.Author, author)
            .With(x => x.AuthorId, author.Id)
            .Create();
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        postServiceMock.Setup(service => service.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        // Act
        var result = await postController.GetPostById(post.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(post.Id, returnValue.Id);
        Assert.Equal(post.Title, returnValue.Title);
        Assert.Equal(post.Content, returnValue.Content);
        Assert.Equal(post.CreatedDate, returnValue.Created);
        Assert.Equal(post.UpdatedDate, returnValue.Updated);
        Assert.Equal(post.Author.UserName, returnValue.AuthorName);
        Assert.Equal(post.AuthorId, returnValue.AuthorId);
    }

    [Fact]
    public async Task GetPostById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        postServiceMock.Setup(service => service.GetPostByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Post)null);

        // Act
        var result = await postController.GetPostById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetPostById_Exception_ReturnsBadRequestResult()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        postServiceMock.Setup(service => service.GetPostByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await postController.GetPostById(Guid.NewGuid());

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeletePostById_ValidId_ReturnsOkResult()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postServiceMock = new Mock<IPostService>();
        var postController = CreatePostControllerWithUser(postServiceMock, ClaimsPrincipalFactory.CreateUser(author));
        postServiceMock.Setup(service => service.DeletePostAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);

        // Act
        var result = await postController.DeletePostById(It.IsAny<Guid>());

        // Assert
        var okResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePostById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postServiceMock = new Mock<IPostService>();
        var postController = CreatePostControllerWithUser(postServiceMock, ClaimsPrincipalFactory.CreateUser(author));
        postServiceMock.Setup(service => service.DeletePostAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new ArgumentException("Post not found"));

        // Act
        var result = await postController.DeletePostById(It.IsAny<Guid>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeletePostById_Unauthorized_ReturnsForbid()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var user = ClaimsPrincipalFactory.CreateAnonymous();
        var postController = CreatePostControllerWithUser(postServiceMock, user);

        // Act
        var result = await postController.DeletePostById(It.IsAny<Guid>());

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeletePostById_Exception_ReturnsBadRequestResult()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        postServiceMock.Setup(service => service.DeletePostAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await postController.DeletePostById(It.IsAny<Guid>());

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdatePost_ValidPost_ReturnsOkResult()
    {
        // Arrange
        var postRequest = _fixture.Create<PostRequestDto>();
        var author = _fixture.Create<ApplicationUser>();
        var authorClaimsPrincipal = ClaimsPrincipalFactory.CreateUser(author);

        var postServiceMock = new Mock<IPostService>();
        var postController = CreatePostControllerWithUser(postServiceMock, authorClaimsPrincipal);

        postServiceMock.Setup(service => service.UpdatePostAsync(It.IsAny<Post>(), It.IsAny<Guid>()))
            .ReturnsAsync(new Post
            {
                Id = Guid.NewGuid(),
                Title = postRequest.Title,
                Content = postRequest.Content,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                AuthorId = author.Id,
                Author = author
            });

        // Act
        var result = await postController.UpdatePost(Guid.NewGuid(), postRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(postRequest.Title, returnValue.Title);
        Assert.Equal(postRequest.Content, returnValue.Content);
        Assert.Equal(author.UserName, returnValue.AuthorName);
        Assert.Equal(author.Id, returnValue.AuthorId);
    }

    [Fact]
    public async Task UpdatePost_InvalidPost_ReturnsBadRequest()
    {
        // Arrange
        var author = _fixture.Create<ApplicationUser>();
        var postServiceMock = new Mock<IPostService>();
        var postController = CreatePostControllerWithUser(postServiceMock, ClaimsPrincipalFactory.CreateUser(author));
        postController.ModelState.AddModelError("test", "test");

        // Act
        var result = await postController.UpdatePost(It.IsAny<Guid>(), It.IsAny<PostRequestDto>());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdatePost_Unauthorized_ReturnsForbid()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var user = ClaimsPrincipalFactory.CreateAnonymous();
        var postController = CreatePostControllerWithUser(postServiceMock, user);
        var postRequest = _fixture.Create<PostRequestDto>();

        // Act
        var result = await postController.UpdatePost(It.IsAny<Guid>(), postRequest);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task UpdatePost_Exception_ReturnsBadRequestResult()
    {
        // Arrange
        var postServiceMock = new Mock<IPostService>();
        var postController = new PostController(postServiceMock.Object);
        var author = _fixture.Create<ApplicationUser>();
        var authorClaimsPrincipal = ClaimsPrincipalFactory.CreateUser(author);
        postController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = authorClaimsPrincipal }
        };
        var postRequest = _fixture.Create<PostRequestDto>();

        postServiceMock.Setup(service => service.UpdatePostAsync(It.IsAny<Post>(), It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await postController.UpdatePost(It.IsAny<Guid>(), postRequest);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}