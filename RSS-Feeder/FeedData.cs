using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace RSS_Feeder
{
    //Holds info for a single blog feed, including a list of blog posts (FeedItem)
    public class FeedData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PubDate { get; set; }
        public Uri Link { get; set; }

        private List<FeedItem> _Items = new List<FeedItem>();
        public List<FeedItem> Items
        {
            get
            {
                return this._Items;
            }
        }
    }


    //Holds info for a single blog post
    public class FeedItem
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime PubDate { get; set; }
        public Uri Link { get; set; }
    }

    //Holds a collection of blog feeds (FeedData), and contains methods needed to retrieve the feeds.
    public class FeedDataSource
    {
        private ObservableCollection<FeedData> _Feeds = new ObservableCollection<FeedData>();
        public ObservableCollection<FeedData> Feeds
        {
            get
            {
                return this._Feeds;
            }
        }
        public async Task GetFeedsAsync()
        {

        }

        private async Task<FeedData> GetFeedAsync(string feedUriString)
        {
            SyndicationClient client = new SyndicationClient();
            Uri feedUri = new Uri(feedUriString);
            try
            {
                SyndicationFeed feed = await client.RetrieveFeedAsync(feedUri);
                FeedData feedData = new FeedData();

                if(feed.Title != null && feed.Title.Text != null)
                {
                    feedData.Title = feed.Title.Text;
                }
                if (feed.Subtitle != null && feed.Subtitle.Text != null)
                {
                    feedData.Description = feed.Subtitle.Text;
                }
                if(feed.Items != null && feed.Items.Count > 0)
                {
                    //Use the date of the latest post as the last updated date
                    feedData.PubDate = feed.Items[0].PublishedDate.DateTime;
                    foreach(SyndicationItem item in feed.Items)
                    {
                        FeedItem feedItem = new FeedItem();
                        if (item.Title != null && item.Title.Text != null)
                        {
                            feedItem.Title = item.Title.Text;
                        }
                        if(item.PublishedDate != null)
                        {
                            feedItem.PubDate = item.PublishedDate.DateTime;
                        }
                        if(item.Authors != null && item.Authors.Count > 0)
                        {
                            feedItem.Author = item.Authors[0].Name.ToString();
                        }


                        if(feed.SourceFormat == SyndicationFormat.Rss20)
                        {
                            if(item.Summary != null && item.Summary.Text != null)
                            {
                                feedItem.Content = item.Summary.Text;
                            }
                            if(item.Links != null && item.Links.Count > 0)
                            {
                                feedItem.Link = item.Links[0].Uri;
                            }
                        }
                        feedData.Items.Add(feedItem);
                    }
                }
                return feedData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static FeedData GetFeed(string title)
        {

        }


    }


}
