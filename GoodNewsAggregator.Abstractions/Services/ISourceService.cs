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
        Task<List<SourceDto>> GetSourcesAsync();
        int GetSourceId(string sourceName);
    }
}
