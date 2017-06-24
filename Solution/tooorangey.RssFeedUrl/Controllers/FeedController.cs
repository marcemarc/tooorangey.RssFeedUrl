using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using tooorangey.RssFeedUrl.Models;
using Umbraco.Core.Logging;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace tooorangey.RssFeedUrl.Controllers
{

    public class FeedController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public FeedResult GetRssFeed(string feedUrl)
        {
            var feedResult = new FeedResult() { HasFeedResults = false, FeedUrl = feedUrl };
            var feedContent = default(SyndicationFeed);
            if (!String.IsNullOrEmpty(feedUrl) && feedUrl.ToLower().StartsWith("http"))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(feedUrl);
                    request.UserAgent = "tooorangey.FeedReader";
                    request.Headers.Add("Accept-Encoding", "gzip,deflate");
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    using (WebResponse response = request.GetResponse())
                    {
                        using (System.Xml.XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                        {
                            feedContent = System.ServiceModel.Syndication.SyndicationFeed.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //think about maybe logging the error
                    LogHelper.Error(MethodBase.GetCurrentMethod().DeclaringType, "Error loading feed: " + feedUrl, ex);
                    feedResult.StatusMessage = "A bad error has occurred: " + ex.Message;
                }
            }
            else
            {
                feedResult.StatusMessage = "Provide a Feed Url beginning with http...";
            }
            if (feedContent != null)
            {
                feedResult.HasFeedResults = true;
                feedResult.SyndicationFeed = feedContent;
            }
            return feedResult;
        }
    }
}
