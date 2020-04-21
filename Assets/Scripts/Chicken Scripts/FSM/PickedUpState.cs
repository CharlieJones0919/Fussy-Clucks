using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedUpState : ChuckBaseState
{
    //Reference to the object's Chuck script.
    private Chuck thisChuck;

    //Called in the Chuck script to set this state's reference to itself.
    public PickedUpState(Chuck thisChuck)
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
        thisChuck.CheckStats(); //Check the chicken/egg's current stats.

        if (thisChuck.BeenPickedUp()) //If the player's finger is still on the chicken, call PickedUp() which moves it towards the finger's current position in worldspace until they lift their finger.
        {
            thisChuck.PickedUp();
            return null;
        }
        else    //If this state is entered or the object is dropped but the object is currently an egg, swap to EggState. Otherwise, swap back to WanderState.
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