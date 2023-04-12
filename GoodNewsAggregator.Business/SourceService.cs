using AutoMapper;
using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Abstractions.Services;
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
        public int GetSourceId(string sourceName)
        {
            var sourceId = _unitOfWork.Sources.FindBy(source => source.Name.Equals(sourceName))
                .Select(source => source.Id)
                .FirstOrDefault();

            return sourceId;
        }

        public async Task<List<SourceDto>> GetSourcesAsync()
        {
            var sourceDtos = await _unitOfWork.Sources.GetAsQueryable()
                .Select(source => _mapper.Map<SourceDto>(source))
                .ToListAsync();
            return sourceDtos;
        }
    }
}
