using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : ChuckBaseState
{
    //Reference to the object's Chuck script.
    private Chuck thisChuck;

    //Called in the Chuck script to set this state's reference to itself.
    public WanderState(Chuck thisChuck)
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
        thisChuck.CheckStats(); //Check the chicken's current stats.

        if (thisChuck.eggProps.isEgg) //Checked just in case the FSM is entered after being deactivated from this state and reactivated here.
        {
            return typeof(EggState);
        }
        else
        {
            if (thisChuck.BeenPickedUp()) //If the chicken has been tapped, enter the PickedUpState.
            {
                return typeof(PickedUpState);
            }
            else if (thisChuck.chickyProps.inHutch || thisChuck.chickyProps.inNest || (thisChuck.transform.position.y > 2.0f)) //If the chicken is in a nest or hutch  or is still above the ground after being picked up, don't do anything. 
            {
                return null;
            }
            else if (thisChuck.chickyProps.sleepy)  //If the chicken is sleepy, drop coins still but don't go towards seed or wander.
            {
                thisChuck.CheckCoinDrop();
                return null;
            }
            else if ((thisChuck.chickyProps.hunger < (100 - thisChuck.constPropsRef.seedFeedAmount)) && thisChuck.SeedAvailable()) //If the chicken's hunger is less than the amount seed would replenish it, and seed is available, go towards it.
            {
                thisChuck.CheckCoinDrop();
                thisChuck.GoToSeed();
                return null;
            }
            else    //If the chicken isn't picked up, sleepy, or going after food, move in random directions.
            {
                thisChuck.CheckCoinDrop();  
                thisChuck.Wandering();
                return null;
            }
        }
    }
}