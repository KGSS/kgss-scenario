using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGSS_Scenario
{
    class RandomFailure : TutorialScenario
    {
        const float START_FAILURE_DELAY_SECONDS = 10;
        const float FAILURE_INTERVAL_SECONDS = 10;
        const float FAILURE_PROBABILITY = 0.5f;
        const byte DEFAULT_INTERMITTENT_EXPLOSION_WEIGHT = 1;

        FailureGenerator failureGenerator = null;
        protected static System.Random random = null;

        #region Initilisation

        protected override void OnTutorialSetup()
        {
            initialiseRandomNumbers();
            List<FailureDescriptor> possibleFailures = new List<FailureDescriptor>();
            initialiseFailures(ref possibleFailures);
            failureGenerator = new FailureGenerator(FAILURE_PROBABILITY, ref possibleFailures, ref random);

            this.InvokeRepeating("fixedUpdateFailureGenerator", 
                START_FAILURE_DELAY_SECONDS, FAILURE_INTERVAL_SECONDS);
        }

        private void initialiseRandomNumbers()
        {
            random = new System.Random(System.DateTime.Now.Second);
        }

        private void initialiseFailures(ref List<FailureDescriptor> possibleFailures)
        {
            //Intermittent part explosion.
            possibleFailures.Add(new FailureDescriptor(() =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    KGSSLogger.Log("Random Failure - Intermittent explosion");

                    FlightGlobals.ActiveVessel.parts[random.Next(0, FlightGlobals.ActiveVessel.parts.Count)].explode();
                }

            }, DEFAULT_INTERMITTENT_EXPLOSION_WEIGHT));
        }

        #endregion

        #region Events

        protected void fixedUpdateFailureGenerator()
        {
            KGSSLogger.Log("RandomFailure.fixedUpdateFailureGenerator");
            failureGenerator.fixedUpdate();
        }

        #endregion

    }   
}
