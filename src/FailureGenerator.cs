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
        protected List<Pair<Failure, float>> possibleFailures = new List<Pair<Failure,float>>();

        #region Initilisation

        public FailureGenerator()
        {
            initialiseFailures();
        }

        private void initialiseFailures()
        {
            //Intermittent part explosion.
            possibleFailures.Add(new Pair<Failure, float>(() =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    KGSSLogger.Log("Random Failure: Intermittent explosion");

                    FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].explode();
                }

            }, 0.1f));

            //Intermittent part disable.
            possibleFailures.Add(new Pair<Failure, float>(() =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    KGSSLogger.Log("Random Failure: Intermittent disable");

                    FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].enabled = false;
                }

            }, 0.1f));


            //Intermittent part enable.
            possibleFailures.Add(new Pair<Failure, float>(() =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    KGSSLogger.Log("Random Failure: Intermittent enable");
                    FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].enabled = true;
                }

            }, 0.1f));
        }

        #endregion

        #region Events

        public void fixedUpdate()
        {
            foreach(Pair<Failure, float> pair in possibleFailures)
            {
                if (random.NextDouble() < pair.Second)
                {
                    pair.First();
                    //Limit to one failure per update cycle
                    break;
                }
            }
        }
        
        #endregion
    }
}
