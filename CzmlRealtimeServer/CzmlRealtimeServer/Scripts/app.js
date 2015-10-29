var app = (function () {

    var viewer = {};
    var source = {};
    var url = 'http://localhost:60088/api/Realtime';
    var firstLoad = 1;
    var czmldatasource = {};

    //function to create the cesium viewer, called by _run.js
    var _initCesium = function () {
        viewer = new Cesium.Viewer('cesiumContainer');
    };

    var _connectToRealtime = function () {
        source = new EventSource(url);
        czmldatasource = new Cesium.CzmlDataSource('Realtime');

        source.addEventListener('message', function (e) {
            var json = JSON.parse(e.data);

            if (json.removeID) {
                czmldatasource.entities.removeById(json.removeID);
                return;
            }

            if (firstLoad) {
                czmldatasource.process(json[0]);  //parse document object
                viewer.dataSources.add(czmldatasource);
                firstLoad = 0;
            }

            else {
                czmldatasource.process(json[1]);
            }

        }, false);

        source.addEventListener('open', function (e) {
            console.log("open!");
        }, false);

        source.addEventListener('error', function (e) {
            if (e.readyState == EventSource.CLOSED) {
                console.log("error!");
            }
        }, false);
    };

    return {
        initCesium: _initCesium,
        connectToRealtime: _connectToRealtime
    };

})();





