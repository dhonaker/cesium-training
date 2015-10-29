var app = (function () {
    var self = this;

    self.url = 'http://localhost:8080/satellite/propagate';

    self.initCesium = function () {
        self.viewer = new Cesium.Viewer('cesiumContainer');
    };

    self.PropagateOrbit = function () {
        var satName = $('#satelliteName').val();
        var startTime = $('#startTime').val();
        var stopTime = $('#stopTime').val();
        var semiMA = $('#semiMajorAxis').val();
        var ecc = $('#eccentricity').val();
        var inc = $('#inclination').val();
        var argOfPer = $('#argOfPeriapsis').val();
        var raan = $('#raan').val();
        var trueAnomaly = $('#trueAnomaly').val();

        var postData = {
            startTime: startTime,
            stopTime: stopTime,
            satelliteName: satName,
            semiMajorAxis: semiMA,
            eccentricity: ecc,
            inclination: inc,
            argOfPeriapsis: argOfPer,
            raan: raan,
            trueAnomoly: trueAnomaly
        };

        self.ajaxRequest(self.url, 'POST', postData).then(self.addCzmlToViewer);
    };

    self.ajaxRequest = function (url, type, data) {
        var options = {
            url: url,
            headers: {
                Accept: "application/json"
            },
            contentType: "application/json",
            dataType: 'json',
            cache: false,
            type: type,
            data: JSON.stringify(data) 
        }

        return $.ajax(options);
    };

    self.addCzmlToViewer = function (data) {
        self.viewer.dataSources.add(Cesium.CzmlDataSource.load(data));
    };

    return self;
})();