using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownState : ChuckBaseState
{
    private Chuck thisChuck;

    public ThrownState(Chuck thisChuck)
    {
        this.thisChuck = thisChuck;
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        if (thisChuck.IsThrown())
        {
            thisChuck.IsThrown();
            return null;
        }
        else if (thisChuck.BeenPickedUp())
        {
            return typeof(PickedUpState);
        }
        else
        {
            return typeof(WanderState);
        }
    }
}