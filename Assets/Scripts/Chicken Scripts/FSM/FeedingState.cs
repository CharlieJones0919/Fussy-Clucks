using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingState : ChuckBaseState
{
    private Chuck thisChuck;

    public FeedingState(Chuck thisChuck)
    {
        this.thisChuck = thisChuck;
    }

    public override Type StateEnter()
    {
        Debug.Log("Entered FeedingState");
        return null;
    }

    public override Type StateExit()
    {
        Debug.Log("Exited FeedingState");
        return null;
    }

    public override Type StateUpdate()
    {
        //thisChuck.CheckStats();

        if (thisChuck.BeenPickedUp())
        {
            return typeof(PickedUpState);
            
        }
        else
        {
            return null;
        }
    }
}