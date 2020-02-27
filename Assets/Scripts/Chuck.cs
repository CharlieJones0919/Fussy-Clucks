using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chuck : MonoBehaviour
{
    private Rigidbody chuckRB;
    public float chuckSpeed;

    public float timeToChangeDir;
    [SerializeField]
       private Vector2 randomDir;
    [SerializeField]
       private float wanderTimePassed;
    [SerializeField]
       private bool atMapLimit;

    [SerializeField]
        private bool isBeingHeld = false;
    public float thrownTime;
    [SerializeField]
        private float thrownTimer;

    private void Awake()
    {
        InitializeStateMachine();

        chuckRB = GetComponent<Rigidbody>();

        randomDir = UnityEngine.Random.insideUnitCircle;
        wanderTimePassed = 0;
        atMapLimit = false;

        thrownTimer = 0;
    }

    private void InitializeStateMachine()
    {
        Dictionary<Type, ChuckBaseState> states = new Dictionary<Type, ChuckBaseState>();

        states.Add(typeof(WanderState), new WanderState(this));
        states.Add(typeof(PickedUpState), new PickedUpState(this));
        states.Add(typeof(ThrownState), new ThrownState(this));

        GetComponent<ChuckStateMachine>().SetStates(states);
    }

    public void Wandering()
    {
        if (wanderTimePassed >= timeToChangeDir)
        {
            randomDir = UnityEngine.Random.insideUnitCircle;
            wanderTimePassed = 0.0f;
        }
        else
        {
            if (!atMapLimit)
            {
                chuckRB.AddForce(randomDir.x * chuckSpeed, 0.0f, randomDir.y * chuckSpeed, ForceMode.Force);
                wanderTimePassed += Time.deltaTime;
            }
            else
            {
                chuckRB.AddForce(-randomDir.x * chuckSpeed, 0.0f, -randomDir.y * chuckSpeed, ForceMode.Impulse);
                wanderTimePassed = timeToChangeDir;
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

    public void PickedUp()
    {
        chuckRB.velocity = Vector3.zero;
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        touchPos.y = 2.0f;

        chuckRB.AddForce((touchPos - transform.position) * 200.0f);
    }

    public bool BeenPickedUp()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit touching;
            Ray touchPos = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(touchPos, out touching))
            {
                if (touching.collider.gameObject.tag == "Chuck")
                {
                    isBeingHeld = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isBeingHeld = false;
        }

        return isBeingHeld;
    }

    public bool IsThrown()
    {
        if (thrownTimer < thrownTime)
        {
            thrownTimer += Time.deltaTime;
            return true;
        }
        else
        {
            thrownTimer = 0;
            return false;
        }
    }
}