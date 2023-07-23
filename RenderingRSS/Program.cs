var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapPost("/ToggleFavorite", async (HttpContext context) =>
{
    var link = context.Request.Form["link"];
    var favoriteFeeds = GetFavoriteFeedsFromCookie(context);

    if (!string.IsNullOrEmpty(link))
    {
        if (favoriteFeeds.Contains(link))
        {
            favoriteFeeds.Remove(link);
        }
        else
        {
            favoriteFeeds.Add(link);
        }

        SetFavoriteFeedsInCookie(context, favoriteFeeds);
        var isFavorite = favoriteFeeds.Contains(link);

        await context.Response.WriteAsJsonAsync(new { IsFavorite = isFavorite });
    }
});

app.MapRazorPages();

app.Run();

List<string> GetFavoriteFeedsFromCookie(HttpContext context)
{
    var favoriteFeedsCookie = context.Request.Cookies["FavoriteFeeds"];
    if (!string.IsNullOrEmpty(favoriteFeedsCookie))
    {
        return favoriteFeedsCookie.Split(',').ToList();
    }
    return new List<string>();
}

void SetFavoriteFeedsInCookie(HttpContext context, List<string> favoriteFeeds)
{
    var favoriteFeedsCookieValue = string.Join(',', favoriteFeeds);
    context.Response.Cookies.Append("FavoriteFeeds", favoriteFeedsCookieValue);
}