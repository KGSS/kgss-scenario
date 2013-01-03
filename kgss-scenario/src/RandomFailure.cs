//Author: Richard Bunt
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;
using System;

class RandomFailure : TutorialScenario
{
    FailureGenerator failureGenerator = null;
    protected static System.Random random = null;
    PluginConfiguration config = PluginConfiguration.CreateForType<RandomFailure>();

    FailureDescriptor.FailureGarbage defaultBaseGarbageCollection = null;
    FailureDescriptor.FailureGarbage defaultTypeGarbageCollection = null;
    FailureDescriptor.FailureGarbage defaultPartGarbageCollection = null;

    public delegate void PartStateChange(ref Part parts);

    #region Initilisation
        
    protected override void OnTutorialSetup()
    {
        try
        {
            config.load();

            KGSSLogger.printParts(FlightGlobals.ActiveVessel.parts);

            initialiseRandomNumbers();
        
            initialiseDefaultGarabageCollection();
         
            List<FailureDescriptor> possibleFailures = new List<FailureDescriptor>();
            initialiseFailures(ref possibleFailures);

            failureGenerator = new FailureGenerator(
                config.GetValue<Vector3d>("FAILURE_PROBABILITY").x,
                config.GetValue<Vector3d>("PARTS_PENATLY_FACTOR").x, ref possibleFailures, ref random);

            this.InvokeRepeating("fixedUpdateFailureGenerator",
               config.GetValue<int>("START_FAILURE_DELAY_SECONDS"), config.GetValue<int>("FAILURE_INTERVAL_SECONDS"));
        }
        catch (Exception e)
        {
            KGSSLogger.Out(e.Message); 
            KGSSLogger.Out(e.StackTrace); 
        }
    }

    private void initialiseRandomNumbers()
    {
        random = new System.Random(System.DateTime.Now.Second);
    }

    private void initialiseDefaultGarabageCollection()
    {
        defaultBaseGarbageCollection = (FailureDescriptor failureDescriptor) =>
            {
                ifEmptySelectionSpace(ref FlightGlobals.ActiveVessel.parts,
                    failureDescriptor);
            };

        defaultTypeGarbageCollection = (FailureDescriptor failureDescriptor) =>
            {
                List<Part> selectionSpace = extractPartOfType(FlightGlobals.ActiveVessel.parts,
                    intToPartCategories(Convert.ToInt32(failureDescriptor.partInformation)));

                ifEmptySelectionSpace(ref selectionSpace, failureDescriptor);
            };

        defaultPartGarbageCollection = (FailureDescriptor failureDescriptor) =>
            {
                List<Part> selectionSpace = extractPartOfName(FlightGlobals.ActiveVessel.parts,
                    failureDescriptor.partInformation);

                ifEmptySelectionSpace(ref selectionSpace, failureDescriptor);
            };
    }

    private void initialiseFailures(ref List<FailureDescriptor> possibleFailures)
    {
        KGSSLogger.Log(possibleFailures.ToString());
        initialiseIntermittentExplosions(ref possibleFailures);
        //initialiseIntermittentDetatch(ref possibleFailures);
        initialiseIntermittentFuelLeak(ref possibleFailures);
    }

    private void initialiseIntermittentExplosions(ref List<FailureDescriptor> possibleFailures)
    {
        intermittentFailureHeirachyBuilder(ref possibleFailures, "INTERMITTENT_EXPLOSION",
        (ref Part part) =>
        {
            part.explode();
        }, "explosion");
    }

    private void initialiseIntermittentDetatch(ref List<FailureDescriptor> possibleFailures)
    {
        intermittentFailureHeirachyBuilder(ref possibleFailures, "INTERMITTENT_DETACH",
        (ref Part part) =>
        {
            //part.DetachFromParent();
        }, "detach");
    }

    private void initialiseIntermittentFuelLeak(ref List<FailureDescriptor> possibleFailures)
    {
        intermittentFailureHeirachyBuilder(ref possibleFailures, "INTERMITTENT_FUEL_LEAK",
        (ref Part part) =>
        {
            part.DrainFuel(50f);
        }, "fuel leak");
    }

