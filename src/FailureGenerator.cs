using System;
using System.Collections.Generic;
using System.Text;


class FailureGenerator
{

    protected Random random = null;
    //Store the failure function and the associated probabilities in a pair.
    //The pair of probabilities represent the base probability, and the probability adjusted based on craft metrics.
    protected List<FailureDescriptor> possibleFailures = null;

    protected double failureProbability = 0;
    protected double baseFailureProbability = 0;

    protected ProbabilityEventGenerator probabilityEventGenerator = null;

    #region Initilisation

    public FailureGenerator(double failureProbability, ref List<FailureDescriptor> possibleFailures, ref Random random)
    {
        this.failureProbability = failureProbability;
        this.baseFailureProbability = failureProbability;
        this.possibleFailures = possibleFailures;
        this.random = random;
        
        initialiseProbabilityEventGenerator();
    }

    private void initialiseProbabilityEventGenerator()
    {
        List<int> weights = new List<int>();

        foreach(FailureDescriptor fd in possibleFailures)
        {
            weights.Add(fd.weight);
        }

        probabilityEventGenerator = new ProbabilityEventGenerator(weights, random);
    }

    #endregion

    #region Events

    public void fixedUpdate()
    {
        adjustFailureProbabilityBasedOnCraftSize();
        potentiallyCauseFailure();
    }

    private void adjustFailureProbabilityBasedOnCraftSize()
    {
        //Adjust failure rate based on the number of parts in the vessel.
        failureProbability = baseFailureProbability + (0.001f * FlightGlobals.ActiveVessel.parts.Count);
        KGSSLogger.Log("Failure Generator - Failure probability set to: " + failureProbability);
    }

    private void potentiallyCauseFailure()
    {
        if (!FlightGlobals.ActiveVessel.Landed)
        {
            KGSSLogger.Log("Failure Generator - Checking for failure");

            if (random.NextDouble() < failureProbability)
            {
                possibleFailures[probabilityEventGenerator.generateEvent()].failure();
            }
        }
    }
        
    #endregion
}

