using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the ChuckStateMachine class/object type which takes states and adds them to it's "states" list. 
public class ChuckStateMachine : MonoBehaviour
{
    private Dictionary<Type, ChuckBaseState> states;    //List of all the finite state machine's states. (Only 3 in this instance).
    public ChuckBaseState currentState; //Stores the value of the current state.
    
    //Returns and sets the values of the current state.
    public ChuckBaseState CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            currentState = value;
        }
    }

    //Adds the states passed in as parameters into the states list.
    public void SetStates(Dictionary<Type, ChuckBaseState> states)  
    {
        this.states = states;
    }

    void Update()
    {

        //If the CurrentState doesn't have a value yet, set it to the first state in the state's list. 
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
        }
        else
        {
            //Set the next state as what the state's StateUpdate function returned. If it returned null, remain in the current state, otherwise swap to the returned state.
            var nextState = CurrentState.StateUpdate();

            if ((nextState != null) && (nextState != currentState.GetType()))
            {
                SwitchToState(nextState);
            }
        }
    }

    //Call the the current state's StateExit function, set the current state to the new state as returned by StateUpdate, and call the new current state's StateEnter function.
    void SwitchToState(Type nextState)
    {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }
}
