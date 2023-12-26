using Blog.Core.IRepository;
using Blog.Core.IService;
namespace Blog.Core.Service
{
    public class CommentService: ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        public CommentService(
            ICommentRepository commentRepository,
            IPostRepository postRepository
        )
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            if (!await _postRepository.PostExistsAsync(comment.PostId))
            {
                throw new ArgumentException($"Post with id {comment.PostId} doesn't exist");
            }
            return await _commentRepository.CreateCommentAsync(comment);
        }
        public async Task<Comment?> GetCommentByIdAsync(Guid id)
        {
            return await _commentRepository.GetCommentByIdAsync(id);
        }
        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId)
        {
            return await _commentRepository.GetCommentsByPostAsync(postId);
        }
        public async Task<IEnumerable<Comment>> GetCommentsByAuthorIdAsync(Guid authorId)
        {
            return await _commentRepository.GetCommentsByAuthorAsync(authorId);
        }
        public async Task DeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                throw new ArgumentException($"Comment with id {commentId} doesn't exist");
            }
            if (comment.AuthorId != userId)
            {
                throw new UnauthorizedAccessException($"User with id {userId} is not authorized to delete this comment");
            }
            await _commentRepository.DeleteCommentAsync(comment);
        }
        
        
    }
}