using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DTO
{
    public class BasicNewsDTO
    {
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public double Rate { get; set; }
        public string SourceName { get; set; }
    }
}
