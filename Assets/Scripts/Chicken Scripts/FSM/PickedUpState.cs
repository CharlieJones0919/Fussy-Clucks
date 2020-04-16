using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedUpState : ChuckBaseState
{
    private Chuck thisChuck;

    public PickedUpState(Chuck thisChuck)
    {
        this.thisChuck = thisChuck;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered PickedUpState");
        return null;
    }

    public override Type StateExit()
    {
        Debug.Log("Exited PickedUpState");
        return null;
    }

    public override Type StateUpdate()
    {
        if (thisChuck.BeenPickedUp() == true)
        {
            thisChuck.PickedUp();
            return null;
        }
        else if (thisChuck.IsEggStill())
        {
            return typeof(EggState);
        }
        else
        {
            return typeof(ThrownState);
        }
    }
}