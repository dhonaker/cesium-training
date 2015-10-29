var app = (function () {

    var viewer = {};

    var _initCesium = function () {
        viewer = new Cesium.Viewer('cesiumContainer');
    };

    var _shapesVolumes = function () {

        var positions = Cesium.Cartesian3.fromDegreesArray([
        -115.0, 37.0,
        -115.0, 33.0,
        -107.0, 33.0,
        -107.0, 37.0]);

        var holes = Cesium.Cartesian3.fromDegreesArray([
        -113.0, 36.5,
        -113.0, 35.5,
        -109.0, 35.5,
        -109.0, 36.5]);


        //ELLIPSOID Entity
        var ellipsoid = new Cesium.Entity({
            id: 'ellipsoid1',
            name: 'ellipsoid 1',
            position: Cesium.Cartesian3.fromDegrees(-100.0, 30.0, 300000.0),
            ellipsoid: {
                radii: new Cesium.Cartesian3(200000.0, 200000.0, 300000.0),
                material: Cesium.Color.RED,
                outline: true,
                outlineColor: Cesium.Color.fromBytes(0, 255, 0, 255)
            }
        });


        //SAMPLED COLOR PROPERTY
        var nowTime = Cesium.JulianDate.now();
        var plusOneHour = Cesium.JulianDate.addHours(nowTime, 1, new Cesium.JulianDate());

        var colorProperty = new Cesium.SampledProperty(Cesium.Color);
        colorProperty.addSample(nowTime, Cesium.Color.RED);
        colorProperty.addSample(plusOneHour, Cesium.Color.BLUE);

        ellipsoid.ellipsoid.material = new Cesium.ColorMaterialProperty(colorProperty);


        //WALL Entity
        var wall = new Cesium.Entity({
            id: 'wall1',
            name: 'wall 1',
            wall: {
                positions: positions,
                maximumHeights: [70000.0, 190000.0, 40000.0, 150000.0],
                minimumHeights: [0, 20000.0, 30000.0, 50000.0],
                outline: true,
                material: new Cesium.ImageMaterialProperty({
                    image: 'images/colorado.jpg',
                    repeat: new Cesium.Cartesian2(4, 1)
                })
            }
        });


        //POLYGON Entity
        var polygon = new Cesium.Entity({
            id: 'polygon1',
            name: 'polygon 1',
            polygon: {
                hierarchy: {
                    positions: positions,
                    holes: [{ positions: holes}]
                },
                outline: true,
                material: 'images/colorado.jpg'
            }
        });


        //BOX Entity
        var box = new Cesium.Entity();
        box.id = 'box1';
        box.name = 'box 1';
        box.position = Cesium.Cartesian3.fromDegrees(-90, 40.0, 200000.0);

        var boxgraphics = new Cesium.BoxGraphics();
        boxgraphics.dimensions = new Cesium.Cartesian3(400000.0, 600000.0, 300000.0);
        boxgraphics.outline = true;
        boxgraphics.material = new Cesium.GridMaterialProperty({
            color: Cesium.Color.LIME,
            cellAlpha: 0,
            lineCount: new Cesium.Cartesian2(12, 12)
        });

        box.box = boxgraphics;


        //ADD ENTITIES TO VIEWER AND ZOOM
        viewer.entities.add(ellipsoid);
        viewer.entities.add(wall);
        viewer.entities.add(polygon);
        viewer.entities.add(box);

        viewer.flyTo([box, polygon, wall, ellipsoid]);
    };

    var _billboardsModels = function () {

        var point = new Cesium.Entity({
            id: 'point1',
            name: 'point 1',
            position: Cesium.Cartesian3.fromDegrees(-95, 35, 10.0),
            point: {
                pixelSize: 12,
                color: Cesium.Color.AQUA
            },
            label: {
                text: 'Aqua Point',
                verticalOrigin: Cesium.VerticalOrigin.BOTTOM,
                horizontalOrigin: Cesium.HorizontalOrigin.LEFT
            }
        });

        var billboard = new Cesium.Entity({
            id: 'billboard1',
            name: 'billboard 1',
            position: Cesium.Cartesian3.fromDegrees(-105, 40),
            billboard: {
                image: 'images/Facility.png',
                scale: 1.5
            }
        });

        var model = new Cesium.Entity({
            id: 'model1',
            name: 'model 1',
            position: Cesium.Cartesian3.fromDegrees(-100.0, 45.0, 0.0),
            model: {
                uri: 'images/Cesium_Man.bgltf',
                scale: 1000000
            }
        });

        viewer.entities.add(point);
        viewer.entities.add(billboard);
        viewer.entities.add(model);

        var nowTime = Cesium.JulianDate.now();
        var plusTime = Cesium.JulianDate.addSeconds(nowTime, 10.0, new Cesium.JulianDate());
        var plusTime2 = Cesium.JulianDate.addSeconds(nowTime, 20.0, new Cesium.JulianDate());

        var positionProperty = new Cesium.SampledPositionProperty();
        positionProperty.addSample(nowTime, new Cesium.Cartesian3.fromDegrees(-125, 70));
        positionProperty.addSample(plusTime, new Cesium.Cartesian3.fromDegrees(-70, 25));
        positionProperty.addSample(plusTime2, new Cesium.Cartesian3.fromDegrees(-100, 0));

        positionProperty.setInterpolationOptions({
            interpolationAlgorithm: Cesium.LagrangePolynomialApproximation,
            interpolationDegree: 5
        });

        model.position = positionProperty;
        model.orientation = new Cesium.VelocityOrientationProperty(positionProperty);
    };

    var _dataSources = function () {
        var geojson = new Cesium.GeoJsonDataSource('geojson');
        var promise = geojson.load('data/us_counties.json');
        promise.then(function (geojson) {
            viewer.dataSources.add(geojson);

            var entities = geojson.entities.values;
            var entity1 = entities[1];
            entity1.polygon.material = Cesium.Color.RED;

            for (var i = 0; i < entities.length; i++) {
                var entity = entities[i];
                var randomColor = Cesium.Color.fromRandom();
                entity.polygon.material = randomColor;
                entity.polygon.outlineColor = randomColor;
            }
        });
    };

    var _imageryProviders = function () {
        var provider = new Cesium.WebMapServiceImageryProvider({
            url: 'http://mesonet.agron.iastate.edu/cgi-bin/wms/goes/conus_ir.cgi',
            layers: 'goes_conus_ir',
            parameters: {
                transparent: 'true',
                format: 'image/png'
            }
        });

        var layer = viewer.imageryLayers.addImageryProvider(provider);
        layer.alpha = 0.5;
    };

    var _terrainProviders = function () {
        var provider = new Cesium.CesiumTerrainProvider({
            url: '//assets.agi.com/stk-terrain/world',
            requestVertexNormals: true
        });

        viewer.terrainProvider = provider;
    };

    var _picking = function () {
        viewer.screenSpaceEventHandler.setInputAction(function (spot) {
            var cartesian = viewer.camera.pickEllipsoid(spot.position, viewer.scene.globe.ellipsoid);
            var point = viewer.entities.getById('point1');
            point.position = cartesian;
        }, Cesium.ScreenSpaceEventType.LEFT_DOUBLE_CLICK, Cesium.KeyboardEventModifier.SHIFT);

        viewer.screenSpaceEventHandler.setInputAction(function (movement) {
            var cartesian = viewer.camera.pickEllipsoid(movement.endPosition, viewer.scene.globe.ellipsoid);
            var point = viewer.entities.getById('point1');
            point.position = cartesian;
        }, Cesium.ScreenSpaceEventType.MOUSE_MOVE, Cesium.KeyboardEventModifier.CTRL);
    };

    return {
        initCesium: _initCesium,
        shapesVolumes: _shapesVolumes,
        billboardsModels: _billboardsModels,
        dataSources: _dataSources,
        imageryProviders: _imageryProviders,
        terrainProviders: _terrainProviders,
        picking: _picking
    };

})();