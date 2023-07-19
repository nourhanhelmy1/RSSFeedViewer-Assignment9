# RSS Feed Viewer

RSS Feed Viewer is a web application that allows users to view and manage their favorite RSS feeds. Users can mark articles as favorites, and the application will remember their preferences using cookies.

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Technologies Used](#technologies-used)
- [Contributing](#contributing)

## Features

- View a list of RSS feeds fetched from a remote server.
- Pagination support for the feed list.
- Mark articles as favorites with a simple click.
- Persistent storage of favorite articles using cookies.
- Fetch and parse RSS feeds from multiple sources.
- Secure handling of anti-forgery tokens.

## Installation

1. Clone this repository to your local machine.
2. Open the solution in your preferred development environment (e.g., Visual Studio, Visual Studio Code).
3. Build and run the project.

## Usage

1. Open the web application in your web browser.
2. You will see a list of RSS feeds fetched from the server.
3. Click on the "Read More" button to view the full article in a new tab.
4. To mark an article as a favorite, click on the "Star" button next to the article. The button will change to "Unstar" when the article is marked as a favorite.
5. Favorite articles will be remembered even after closing the browser, thanks to the use of cookies.
6. You can navigate through different pages of the feed list using the pagination links at the bottom.

## Technologies Used

- ASP.NET Core
- Razor Pages
- C#
- HTML
- CSS
- JavaScript
- HttpClient
- Anti-forgery tokens
- JSON
- RSS Feeds

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request.

