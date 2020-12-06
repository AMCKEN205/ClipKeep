// Functional / exposed functions

// Would use a button here but browsers prevent access to user clipboard unless onPaste.
jQuery(".pasteform textarea").on("paste", function (e) {
    // Disable the paste submit area whilst handling a ongoing paste submission.
    $(".pasteform textarea").prop("disabled", "true")
    e.preventDefault();

    var data = e.originalEvent;
    
    if (data.clipboardData && data.clipboardData.items)
    {

        var items = data.clipboardData.items;
        var userData = new Object();
        // Check the whole clipboard
        for (var clipBoardIndex = 0; clipBoardIndex < items.length; clipBoardIndex++)
        {
            // handle image files
            if (items[clipBoardIndex].type.indexOf("image") != -1) {

                // convert image to byte string so we can send to the server in JSON format.

                var imageFile = items[clipBoardIndex].getAsFile();
                // Set an image file size paste limit to avoid hitting azure cosmos free tier limits!

                var imageFileSizeBytesLimit = 1000000 * 30 // 1 MB = 1000000 bytes so * 30 for 30 MB

                // Check the image pasted doesn't hit the file size limit.
                // JS clipboard file retrieval seems to increase file size however, can't work out why 
                // however google suggests it's a common issue.
                // TODO: get the actual file size without increasing. 
                if (imageFile.size > imageFileSizeBytesLimit) {
                    alert("Exceeded file size paste limit! files shouldn't be larger than 30MB.")
                    return;
                }

                // Get the file type, used for copied image display on the frontend.
                // Pasted images usually get converted to PNG anyway (found this out from debug + google!) 
                // so shouldn't be an issue but just in case! (and as I'd already written the logic!)
                userData.PastedContentType = imageFile.type;

                handleImageFileRead(imageFile, userData);
                messageForAlert = "";
                return;
            }
            // handle text
            else if (items[clipBoardIndex].type.indexOf("text/plain") != -1) {
                items[clipBoardIndex].getAsString(function(dataString) {
                    userData.PastedContent = dataString;
                    userData.PastedContentType = "Text";
                    submitUserData(userData);
                    messageForAlert = "";
                    return;
                });
                
            }
        }
        // otherwise tell the user they've pasted data not accepted by ClipKeep. No need for conditional
        // as we'll only reach this point if we haven't returned due to too large a file, text submit or
        // image submit.

        // We'll have to manually disable the clipboard as the user won't be re-getting the page.
        $(".pasteform textarea").prop("disabled", "false");

        // TODO: alert user. This seems easier than it actually is. Was having all sorts of timing issues
        // issues where the alert value became the UserData object's pasted content value etc. Threading issues?!
    }
});

// Helper functions

function submitUserData(userData) {
    if ("PastedContent" in userData) {
        var userDataJson = JSON.stringify(userData);
        jQuery.ajax
        ({
            type: "POST",
            data: userDataJson,
            contentType: "application/json",
            processData: false,
            url: "MainApp/Index",
            success: function(result) {
                console.log(result);
            },
            error: function(error) {
                console.log(error);
            }
        });
        // Force page refresh in 2 seconds so user can see their pasted content
        setTimeout(() => window.location.href = window.location.href, 1000);
    } 
}

function handleImageFileRead(imageFile, userData) {
    // Define pasted image to data url string getter, allows storage in JSON doc sent to server.
    var imageFromFile = new FileReader();

    imageFromFile.addEventListener("load", function () {
        // Convert to base64 URL representation
        userData.PastedContent = imageFromFile.result;
        submitUserData(userData)
    }, false);

    imageFromFile.readAsDataURL(imageFile);
}


