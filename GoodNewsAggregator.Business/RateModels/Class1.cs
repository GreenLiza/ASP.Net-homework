using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Business.RateModels
{
    public class Annotations
    {
        public List<Lemma> Lemma { get; set; }
    }

    public class Lemma
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Value { get; set; }
    }

    public class Root
    {
        public string Text { get; set; }
        public Annotations Annotations { get; set; }
    }
}
