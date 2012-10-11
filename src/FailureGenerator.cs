using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class FailureGenerator
{ 
    protected static Random random = new Random();

    //Store the failure function and the associated probabilities in a pair.
    //The pair of probabilities represent the base probability, and the probability adjusted based on craft metrics.
    protected List<FailureDescriptor> possibleFailures = new List<FailureDescriptor>();

    #region Initilisation

    public FailureGenerator()
    {
        initialiseFailures();
    }

    private void initialiseFailures()
    {
        //Intermittent part explosion.
        possibleFailures.Add(new FailureDescriptor(() =>
        {
            if (FlightGlobals.ActiveVessel.parts.Count > 0)
            {
                KGSSLogger.Log("Random Failure: Intermittent explosion");

                FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].explode();
            }

        }, 0.05f));

        //Intermittent part disable.
        possibleFailures.Add(new FailureDescriptor(() =>
        {
            if (FlightGlobals.ActiveVessel.parts.Count > 0)
            {
                KGSSLogger.Log("Random Failure: Intermittent disable");

                FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].enabled = false;
            }

        }, 0.05f));

        //Intermittent part enable.
        possibleFailures.Add(new FailureDescriptor(() =>
        {
            if (FlightGlobals.ActiveVessel.parts.Count > 0)
            {
                KGSSLogger.Log("Random Failure: Intermittent enable");
                FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].enabled = true;
            }

        }, 0.05f));
    }

    #endregion

    #region Events

    public void fixedUpdate()
    {
        adjustProbabilitiesBasedOnCraftSize();
        potentiallyCauseFailure();
    }

    private void adjustProbabilitiesBasedOnCraftSize()
    {
        foreach (FailureDescriptor pair in possibleFailures)
        {
            pair.probability = pair.baseProbability + (0.001f * FlightGlobals.ActiveVessel.parts.Count);
            KGSSLogger.Log("Random Failure: " + pair.failure.ToString() + " probability set to: " + pair.probability);
        }
    }

    private void potentiallyCauseFailure()
    {
        if (!FlightGlobals.ActiveVessel.Landed)
        {
            KGSSLogger.Log("Random Failure: Checking for failure");
            foreach (FailureDescriptor pair in possibleFailures)
            {
                if (random.NextDouble() < pair.probability)
                {
                    pair.failure();
                    //Limit to one failure per update cycle
                    break;
                }
            }
        }
    }
        
    #endregion
}

