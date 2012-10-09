using System.Collections;
using UnityEngine;

namespace KGSS_Scenario
{
    class RandomFailure : TutorialScenario
    {
        const float START_FAILURE_DELAY_SECONDS = 10;
        const float FAILURE_INTERVAL_SECONDS = 10;

        FailureGenerator failureGenerator = null;

        protected override void OnTutorialSetup()
        {
            failureGenerator = new FailureGenerator();

            this.InvokeRepeating("fixedUpdateFailureGenerator", 
                START_FAILURE_DELAY_SECONDS, FAILURE_INTERVAL_SECONDS);
        }

        protected void fixedUpdateFailureGenerator()
        {
            KGSSLogger.Log("RandomFailure.fixedUpdateFailureGenerator");
            failureGenerator.fixedUpdate();
        }
    }
}
