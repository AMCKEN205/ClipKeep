async function copyFromClipKeep(copyBtn) {
    var $TextGetter = $("<input>");

    $("body").append($TextGetter);

    var copiedContentIsText = $(copyBtn).parents(".pasted-item").find(".pasted-item-content").text() != "";

    if (copiedContentIsText) {
        // highlight the text with the element getter and copy to clipboard
        $TextGetter.val($(copyBtn).parents(".pasted-item").find(".pasted-item-content").text()).select();
        document.execCommand("copy");
    } else {
         
        var imgSrc = $(copyBtn).closest(".pasted-item").find("img").attr("src");
        await copyImg(imgSrc);
    }
    
    $TextGetter.remove();
}

async function copyImg(src) {
    // Need to remove the type info so the string is just raw base 64 data url.
    var srcRawB64 = src.replace(/^data:image\/[a-z]+;base64,/, "");

    var imgByteChars = atob(srcRawB64);

    var imgByteNums = new Array(imgByteChars.length);

    for (let byteIndex = 0; byteIndex < imgByteChars.length; byteIndex++) {
        imgByteNums[byteIndex] = imgByteChars.charCodeAt(byteIndex);
    }

    var imgByteArray = new Uint8Array(imgByteNums);

    var imgBlob = new Blob([imgByteArray], { type: "image/png" });

    // Paste the image BLOB into the clipboard.

    try {
        navigator.clipboard.write([
            new ClipboardItem({
                "image/png": imgBlob, 
            })
        ]);
    } catch (error) {
        alert("Sorry, it looks like your browser isn't compatible with our image copying functionality. " +
            "Try using up-to-date edge or chrome browsers!")
        console.error(error);
    }
}