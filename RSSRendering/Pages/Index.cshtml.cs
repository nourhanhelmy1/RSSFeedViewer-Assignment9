using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using RenderingRSS.Services; // Add this namespace

namespace RenderingRSS.Pages
{
    public class IndexModel : PageModel
    {
        private const int PageSize = 10;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RssService _rssService; // Add RssService dependency

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

        public IndexModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, RssService rssService)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _rssService = rssService;
        }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1, string star = null, string unstar = null)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var opmlResponse = await httpClient.GetAsync("https://blue.feedland.org/opml?screenname=dave");

            if (opmlResponse.IsSuccessStatusCode)
            {
                var opmlContent = await opmlResponse.Content.ReadAsStringAsync();
                var feedUrls = ParseOpmlContent(opmlContent);

                var tasks = feedUrls.Select(url => FetchAndParseRssFeedAsync(httpClient, url));
                var rssResponses = await Task.WhenAll(tasks);
                _rssService.RssItems = rssResponses.SelectMany(r => r).ToList();

                TotalPages = (int)Math.Ceiling((double)_rssService.RssItems.Count / PageSize);
                CurrentPage = pageNumber;

                // Apply paging
                var rssItems = _rssService.RssItems
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                return Page();
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }

        private List<string> ParseOpmlContent(string opmlContent)
        {
            var feedUrls = new List<string>();

            var doc = XDocument.Parse(opmlContent);
            var outlines = doc.Descendants("outline");

            foreach (var outline in outlines)
            {
                var xmlUrl = outline.Attribute("xmlUrl")?.Value;
                if (!string.IsNullOrEmpty(xmlUrl))
                {
                    feedUrls.Add(xmlUrl);
                }
            }

            return feedUrls;
        }

        private async Task<List<RssItem>> FetchAndParseRssFeedAsync(HttpClient httpClient, string url)
        {
            var rssItemList = new List<RssItem>();

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var xmlContent = await response.Content.ReadAsStringAsync();
                rssItemList = ParseXmlContent(xmlContent);

                // Check if the feed is in the favorites list and update the IsFavorite property
                foreach (var rssItem in rssItemList)
                {
                    var favoriteFeeds = GetFavoriteFeedCookies();
                    rssItem.IsFavorite = favoriteFeeds.Contains(rssItem.Link);
                }
            }

            return rssItemList;
        }

        private List<RssItem> ParseXmlContent(string xmlContent)
        {
            var rssItemList = new List<RssItem>();
            var doc = XDocument.Parse(xmlContent);
            var items = doc.Descendants("item");

            foreach (var item in items)
            {
                var rssItem = new RssItem
                {
                    Description = item.Element("description")?.Value,
                    PubDate = item.Element("pubDate")?.Value,
                    Link = item.Element("link")?.Value,
                };

                rssItemList.Add(rssItem);
            }

            return rssItemList;
        }

        private void SetFavoriteFeedCookie(string feedLink, bool isFavorite)
        {
            var favoriteFeeds = GetFavoriteFeedCookies();
            if (isFavorite)
            {
                favoriteFeeds.Add(feedLink);
            }
            else
            {
                favoriteFeeds.Remove(feedLink);
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append("FavoriteFeeds", string.Join(',', favoriteFeeds));
        }

        private List<string> GetFavoriteFeedCookies()
        {
            var favoriteFeedsCookie = _httpContextAccessor.HttpContext.Request.Cookies["FavoriteFeeds"];
            if (!string.IsNullOrEmpty(favoriteFeedsCookie))
            {
                return favoriteFeedsCookie.Split(',').ToList();
            }

            return new List<string>();
        }
    }

    public class RssItem
    {
        public string Description { get; set; }
        public string PubDate { get; set; }
        public string Link { get; set; }
        public bool IsFavorite { get; set; } // New property to indicate whether the feed is a favorite
    }
}
