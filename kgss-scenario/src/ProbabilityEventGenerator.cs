//Author: Richard Bunt
using System;
using System.Collections.Generic;
using System.Text;

class ProbabilityEventGenerator
{
    protected List<int> cdf = new List<int>();
    protected Random random = null;

    public ProbabilityEventGenerator(List<int> weights, Random random)
    {
        this.random = random;
        calculateCdf(weights);
    }

    private void calculateCdf(List<int> weights)
    {
        KGSSPluginLogger.Log("ProbabilityEventGenerator - weights:" + KGSSPluginLogger.ListToString(weights));
        if (weights.Count > 0)
        {
            cdf.Add(weights[0]);

            for (int i = 1; i < weights.Count; i++)
            {
                cdf.Add(cdf[cdf.Count - 1] + weights[i]);
            }
        }

        KGSSPluginLogger.Log("ProbabilityEventGenerator - cdf:" + KGSSPluginLogger.ListToString(cdf));
    }

    public virtual int generateEvent()
    {
        double selection = random.NextDouble() * cdf[cdf.Count - 1];
        KGSSPluginLogger.Log("ProbabilityEventGenerator - Raw event generated: " + selection);
        
        int i = 0;

        while (selection > cdf[i])
        {
            i++;
        }

        if (cdf[cdf.Count - 1] == 0)
        {
            KGSSPluginLogger.Log("Probability Event Generator - CDF 0");
            i = -1;
        }

        KGSSPluginLogger.Log("ProbabilityEventGenerator - Event selected: " + i);

        return i;
    }
}

