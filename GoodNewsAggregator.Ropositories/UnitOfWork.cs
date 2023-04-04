using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Abstractions.Repositories;
using GoodNewsAggregator.Data;
using GoodNewsAggregator.Data.Entities;

namespace GoodNewsAggregator.Ropositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NewsAggregatorContext _dbContext;
        private readonly IRepository<Comment> _commentRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IRepository<RatingWord> _ratingWordsRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Source> _sourceRepository;
        private readonly IUserRepository _userRepository;

        public UnitOfWork(NewsAggregatorContext dbContext,
            IRepository<Comment> commentRepository,
            INewsRepository newsRepository,
            IRepository<RatingWord> ratingWordsRepository,
            IRepository<Role> roleRepository,
            IRepository<Source> sourceRepository,
            IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _commentRepository = commentRepository;
            _newsRepository = newsRepository;
            _ratingWordsRepository = ratingWordsRepository;
            _roleRepository = roleRepository;
            _sourceRepository = sourceRepository;
            _userRepository = userRepository;
        }

        public IRepository<Comment> Comments => _commentRepository;
        public INewsRepository News => _newsRepository;
        public IRepository<RatingWord> RatingWords => _ratingWordsRepository;
        public IRepository<Role> Roles => _roleRepository;
        public IRepository<Source> Sources => _sourceRepository;
        public IUserRepository Users => _userRepository;

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

             
        public void Dispose()
        {
            _dbContext.Dispose();
            _newsRepository?.Dispose();
            _userRepository?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    

    
}