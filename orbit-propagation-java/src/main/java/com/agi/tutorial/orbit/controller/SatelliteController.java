package com.agi.tutorial.orbit.controller;

import static org.springframework.web.bind.annotation.RequestMethod.POST;

import java.io.IOException;

import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

import com.agi.tutorial.orbit.model.SatelliteState;
import com.agi.tutorial.orbit.service.OrbitPropagator;

@RestController
@RequestMapping("/satellite")
public class SatelliteController
{
    // @formatter:off
    /*
        {
            "startTime": "2010-01-01T00:00.000",
            "stopTime": "2010-01-02T00:00.000",
            "satelliteName": "ScienceSat",
            "semiMajorAxis": 7000000.000,
            "eccentricity": 0.001,
            "inclination": 85.0,
            "argOfPeriapsis": 0.0,
            "raan": 0.0,
            "trueAnomoly": 0.0
        }
     */
    // @formatter:on

    @RequestMapping(value = "propagate", method = POST, consumes = "application/json", produces = "application/json")
    public @ResponseBody String propagateOrbit(@RequestBody SatelliteState data) throws IOException
    {
        data.validate();

        OrbitPropagator propagator = new OrbitPropagator();
        return propagator.propagateToCzml(data);
    }

    @RequestMapping("test")
    public String test()
    {
        return "Working";
    }
}
