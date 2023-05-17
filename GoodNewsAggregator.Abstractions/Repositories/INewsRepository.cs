using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Abstractions.Repositories
{
    public interface INewsRepository : IRepository<News>
    {
        Task<News> GetNewsByTitleAsync(string title);
        public Task<List<News>> GetNewsByPageAsync(int page, int pageSize);
        Task<List<News>> GetNewsListAsync();
        Task EditNewsArticleAsync(News news);

    }
}
