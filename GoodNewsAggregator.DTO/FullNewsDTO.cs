using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DTO
{
    public class FullNewsDTO : BasicNewsDTO
    {
        public string ShortDescription { get; set; }
        public string FullText { get; set; }
        public int SourceId { get; set; }

    }
}
