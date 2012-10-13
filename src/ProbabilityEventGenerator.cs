using System;
using System.Collections.Generic;
using System.Text;


class ProbabilityEventGenerator
{
    private List<int> cdf = new List<int>();
    Random random = null;

    public ProbabilityEventGenerator(List<int> weights, Random random)
    {
        this.random = random;
        calculateCdf(weights);
    }

    private void calculateCdf(List<int> weights)
    {
        KGSSLogger.Log("ProbabilityEventGenerator - weights:" + KGSSLogger.ListToString(weights));
        if (weights.Count > 0)
        {
            cdf.Add(weights[0]);

            for (int i = 1; i < weights.Count; i++)
            {
                cdf.Add(cdf[cdf.Count - 1] + weights[i]);
            }
        }

        KGSSLogger.Log("ProbabilityEventGenerator - cdf:" + KGSSLogger.ListToString(cdf));
    }

    public int generateEvent()
    {
        double selection = random.NextDouble() * cdf[cdf.Count - 1];
        KGSSLogger.Log("ProbabilityEventGenerator - Raw event generated: " + selection);
        
        int i = 0;

        while (selection > cdf[i])
        {
            i++;
        }

        KGSSLogger.Log("ProbabilityEventGenerator - Event selected: " + i);
        return i;
    }
}

