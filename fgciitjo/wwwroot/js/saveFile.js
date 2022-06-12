function saveFile(fileName, Content) {
    var link = document.createElement('a');
    link.download = fileName;
    // link.href = "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document:base64," + encodeURIComponent(Content)
    link.href = Content;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}