using System;
using System.Collections.Generic;
using System.Text;


class NeutrinoSensorModule : PartModule
{
    [KSPField(guiActive=true, guiName="Value")]
    string  reading = "";

    public float powerConsumption = 0.05f;

    public override void OnUpdate()
    {
        if (getDeployed())
        {
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

    private bool getDeployed()
    {
        foreach (var module in part.Modules)
        {
            if (module.GetType().Equals(typeof(ModuleAnimateGeneric)))
            {
                ModuleAnimateGeneric anim = (ModuleAnimateGeneric)module;

                if (anim.status.Equals("Fixed"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private string neutrinoFunction()
    {
        int neutrinos = 0;

        return neutrinos.ToString() + " neutrinos / s";
    }
}

