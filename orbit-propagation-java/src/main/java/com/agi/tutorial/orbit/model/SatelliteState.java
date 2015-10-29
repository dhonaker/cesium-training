package com.agi.tutorial.orbit.model;

import org.joda.time.DateTime;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SatelliteState
{
    private DateTime startTime;

    private DateTime stopTime;

    private String satelliteName;

    private double semiMajorAxis;

    private double eccentricity;

    private double inclination;

    private double argOfPeriapsis;

    @JsonProperty("raan")
    private double rightAscensionOfAscendingNode;

    private double trueAnomaly;

    public DateTime getStartTime() {
        return startTime;
    }

    public void setStartTime(DateTime startTime) {
        this.startTime = startTime;
    }

    public DateTime getStopTime() {
        return stopTime;
    }

    public void setStopTime(DateTime stopTime) {
        this.stopTime = stopTime;
    }

    public String getSatelliteName() {
        return satelliteName;
    }

    public void setSatelliteName(String satelliteName) {
        this.satelliteName = satelliteName;
    }

    public double getSemiMajorAxis() {
        return semiMajorAxis;
    }

    public void setSemiMajorAxis(double semiMajorAxis) {
        this.semiMajorAxis = semiMajorAxis;
    }

    public double getEccentricity() {
        return eccentricity;
    }

    public void setEccentricity(double eccentricity) {
        this.eccentricity = eccentricity;
    }

    public double getInclination() {
        return inclination;
    }

    public void setInclination(double inclination) {
        this.inclination = inclination;
    }

    public double getArgOfPeriapsis() {
        return argOfPeriapsis;
    }

    public void setArgOfPeriapsis(double argOfPeriapsis) {
        this.argOfPeriapsis = argOfPeriapsis;
    }

    public double getRightAscensionOfAscendingNode() {
        return rightAscensionOfAscendingNode;
    }

    public void setRightAscensionOfAscendingNode(double rightAscensionOfAscendingNode) {
        this.rightAscensionOfAscendingNode = rightAscensionOfAscendingNode;
    }

    public double getTrueAnomaly() {
        return trueAnomaly;
    }

    public void setTrueAnomaly(double trueAnomaly) {
        this.trueAnomaly = trueAnomaly;
    }

    public void validate() {
        if (startTime == null || stopTime == null) {
            throw new AssertionError("StartTime and StopTime must be provided");
        }

        if (startTime.compareTo(stopTime) > 0) {
            throw new AssertionError("StartTime must be less than StopTime");
        }
    }
}
