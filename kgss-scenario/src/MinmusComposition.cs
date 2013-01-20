using KSP.IO;
//Author: Richard Bunt
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MinmusComposition : TutorialScenario
{
    #region Constants

    public static double TERRAIN_THRESHOLD = 30;
    int SCORE_MULT = 10000000;

    #endregion

    #region Fields

    TutorialPage failure = null, success = null;
    List<String> resultText = new List<string>();
    PluginConfiguration config = PluginConfiguration.CreateForType<MinmusComposition>();

    string destinationSOI = "";
    
    #endregion

    #region Tutorial Events

    protected override void OnAssetSetup()
    {
        instructorPrefabName = "Instructor_Gene";
    }

    protected override void OnTutorialSetup()
    {
        readParameters();

        #region Failure

        failure = new TutorialPage("failure");
        failure.windowTitle = "[KGSS] Mission Debrief";
        failure.OnEnter = (KFSMState st) =>
        {
            instructor.name = "Rich";
            instructor.StopRepeatingEmote();
        };
        failure.OnDrawContent = () =>
        {
            instructor.PlayEmote(instructor.anim_false_disappointed);

            foreach (string s in resultText)
            {
                GUILayout.Label(s);
            }

            if (GUILayout.Button("Exit"))
            {
                Destroy(this);
            }
        };
        Tutorial.AddPage(failure);

        #endregion

        #region Success

        success = new TutorialPage("success");
        success.windowTitle = "[KGSS] Mission Debrief";
        success.OnEnter = (KFSMState st) =>
        {
            instructor.name = "Rich";
            instructor.StopRepeatingEmote();
        };
        success.OnDrawContent = () =>
        {
            instructor.PlayEmote(instructor.anim_true_thumbsUp);
            
            foreach (string s in resultText)
            {
                GUILayout.Label(s);
            }
            

            if (GUILayout.Button("Exit"))
            {
                Destroy(this);
            }
        };
        Tutorial.AddPage(success);

        #endregion
    }

    #endregion

    public static double sumVectorIf(List<double> l, List<bool> lb)
    {
        double sum = 0;
        int i = 0;
        foreach (double d in l)
        {
            if (lb[i])
            {
                sum += Math.Pow(d, -1);
            }

            i++;
        }

        return sum;
    }

    public static bool oneTrue(List<bool> l)
    {
        foreach (bool b in l)
        {
            if (b)
            {
                return true;
            }
        }

        return false;
    }

    public void missionEnd(MinmusCompositionMissionEndInformation info)
    {
        if (oneTrue(info.lineOfSight))
        {
            KGSSPluginLogger.Log("Minmus Composition - Crash Altitude: " + info.alt);
            KGSSPluginLogger.Log("Minmus Composition - Crash Velocity: " + info.velocity);
            KGSSPluginLogger.Log("Minmus Composition - Crash SOI: " + info.soiName);
            
            foreach (double d in info.distance)
            {
                KGSSPluginLogger.Log("Minmus Composition - Crash Distance: " + d);
            }

            if (info.soiName.Equals(destinationSOI) && info.alt < TERRAIN_THRESHOLD)
            {
                resultText.Add("Mission Success - The KGSS is now busy analysing this latest data.");
                resultText.Add("");
                resultText.Add("Impact Velocity: ");
                resultText.Add((Math.Round(info.velocity, 1) + " m/s"));

                int i = 0;
                foreach (double d in info.distance)
                {
                    if(info.lineOfSight[i])
                    {
                        resultText.Add("Distance from X-ray Satellite: ");
                        resultText.Add((Math.Round(d,1) + " m"));
                    }

                    i++;
                }

                resultText.Add("Mission Score: ");
                resultText.Add(((info.velocity * sumVectorIf(info.distance, info.lineOfSight) * (double)SCORE_MULT)).ToString());
                Tutorial.StartTutorial(success);
            }
            else
            {
                resultText.Add("Mission Failed - The impact probe was destroyed before reaching the target.");
                Tutorial.StartTutorial(failure);
            }
        }
        else
        {
            resultText.Add("Mission Failed - The KGSS X-ray Satellite had no line of sight to the probes impact site.");
            Tutorial.StartTutorial(failure);
        }
    }

    private void readParameters()
    {
        config.load();

        destinationSOI = config.GetValue<String>("SOI");
    }
}

public class MinmusCompositionMissionEndInformation
{
    public string soiName;
    public List<bool> lineOfSight = new List<bool>();
    public double velocity;
    public List<double> distance = new List<double>();
    public double alt;
}



