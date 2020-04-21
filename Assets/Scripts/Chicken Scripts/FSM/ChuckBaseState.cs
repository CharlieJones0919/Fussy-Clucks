using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the structure of the state classes for all states to derive from. (What functions they have).
public abstract class ChuckBaseState
{
    public abstract Type StateUpdate();     //Called periodically continuously until another state is returned.
    public abstract Type StateEnter();      //Called when the state is entered/triggered.
    public abstract Type StateExit();       //Called when the state is exited. (One of the prior functions returns another state).
}