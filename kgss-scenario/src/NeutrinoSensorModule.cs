//Author: Richard Bunt

using System;
using System.Collections.Generic;
using System.Text;
using KSP.IO;

class NeutrinoSensorModule : PartModule
{
    private CelestialBody sun, jool = null;

    [KSPField(guiActive=true, guiName="Value")]
    string reading = "";

#if (DEBUG)
    [KSPField(guiActive = true, guiName = "LOS")]
    string lineOfSight = "";
#endif

    public float powerConsumption = 0.02f;
    Random random = null;
    float timeElapsed = 0;

    //Neutrino function parameters
    PluginConfiguration config = PluginConfiguration.CreateForType<NeutrinoSensorModule>();

    Vector3d l, d, s ,sm, jm, j, b, c, sd, jd;

    ProbabilityEventGenerator sunSolar, joolSolar;
    NeutrinosScenario ns = null;

    public override void OnStart(PartModule.StartState state)
    {
        ns = getNeutrinosScenarioModule();
        
        if (ns != null)
        {
            sun = getBodyByString("Sun");
            jool = getBodyByString("Jool");
            readFunctionParameters();
            random = new Random(System.DateTime.Now.Second);
            sunSolar = new SolarFlareGenerator(new List<int> { (int)s.x, (int)s.y, (int)s.z }, random, sd.x, sd.y);
            joolSolar = new SolarFlareGenerator(new List<int> { (int)j.x, (int)j.y, (int)j.z }, random, jd.x, jd.y);
        }

        base.OnStart(state);
    }

    public override void  OnUpdate()
    {
        if (ns != null)
        {

            if (timeElapsed > c.x)
            {
                timeElapsed = 0;

                if (getDeployed())
                {
                    //Power code from olex
                    float requiredPower = powerConsumption * TimeWarp.deltaTime;
                    float availPower = part.RequestResource("ElectricCharge", requiredPower);

                    if (availPower < requiredPower)
                    {
                        reading = "Not enough power";
                    }
                    else
                    {
                        reading = neutrinoFunction();
                    }
                }
                else
                {
                    reading = "Dish not deployed";
                }
            }
            else
            {
                timeElapsed += TimeWarp.deltaTime;
            }
        }

        base.OnUpdate();
    }

    public string getReadingText()
    {
        return reading;
    }

    private void readFunctionParameters()
    {
        config.load();

        l = config.GetValue<Vector3d>("l");
        d = config.GetValue<Vector3d>("d");
        s = config.GetValue<Vector3d>("s");
        j = config.GetValue<Vector3d>("j");
        b = config.GetValue<Vector3d>("b");
        c = config.GetValue<Vector3d>("c");
        sm = config.GetValue<Vector3d>("sm");
        jm = config.GetValue<Vector3d>("jm");
        sd = config.GetValue<Vector3d>("sd");
        jd = config.GetValue<Vector3d>("jd");
    }

    private bool getDeployed()
    {
        foreach (var module in part.Modules)
        {
            if (module.GetType().Equals(typeof(ModuleAnimateGeneric)))
            {
                ModuleAnimateGeneric anim = (ModuleAnimateGeneric)module;

                if (anim.Progress == 1)
                {
                    return true;
                }
            }
        }

        return false ;
    }

    private string neutrinoFunction()
    {
        double neutrinos = 0;
        bool sunLineOfSight = isLineOfSight(sun.position, vessel.GetWorldPos3D()), 
             joolLineOfSight = isLineOfSight(jool.position, vessel.GetWorldPos3D());

#if (DEBUG)
        lineOfSight = sunLineOfSight.ToString();
#endif

        neutrinos =
            ((sunLineOfSight ? l.x : l.y) * (Math.Pow(Vector3d.Distance(sun.position, vessel.GetWorldPos3D()), d.x) 
                * sourceEventToContribution(sunSolar, sm) * d.z))  +
            ((joolLineOfSight ? l.x : l.y) * (Math.Pow(Vector3d.Distance(jool.position, vessel.GetWorldPos3D()), d.x) 
                * sourceEventToContribution(joolSolar, jm) * d.z))  +
            backgroundContribution() +
            b.z;


        string formatted = "";

        if (Math.Abs(neutrinos) > c.z)
        {
            formatted = neutrinos.ToString("E" + ((int)c.y).ToString());
        }
        else
        {
            formatted = Math.Round(neutrinos, (int)c.y).ToString();
        }

        return formatted + " neutrinos / s";
    }

    private double backgroundContribution()
    {
        return b.x + (random.NextDouble() * (b.y - b.x));
    }

    private double sourceEventToContribution(ProbabilityEventGenerator p, Vector3d m)
    {
        int e = p.generateEvent();

        switch(e)
        {
            case 0:
                return m.x; 
            case 1:
                return m.y;
            case 2:
                return m.z;
        }

        return 1;
    }

    //Line of sight code from JDP
    public static bool isLineOfSight(Vector3d a, Vector3d b)
    {
        foreach (CelestialBody referenceBody in FlightGlobals.Bodies)
        {
            Vector3d bodyFromA = referenceBody.position - a;
            Vector3d bFromA = b - a;
            if (Vector3d.Dot(bodyFromA, bFromA) > 0)
            {
                Vector3d bFromAnorm = bFromA.normalized;
                if (Vector3d.Dot(bodyFromA, bFromAnorm) < bFromA.magnitude)
                { // check lateral offset from line between b and a
                    Vector3d lateralOffset = bodyFromA - Vector3d.Dot(bodyFromA, bFromAnorm) * bFromAnorm;
                    if (lateralOffset.magnitude < (referenceBody.Radius - 5))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private CelestialBody getBodyByString(string body)
    {
        foreach (CelestialBody referenceBody in FlightGlobals.Bodies)
        {
            if (referenceBody.GetName().Equals(body))
            {
                return referenceBody;
            }
        }

        return null;
    }

    private NeutrinosScenario getNeutrinosScenarioModule()
    {
        foreach (ScenarioModule s in ScenarioRunner.GetLoadedModules())
        {
            if (s.GetType().Equals(typeof(NeutrinosScenario)))
            {
                return (NeutrinosScenario)s;
            }
        }

        return null;
    }
}

