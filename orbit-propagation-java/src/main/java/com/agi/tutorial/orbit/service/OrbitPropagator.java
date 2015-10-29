package com.agi.tutorial.orbit.service;

import static agi.foundation.Trig.degreesToRadians;

import java.awt.Color;
import java.io.StringWriter;
import java.net.URI;
import java.net.URISyntaxException;

import agi.foundation.celestial.CentralBodiesFacet;
import agi.foundation.celestial.EarthCentralBody;
import agi.foundation.celestial.WorldGeodeticSystem1984;
import agi.foundation.cesium.BillboardGraphics;
import agi.foundation.cesium.BillboardGraphicsExtension;
import agi.foundation.cesium.CesiumReferenceFrameExtension;
import agi.foundation.cesium.CesiumResource;
import agi.foundation.cesium.CesiumResourceBehavior;
import agi.foundation.cesium.Clock;
import agi.foundation.cesium.ConstantCesiumProperty;
import agi.foundation.cesium.CzmlDocument;
import agi.foundation.cesium.LabelGraphics;
import agi.foundation.cesium.LabelGraphicsExtension;
import agi.foundation.cesium.PathGraphics;
import agi.foundation.cesium.PathGraphicsExtension;
import agi.foundation.cesium.PolylineOutlineMaterialGraphics;
import agi.foundation.coordinates.KeplerianElements;
import agi.foundation.coordinates.Rectangular;
import agi.foundation.geometry.AxesVehicleVelocityLocalHorizontal;
import agi.foundation.infrastructure.ExtensionCollection;
import agi.foundation.platforms.Platform;
import agi.foundation.propagators.TwoBodyPropagator;
import agi.foundation.time.JulianDate;
import agi.foundation.time.TimeInterval;

import com.agi.tutorial.orbit.model.SatelliteState;

public class OrbitPropagator
{
    public String propagateToCzml(SatelliteState data) {
        EarthCentralBody earth = CentralBodiesFacet.getFromContext().getEarth();
        KeplerianElements ke = new KeplerianElements(data.getSemiMajorAxis(), data.getEccentricity(),
                degreesToRadians(data.getInclination()), degreesToRadians(data.getArgOfPeriapsis()),
                degreesToRadians(data.getRightAscensionOfAscendingNode()), degreesToRadians(data.getTrueAnomaly()),
                WorldGeodeticSystem1984.GravitationalParameter);

        TwoBodyPropagator propagator = new TwoBodyPropagator(new JulianDate(data.getStartTime()),
                earth.getInertialFrame(), ke);

        Platform satellite = new Platform();
        satellite.setName(data.getSatelliteName());
        satellite.setLocationPoint(propagator.createPoint());
        satellite.setOrientationAxes(new AxesVehicleVelocityLocalHorizontal(earth.getInertialFrame(), satellite
                .getLocationPoint()));

        ExtensionCollection extensions = satellite.getExtensions();
        extensions.add(new LabelGraphicsExtension(createLabelGraphics(satellite.getName())));
        extensions.add(new PathGraphicsExtension(createPathGraphics()));
        extensions.add(new BillboardGraphicsExtension(createBillboardGraphics()));

        CesiumReferenceFrameExtension refExtension = new CesiumReferenceFrameExtension();
        refExtension.setCesiumReferenceFrame(earth.getInertialFrame());
        extensions.add(refExtension);

        CzmlDocument czml = new CzmlDocument();
        czml.setName("Simple Example");
        czml.setDescription("Simple Web Service Example");
        czml.setRequestedInterval(new TimeInterval(new JulianDate(data.getStartTime()), new JulianDate(data
                .getStopTime())));

        Clock clock = new Clock();
        clock.setInterval(czml.getRequestedInterval());
        clock.setMultiplier(60.0);
        czml.setClock(clock);

        czml.getObjectsToWrite().add(satellite);

        StringWriter writer = new StringWriter();
        czml.writeDocument(writer);

        return writer.toString();
    }

    private LabelGraphics createLabelGraphics(String text) {
        LabelGraphics graphics = new LabelGraphics();
        graphics.setText(new ConstantCesiumProperty<>(text));
        graphics.setFillColor(new ConstantCesiumProperty<>(Color.WHITE));
        graphics.setPixelOffset(new ConstantCesiumProperty<>(new Rectangular(0, 0)));

        return graphics;
    }

    private PathGraphics createPathGraphics() {
        PathGraphics graphics = new PathGraphics();
        graphics.setShow(new ConstantCesiumProperty<>(true));
        graphics.setWidth(new ConstantCesiumProperty<>(2.0));
        graphics.setTrailTime(new ConstantCesiumProperty<>(10_000.0));
        graphics.setLeadTime(new ConstantCesiumProperty<>(10_000.0));

        PolylineOutlineMaterialGraphics material = new PolylineOutlineMaterialGraphics();
        material.setColor(new ConstantCesiumProperty<>(Color.WHITE));
        material.setOutlineColor(new ConstantCesiumProperty<>(Color.BLACK));
        material.setOutlineWidth(new ConstantCesiumProperty<>(0.5));
        graphics.setMaterial(new ConstantCesiumProperty<>(material));

        return graphics;
    }

    private BillboardGraphics createBillboardGraphics() {
        URI uri;
        try {
            uri = new URI("/Images/Satellite.png");
        }
        catch (URISyntaxException ex) {
            throw new IllegalStateException(ex);
        }

        CesiumResource satMarker = new CesiumResource(uri, CesiumResourceBehavior.LINK_TO);
        BillboardGraphics graphics = new BillboardGraphics();
        graphics.setShow(new ConstantCesiumProperty<>(true));
        graphics.setScale(new ConstantCesiumProperty<>(2.0));
        graphics.setImage(new ConstantCesiumProperty<>(satMarker));

        return graphics;
    }
}
