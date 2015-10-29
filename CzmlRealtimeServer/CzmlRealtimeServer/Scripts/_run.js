//This is a jquery function that runs when the document (DOM) is ready, meaning
//all html has been loaded.  This is where everything starts!
$(function () {
    app.initCesium();
    app.connectToRealtime();
});