using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chuck : MonoBehaviour
{
    public Rigidbody chuckRB;
    public float chuckSpeed;

    public Vector2 randomDir;
    public float timeToChangeDir;
    public float timePassed;
    public bool atMapLimit;

    private void Awake()
    {
        InitializeStateMachine();

        timePassed = 0;
        atMapLimit = false;

        randomDir = UnityEngine.Random.insideUnitCircle;
    }

    private void InitializeStateMachine()
    {
        Dictionary<Type, ChuckBaseState> states = new Dictionary<Type, ChuckBaseState>();

        states.Add(typeof(WanderState), new WanderState(this));

        GetComponent<ChuckStateMachine>().SetStates(states);
    }

    public void Wandering()
    {
        if (timePassed >= timeToChangeDir)
        {
            randomDir = UnityEngine.Random.insideUnitCircle;
            timePassed = 0.0f;
        }
        else
        {
            if (!atMapLimit)
            {
                chuckRB.AddForce(randomDir.x * chuckSpeed, 0.0f, randomDir.y * chuckSpeed, ForceMode.Force);
                timePassed += Time.deltaTime;
            }
            else
            {
                chuckRB.AddForce(-randomDir.x * chuckSpeed, 0.0f, -randomDir.y * chuckSpeed, ForceMode.Impulse);
                timePassed = timeToChangeDir;
            }
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Map Limit")
        {
            atMapLimit = true;
        }
    }

    void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Map Limit")
        {
            atMapLimit = false;
        }
    }
}
