using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using tooorangey.RssFeedUrl.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace tooorangey.RssFeedUrl.PropertyValueConverters
{
    [PropertyValueType(typeof(FeedResult))]
    public class RssFeedUrlPropertyConverter : PropertyValueConverterBase, IPropertyValueConverter
    {
        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            //let's keep the string of the url a string
            var attemptConvertString = source.TryConvertTo<string>();
            if (attemptConvertString.Success)
            {
                return attemptConvertString.Result;
            }

            return null;
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
            {
                return null;
            }
            var feedUrl = source.ToString();
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
            //what if requested dynamically...
            return feedResult;
        }

 
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("tooorangey.RssFeedUrl");
        }
        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            PropertyCacheLevel returnLevel;
            switch (cacheValue)
            {
                case PropertyCacheValue.Object:
                    returnLevel = PropertyCacheLevel.Request;
                    break;
                case PropertyCacheValue.Source:
                    returnLevel = PropertyCacheLevel.Content;
                    break;
                case PropertyCacheValue.XPath:
                    returnLevel = PropertyCacheLevel.Content;
                    break;
                default:
                    returnLevel = PropertyCacheLevel.None;
                    break;
            }

            return returnLevel;
        }

    }
}
