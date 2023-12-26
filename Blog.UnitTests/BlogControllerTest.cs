// using System.Security.Claims;
// using Blog.WebApi.Controllers;
// using Blog.Core.Dtos;
// using Blog.Core.Entities;
// using Blog.Infrastructure.Repositories;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Xunit;

// namespace Blog.WebApi.Tests
// {
//     public class BlogControllerTests
//     {
//         private readonly Mock<IPostRepository> _postRepositoryMock;
//         private readonly Mock<ICommentRepository> _commentRepositoryMock;
//         private readonly BlogController _blogController;

//         public BlogControllerTests()
//         {
//             _postRepositoryMock = new Mock<IPostRepository>();
//             _commentRepositoryMock = new Mock<ICommentRepository>();
//             _blogController = new BlogController(_postRepositoryMock.Object, _commentRepositoryMock.Object);
//         }

//         [Fact]
//         public async Task CreatePost_ReturnsOkResultWithPostResponseDto()
//         {
//             // Arrange
//             var postRequestDto = new PostRequestDto
//             {
//                 Title = "Test Post",
//                 Content = "This is a test post",
//                 AuthorId = Guid.NewGuid()
//             };
//             var postEntity = new Post
//             {
//                 Id = Guid.NewGuid(),
//                 Title = postRequestDto.Title,
//                 Content = postRequestDto.Content,
//                 CreatedDate = DateTime.Now,
//                 UpdatedDate = DateTime.Now,
//                 AuthorId = postRequestDto.AuthorId
//             };
//             _postRepositoryMock.Setup(repo => repo.CreatePostAsync(It.IsAny<Post>())).ReturnsAsync(postEntity);

//             // Act
//             var result = await _blogController.CreatePost(postRequestDto);

//             // Assert
//             var okResult = Assert.IsType<OkObjectResult>(result);
//             var actualPostResponseDto = Assert.IsType<PostResponseDto>(okResult.Value);
//             Assert.Equal(postEntity.Id, actualPostResponseDto.Id);
//             Assert.Equal(postEntity.Title, actualPostResponseDto.Title);
//             Assert.Equal(postEntity.Content, actualPostResponseDto.Content);
//             Assert.Equal(postEntity.CreatedDate, actualPostResponseDto.Created);
//             Assert.Equal(postEntity.UpdatedDate, actualPostResponseDto.Updated);
//             Assert.Equal(postEntity.Author.UserName, actualPostResponseDto.Author);
//             Assert.Equal(postEntity.AuthorId, actualPostResponseDto.AuthorId);
//         }

//         // Add more test methods for other controller methods...

//     }
// }
