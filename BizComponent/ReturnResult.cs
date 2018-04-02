using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizComponent
{
    public class ReturnResult
    {
        public ReturnResult()
        {
            this.Links = new List<Link>();
            this.WordsOccurences = new List<WordOccurence>();
            this.MetaOccurences = new List<WordOccurence>();
        }

        public string TextSearch { get; set; }
        public bool FilterStopWords { get; set; }
        public bool ShowWordOccurence { get; set; }
        public bool ShowMetaOccurence { get; set; }
        public bool ShowLinks { get; set; }

        public List<Link> Links { get; set; }
        public List<WordOccurence> WordsOccurences { get; set; }
        public List<WordOccurence> MetaOccurences { get; set; }

        //success/failed
        public bool Result;
        public string ErrorMessage { get; set; }
}
}
