using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KGSS_Scenario
{
    class FailureGenerator
    {
        protected delegate void Failure();
        protected static Random random = new Random();

        //Store the failure function and the associated probabilities in a pair.
        //The pair of probabilities represent the base probability, and the probability adjusted based on craft metrics.
        protected List<Pair<Failure, Pair<float,float>>> possibleFailures = new List<Pair<Failure, Pair<float,float>>>();

        #region Initilisation

        public FailureGenerator()
        {
            initialiseFailures();
        }

        private void initialiseFailures()
        {
            //Intermittent part explosion.
            possibleFailures.Add(new Pair<Failure, Pair<float, float>>(() =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    KGSSLogger.Log("Random Failure: Intermittent explosion");

                    FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].explode();
                }

            }, new Pair<float,float>(0.05f, 0f)));

            //Intermittent part disable.
            possibleFailures.Add(new Pair<Failure, Pair<float, float>>(() =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    KGSSLogger.Log("Random Failure: Intermittent disable");

                    FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].enabled = false;
                }

            }, new Pair<float, float>(0.05f, 0f)));


            //Intermittent part enable.
            possibleFailures.Add(new Pair<Failure, Pair<float, float>>(() =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    KGSSLogger.Log("Random Failure: Intermittent enable");
                    FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].enabled = true;
                }

            }, new Pair<float, float>(0.05f, 0f)));
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
            foreach (Pair<Failure,  Pair<float, float>> pair in possibleFailures)
            {
                pair.Second.Second = pair.Second.First + (0.001f * FlightGlobals.ActiveVessel.parts.Count);
                KGSSLogger.Log("Random Failure: " + pair.First.ToString() + " probability set to: " + pair.Second.Second);
            }
        }

        private void potentiallyCauseFailure()
        {
            if (!FlightGlobals.ActiveVessel.Landed)
            {
                KGSSLogger.Log("Random Failure: Checking for failure");
                foreach (Pair<Failure, Pair<float, float>> pair in possibleFailures)
                {
                    if (random.NextDouble() < pair.Second.Second)
                    {
                        pair.First();
                        //Limit to one failure per update cycle
                        break;
                    }
                }
            }
        }
        
        #endregion
    }
}
