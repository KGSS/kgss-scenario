//Author: Richard Bunt
using System.Collections;
using UnityEngine;

class Disaster : TutorialScenario
{
    protected override void OnAssetSetup()
    {
        instructorPrefabName = "Instructor_Gene";
    }

    TutorialPage opening;

    protected override void OnTutorialSetup()
    {
        instructor.name = "Togfox";

        #region opening

        opening = new TutorialPage("opening");
        opening.windowTitle = "[KGSS] Mission";
        opening.OnEnter = (KFSMState st) =>
        {
            instructor.StopRepeatingEmote();
            InputLockManager.SetControlLock((ControlTypes.STAGING | ControlTypes.THROTTLE), "UnamedLock");
        };
        opening.OnDrawContent = () =>
        {
            instructor.PlayEmote(instructor.anim_idle_sigh);
            GUILayout.Label("This is mission control to ...");

            if (GUILayout.Button("Next")) Tutorial.GoToNextPage();
        };
        Tutorial.AddPage(opening);

        #endregion

        Tutorial.StartTutorial(opening);
    }

    void OnDestroy()
    {
        //InputLockManager.RemoveControlLock("UnamedLock");
    }

    protected void failureDecouple()
    {
        FlightGlobals.ActiveVessel.parts[0].decouple();
    }
}
