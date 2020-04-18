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
        Debug.Log("Entered WanderState");
        return null;
    }

    public override Type StateExit()
    {
        Debug.Log("Exited WanderState");
        return null;
    }

    public override Type StateUpdate()
    {
        thisChuck.CheckStats();

        if (thisChuck.BeenPickedUp())
        {
            return typeof(PickedUpState);
        }
        else if (thisChuck.chickyProps.inHutch || thisChuck.chickyProps.inNest || thisChuck.chickyProps.sleepy)
        {
            thisChuck.CheckCoinPickUp();
            return null;
        }
        else if (thisChuck.chickyProps.hungry && thisChuck.SeedAvailable())
        {
            thisChuck.CheckCoinDrop();
            thisChuck.CheckCoinPickUp();

            thisChuck.GoToSeed();
            return null;
        }
        else
        {
            thisChuck.CheckCoinDrop();
            thisChuck.CheckCoinPickUp();

            thisChuck.Wandering();
            return null;
        }
    }
}