using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class DetermineTheCompositionOfMinmus : TutorialOrbit101
{
    protected override void OnTutorialSetup()
    {
        KGSSLogger.Log("OnTutorialSetup()");

        TutorialPage initialPage = new TutorialPage("p0");
        KFSMEvent e = new KFSMEvent("e0");

        initialPage.OnDrawContent += new Callback(WindowGUI);
        Tutorial.AddPage(initialPage);
        initialPage.windowTitle = "[KGSS] Mission";
        Tutorial.StartTutorial(initialPage);
        base.instructor.CharacterName = "Togfox";
    }

    public static void WindowGUI()
    {
        GUIStyle mySty = new GUIStyle(GUI.skin.button);
        mySty.normal.textColor = mySty.focused.textColor = Color.white;
        mySty.hover.textColor = mySty.active.textColor = Color.yellow;
        mySty.onNormal.textColor = mySty.onFocused.textColor = mySty.onHover.textColor = mySty.onActive.textColor = Color.green;
        mySty.padding = new RectOffset(8, 8, 8, 8);

        GUILayout.BeginVertical();

        GUILayout.Label("The KGSS wishes to understand the physical and chemical properties of the ice that covers the Minmus.  Countless probes and rovers have been sent to the surface for samples. However, the KGSS requires a high-kinetic impact probe to get under the surface, which will allow KGSS scientists to determine the properties of the surface layer and below.", GUILayout.MinWidth(180.0F));
        if (GUILayout.Button("Next", mySty, GUILayout.ExpandWidth(true)))//GUILayout.Button is "true" when clicked
        {

        }
        GUILayout.EndVertical();
    }
}