    private void intermittentFailureHeirachyBuilder(ref List<FailureDescriptor> possibleFailures, string failureTag,
        PartStateChange changePartState, string name)
    {
        failureHeirachyBuilder(ref possibleFailures, failureTag,
            (FailureDescriptor failureDescriptor) =>
            {
                Part part = selectRandomPart(ref FlightGlobals.ActiveVessel.parts);

                changePartState(ref part);
            },
            defaultBaseGarbageCollection,
            "Base Intermittent " + name,
            (FailureDescriptor failureDescriptor) =>
            {
                List<Part> selectionSpace = extractPartOfType(FlightGlobals.ActiveVessel.parts,
                    intToPartCategories(Convert.ToInt32(failureDescriptor.partInformation)));

                Part part = selectRandomPart(ref selectionSpace);

                changePartState(ref part);
            },
            defaultTypeGarbageCollection,
            "Type Intermittent " + name,
            (FailureDescriptor failureDescriptor) =>
            {
                if (FlightGlobals.ActiveVessel.parts.Count > 0)
                {
                    List<Part> selectionSpace = extractPartOfName(FlightGlobals.ActiveVessel.parts,
                        failureDescriptor.partInformation);

                    Part part = selectRandomPart(ref selectionSpace);

                    changePartState(ref part);
                }
            },
            defaultPartGarbageCollection,
            "Part Intermittent " + name);
    }

    private void failureHeirachyBuilder(ref List<FailureDescriptor> possibleFailures, string failureTag, 
        FailureDescriptor.Failure baseFailure, FailureDescriptor.FailureGarbage baseGarbageCollection, string baseName,
        FailureDescriptor.Failure typeFailure, FailureDescriptor.FailureGarbage typeGarbageCollection, string typeName,
        FailureDescriptor.Failure partFailure, FailureDescriptor.FailureGarbage partGarbageCollection, string partName)
    {
        String unsplitClasses = config.GetValue<String>(failureTag + "_MAP");
        string[] classes = unsplitClasses.Split(';');
        
        for (int i = 0; i < classes.Length; i++)
        {
            string[] splitC = classes[i].Split(':');
        
            switch (splitC[0])
            {
                case "BASE":
                
                    possibleFailures.Add(new FailureDescriptor(
                        baseFailure,
                        baseGarbageCollection,
                        config.GetValue<int>(failureTag + "_" + splitC[0] + "_WEIGHT"),
                        "",
                        baseName
                        ));

                    break;

                case "TYPE":
              
                    possibleFailures.Add(new FailureDescriptor(
                        typeFailure,
                        typeGarbageCollection,
                        config.GetValue<int>(failureTag + "_" + splitC[1] + "_WEIGHT"),
                        splitC[1],
                        typeName
                        ));

                    break;

                case "PART":
              
                    possibleFailures.Add(new FailureDescriptor(
                        partFailure,
                        partGarbageCollection,
                        config.GetValue<int>(failureTag + "_" + splitC[1] + "_WEIGHT"),
                        splitC[1],
                        partName
                        ));
                    break;
            }
        }
    }

    private void ifEmptySelectionSpace(ref List<Part> parts, 
        FailureDescriptor failureDescriptor)
    {
        if (parts.Count == 0)
        {
            failureDescriptor.weight = 0;
        }
    }

    private Part selectRandomPart(ref List<Part> parts)
    {
        return parts[random.Next(0, parts.Count - 1)];
    }

    private List<Part> extractPartOfType(List<Part> parts, PartCategories type)
    {
        List<Part> ret = new List<Part>();

        foreach (Part part in parts)
        {
            if (part.partInfo.category == type)
            {
                ret.Add(part);
            }
        }

        return ret;
    }

    private List<Part> extractPartOfName(List<Part> parts, string name)
    {
        List<Part> ret = new List<Part>();

        foreach (Part part in parts)
        {
            if (part.partInfo.name == name)
            {
                ret.Add(part);
            }
        }

        return ret;
    }

    private PartCategories intToPartCategories(int i)
    {
        switch (i)
        {
            case 0:
                return PartCategories.Control;

            case 1:
                return PartCategories.none;

            case 2:
                return PartCategories.none;

            case 4:
                return PartCategories.Pods;

            case 5:
                return PartCategories.Propulsion;

            case 6: 
                return PartCategories.Structural;

            case 7:
                return PartCategories.Utility;

            default:
                return PartCategories.none;
        }
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

