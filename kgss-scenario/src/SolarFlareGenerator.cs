using System;
using System.Collections.Generic;
using System.Text;


class SolarFlareGenerator : ProbabilityEventGenerator
{
    protected double durationLower = 0, durationUpper = 0, duration = 0;
    protected int currentEvent = 0;

    public SolarFlareGenerator(List<int> weights, Random random, double durationLower, double durationUpper)
        : base(weights, random)
    {
        this.durationLower = durationLower;
        this.durationUpper = durationUpper;
    }

    public override int generateEvent()
    {
        if (duration <= 0)
        {
            duration = durationLower + random.NextDouble() * (durationUpper - durationLower);
            
            

            currentEvent = base.generateEvent();

            switch (currentEvent)
            {
                case 0:
                    KGSSPluginLogger.Log("Neutrino Sensor Module - No Flare of duration " + duration);
                    break;
                case 1:
                    KGSSPluginLogger.Log("Neutrino Sensor Module - Small Flare of duration " + duration);
                    break;
                case 2:
                    KGSSPluginLogger.Log("Neutrino Sensor Module - Large Flare of duration " + duration);
                    break;
            }
        }
        else
        {
            duration--;
        }

        return currentEvent;
    }
}
