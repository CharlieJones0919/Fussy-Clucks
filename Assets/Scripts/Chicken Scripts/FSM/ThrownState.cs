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
        Debug.Log("Entered ThrownState");
        return null;
    }

    public override Type StateExit()
    {
        Debug.Log("Exited ThrownState");
        return null;
    }

    public override Type StateUpdate()
    {
        //thisChuck.CheckStats();

        if (thisChuck.IsThrown() && !thisChuck.BeenPickedUp())
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
            if (thisChuck.eggProps.isEgg)
            {
                return typeof(EggState);
            }
            else
            {
                return typeof(WanderState);
            }
        }
    }
}