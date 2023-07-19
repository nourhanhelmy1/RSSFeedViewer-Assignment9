using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Linq;
using RenderingRSS.Services; // Add this namespace

namespace RenderingRSS.Pages
{
    public class FavoritesModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RssService _rssService;

        public List<RssItem> FavoriteRssItems { get; set; } = new List<RssItem>();

        public FavoritesModel(IHttpContextAccessor httpContextAccessor, RssService rssService)
        {
            _httpContextAccessor = httpContextAccessor;
            _rssService = rssService;
        }

        public void OnGet()
        {
            var favoriteFeeds = GetFavoriteFeedCookies();

            // Filter RssItems to include only favorite feeds
            FavoriteRssItems = _rssService.RssItems.Where(item => favoriteFeeds.Contains(item.Link)).ToList();
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
}
