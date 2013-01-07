//Author: Richard Bunt
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


class ImpactProbe: Part
{
    #region Constants

    public const string SATELLITE_NAME = "KGSS X-ray Satellite";
    private const int AVERAGE_VALUES = 100;

    #endregion

    #region Fields

    private LinkedList<double> velocities = new LinkedList<double>();

    #endregion

    #region Part Events

    protected override void onPartExplode()
    {
        generateMissionDebrief();
        base.onPartExplode();
    }

    protected override void onPartFixedUpdate()
    {
        velocities.AddFirst(vessel.srf_velocity.magnitude);

        if (velocities.Count > AVERAGE_VALUES)
        {
            velocities.RemoveLast();
        }
    }

    #endregion

    public static double average(LinkedList<double> l)
    {
        double sum = 0;
        int count = 0;

        foreach (double d in l)
        {
            sum += d;

            if (d > 0)
            {
                count++;
            }
        }

        return sum / (double)count;
    }

    private void generateMissionDebrief()
    {
        MinmusComposition s = getMinmusCompositionScenarioModule();

        if(s!= null)
        {
            MinmusCompositionMissionEndInformation info = new MinmusCompositionMissionEndInformation();
            List<Vessel> satellite = getXraySatellites();

            info.soiName =  FlightGlobals.currentMainBody.name;
            info.velocity = average(velocities);

            if(satellite.Count > 0)
            {
                foreach (Vessel v in satellite)
                {
                    info.lineOfSight.Add(NeutrinoSensorModule.isLineOfSight(vessel.GetWorldPos3D(), v.GetWorldPos3D()));
                    info.distance.Add(Vector3d.Distance(vessel.GetWorldPos3D(), v.GetWorldPos3D()));
                }
                
                info.alt = vessel.heightFromTerrain;
            }
            else
            {
                info.lineOfSight.Add(false);
            }

            s.missionEnd(info);
        }
    }

    private MinmusComposition getMinmusCompositionScenarioModule()
    {
        foreach (ScenarioModule s in ScenarioRunner.GetLoadedModules())
        {
            if(s.GetType().Equals(typeof(MinmusComposition)))
            {
                return (MinmusComposition)s;
            }
        }

        return null;
    }

    private List<Vessel> getXraySatellites()
    {
        List<Vessel> ret = new List<Vessel>();

        KGSSPluginLogger.Log("ImpactProbe - getXraySatellites");
        foreach (Vessel v in FlightGlobals.Vessels)
        {
            KGSSPluginLogger.Log("ImpactProbe - Vessels: " + v.name);
            if (v.name.StartsWith(SATELLITE_NAME))
            {
                ret.Add(v);
            }
        }

        return ret;
    }
}

