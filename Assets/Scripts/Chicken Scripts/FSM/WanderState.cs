using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : ChuckBaseState
{
    private Chuck thisChuck;

    public WanderState(Chuck thisChuck)
    {
        this.thisChuck = thisChuck;
    }

    public override Type StateEnter()
    {
        if (thisChuck.HasJustBeenCreated())
        {
            return typeof(ThrownState);
        }
        else
        {
            return null;
        }
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (!thisChuck.BeenPickedUp())
        {
            thisChuck.Wandering();
            return null;
        }
        else
        {
            return typeof(PickedUpState);
        }
    }
}