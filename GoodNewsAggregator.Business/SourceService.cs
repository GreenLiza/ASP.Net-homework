using AutoMapper;
using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Business
{
    public class SourceService : ISourceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SourceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddDefaultSource()
        {
            var source = new Source()
            {
                Name = "Good News",
                Link = ""
            };
            await _unitOfWork.Sources.AddAsync(source);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetSourceId(string sourceName)
        {
            var sourceId =await _unitOfWork.Sources.FindBy(source => source.Name.Equals(sourceName))
                .Select(source => source.Id)
                .FirstOrDefaultAsync();

            return sourceId;
        }

        public async Task<List<SourceDto>> GetSourcesAsync()
        {
            var sourceDtos = await _unitOfWork.Sources.GetAsQueryable()
                .Select(source => _mapper.Map<SourceDto>(source))
                .ToListAsync();
            return sourceDtos;
        }

        public async Task<SourceDto> GetSourceByRssLinkAsync(string RssLink)
        {
            var sourceDto = await _unitOfWork.Sources.FindBy(source => source.RssLink.Equals(RssLink))
                .Select(source => _mapper.Map<SourceDto>(source))
                .FirstOrDefaultAsync();
            return sourceDto;
        }

        public async Task<List<string>> GetSourcesRssLinksAsync()
        {
            var sourcesRssLinks = await _unitOfWork.Sources.GetAsQueryable()
                .Where(source => (source.RssLink != null && source.RssLink != ""))
                .Select(source => source.RssLink)
                .ToListAsync();
            return sourcesRssLinks;
        }
    }
}
