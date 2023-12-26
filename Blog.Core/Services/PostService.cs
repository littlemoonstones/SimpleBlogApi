using Blog.Core.IService;
using Blog.Core.IRepository;

namespace Blog.Core.Service
{
    public class PostService: IPostService
    {
        private readonly IPostRepository _postRepository;
        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<Post> CreatePostAsync(Post post)
        {
            return await _postRepository.CreatePostAsync(post);
        }
        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            return await _postRepository.GetPostByIdAsync(id);
        }
        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(Guid authorId)
        {
            return await _postRepository.GetPostsByAuthorAsync(authorId);
        }
        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _postRepository.GetAllPostsAsync();
        }
        public async Task DeletePostAsync(Guid postId, Guid userId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);
            if (post == null)
            {
                throw new ArgumentException($"Post with id {postId} doesn't exist");
            }
            if (post.AuthorId != userId)
            {
                throw new UnauthorizedAccessException($"User with id {userId} is not authorized to delete this post");
            }

            await _postRepository.DeletePostAsync(post);
        }
        public async Task<Post> UpdatePostAsync(Post post, Guid userId)
        {
            
            if (await _postRepository.PostExistsAsync(post.Id) == false)
            {
                throw new ArgumentException($"Post with id {post.Id} doesn't exist");
            }
            if (post.AuthorId != userId)
            {
                throw new UnauthorizedAccessException($"User with id {userId} is not authorized to update this post");
            }
            return await _postRepository.UpdatePostAsync(post);
        }
    }
}