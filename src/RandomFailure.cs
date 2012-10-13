using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;

namespace KGSS_Scenario
{
    class RandomFailure : TutorialScenario
    {
        FailureGenerator failureGenerator = null;
        protected static System.Random random = null;
        PluginConfiguration config = PluginConfiguration.CreateForType<RandomFailure>();

        #region Initilisation

        protected override void OnTutorialSetup()
        {
            config["list"] = new List<string>() {"1", "3" };
            config.save();
            config.load();

            initialiseRandomNumbers();

            List<FailureDescriptor> possibleFailures = new List<FailureDescriptor>();
            initialiseFailures(ref possibleFailures);
            failureGenerator = new FailureGenerator(config.GetValue<Vector2d>("FAILURE_PROBABILITY").x, ref possibleFailures, ref random);

            this.InvokeRepeating("fixedUpdateFailureGenerator", 
                config.GetValue<int>("START_FAILURE_DELAY_SECONDS"), config.GetValue<int>("FAILURE_INTERVAL_SECONDS"));
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

            }, config.GetValue<int>("DEFAULT_INTERMITTENT_EXPLOSION_WEIGHT")));
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
