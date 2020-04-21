using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggState : ChuckBaseState
{
    //Reference to the object's Chuck script.
    private Chuck thisChuck;

    //Called in the Chuck script to set this state's reference to itself.
    public EggState(Chuck thisChuck)
    {
        this.thisChuck = thisChuck;
    }

    //When this state is entered (object is activated as an egg), freeze its rotation. The FSM is entered from the PickedUpState so this function would be called on activation.
    public override Type StateEnter()
    {
        thisChuck.rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        return null;
    }

    public override Type StateExit()
    { 
        return null;
    }

    public override Type StateUpdate()
    {
        thisChuck.CheckStats(); //Check the egg's current stats. (Only checks temperature).

        if (thisChuck.IsEggStill()) //This IsEggStill() function call serves as both a boolean check, and increments the timer denoting how long it has been since the object was activated as an egg.
        {
            if (thisChuck.BeenPickedUp())   //If the chicken has been tapped, enter the PickedUpState.
            {
                return typeof(PickedUpState);
            }
            else
            {
                return null;
            }
        }
        else    //If the egg timer has gotten to its required value, call the Hatch() function to change the object's model, set isEgg to false, unfreeze the object's rotation, and enter the WanderState.
        {
            thisChuck.Hatch();
            thisChuck.eggProps.isEgg = false;
            thisChuck.rigidBody.constraints = RigidbodyConstraints.None;
            return typeof(WanderState);
        }
    }
}