using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggState : ChuckBaseState
{
    private Chuck thisChuck;

    public EggState(Chuck thisChuck)
    {
        this.thisChuck = thisChuck;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered EggState");
        thisChuck.eggProps.isEgg = true;
        thisChuck.rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        return null;
    }

    public override Type StateExit()
    {
        Debug.Log("Exited EggState");
        return null;
    }

    public override Type StateUpdate()
    {
        if (thisChuck.IsEggStill())
        {
            //if (thisChuck.eggProps.inNest)
            //{
            //    thisChuck.IncreaseTemperature();
            //}
            //else
            //{
            //    thisChuck.DecreaseTemperature();
            //}

            if (thisChuck.BeenPickedUp())
            {
                return typeof(PickedUpState);
            }
            else
            {
                return null;
            }
        }
        else
        {
            thisChuck.Hatch();
            thisChuck.eggProps.isEgg = false;
            thisChuck.rigidBody.constraints = RigidbodyConstraints.None;
            return typeof(ThrownState);
        }
    }
}