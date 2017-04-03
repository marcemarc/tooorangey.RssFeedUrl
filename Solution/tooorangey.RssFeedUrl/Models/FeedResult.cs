using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace tooorangey.RssFeedUrl.Models
{
    public class FeedResult
    {
        public SyndicationFeed SyndicationFeed { get; set; }
        public string FeedUrl { get; set; }
        public string StatusMessage { get; set; }
        public bool HasFeedResults { get; set; }
  
    }
}
