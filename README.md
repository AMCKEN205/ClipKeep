# ClipKeep
MVC web app that provides a clipboard accessible across different devices! Users can store at max 5 items at  one time. If the users pastes a new item and is at the 5 items limit their oldest item is deleted.

Users are restricted to stroing text or images.

Implemented in C#/ASP.NET for server side logic, Cosmos DB for the database and some HTML + CSS + JS (largely JQuery) on the frontend.

# Site URL: https://clipkeep.azurewebsites.net/

# Browser Compatability 
The website works best on chrome on mobile, and chrome and edge on desktop. The site works on firefox too however the copy to clipboard button doesn't work for images.

# Test Data
Test data that is known to function correctly with the site is located in the TestData folder inside the ClipKeep folder.

# Run Instructions
- Click the register link at the bottom of the login screen
- enter username + password details
- enter credentials on redirect to login
- paste text or images into the clipkeep textarea/pastebox.

# Known issues
- The website has some glitches with some images. Some work and some don't. Not consistent by filetype so may be the base 64 URI that gets sent to the server is too large?
- The website has some glitches with some instances of very large text (i.e. pasting a large chapter of a book) and identifies these as images.

# Worth noting: 
- The root level (ClipKeep/Web.config) Web.config file has been added to .gitignore as it includes a private authorization key for Cosmos DB
