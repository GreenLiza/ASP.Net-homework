using GoodNewsAggregator.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Abstractions.Services
{
    public interface ISourceService
    {
        Task AddDefaultSource();
        Task<List<SourceDto>> GetSourcesAsync();
        Task<int> GetSourceId(string sourceName);
        Task<SourceDto> GetSourceByRssLinkAsync(string RssLink);
        Task<List<string>> GetSourcesRssLinksAsync();
    }
}
