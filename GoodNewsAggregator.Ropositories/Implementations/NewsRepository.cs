using GoodNewsAggregator.Abstractions.Repositories;
using GoodNewsAggregator.Data;
using GoodNewsAggregator.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.Repositories.Implementations
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(NewsAggregatorContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<News>> GetNewsByPageAsync(int page, int pageSize)
        {
            List<News> news = await DbSet.Include(x => x.Source).Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();
            return news;
        }

        public async Task<News> GetNewsByTitleAsync(string title)
        {
            var news = await DbSet.Include(x => x.Source)
                .FirstOrDefaultAsync(news => news.Title == title);
            return news;
        }

        public async Task<List<News>> GetNewsListAsync()
        {
            List<News> news = await DbSet.Include(x => x.Source).ToListAsync();
            return news;
        }

        public async Task EditNewsArticleAsync(News news)
        {
            var entity = await DbSet.FirstOrDefaultAsync(entity => entity.Id == news.Id);
            entity.Title = news.Title;
            entity.ShortDescription = news.ShortDescription;
            entity.FullText = news.FullText;
            DbSet.Update(entity);
        }
    }
}
