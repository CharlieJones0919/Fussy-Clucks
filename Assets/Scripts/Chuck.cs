using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;  //Specifiying that the "Random" function call should be from Unity.Engine and not System.

public class Chuck : MonoBehaviour
{
    private Rigidbody chuckRB; //Chicken's rigidbody.
    public float chuckSpeed; //Wandering speed.
    [SerializeField]
        private Vector3 maxChuckVel; //Maximum velocity.

    public float timeToChangeDir; //Number of seconds before chicken changes direction when wandering.
    [SerializeField]
       private Vector2 randomDir; //The random direction on the X and Z axis that the chicken wanders.
    [SerializeField]
       private float wanderTimePassed; //Amount of time the chicken has spent wandering since the last direction change.
    [SerializeField]
       private float fenceKnockback; //Force the chicken is knocked back from the fence.

    [SerializeField]
        private bool isBeingHeld = false; //Has chicken been clicked/touched. (Global so it can remain true until touch ends).
    public float thrownTime; //Seconds which should pass after the player unclicks or stops touching the chicken. 
    [SerializeField]
        private float thrownTimer; //How long has passed in seconds since the player stopped touching the chicken.
    [SerializeField]
        private float pickUpHeight; //Position of the chicken on the Y-axis when picked up.
    [SerializeField]
        private float fingerFollowSpeed; //Speed at which the chicken is moved by force towards the touch position.

    //Ran before runtime to initialise variables and to initialise the chicken's FSM.
    private void Awake()
    {
        InitializeStateMachine(); //Runs the InitalizeStateMachine() function as defined below.
    }

    //Adds all the behaviour states to the states dictionary and sets these to the state machine.
    private void InitializeStateMachine()
    {
        Dictionary<Type, ChuckBaseState> states = new Dictionary<Type, ChuckBaseState>(); //Dictionary var to hold the states and their types.

        states.Add(typeof(WanderState), new WanderState(this)); //Adds WanderState to states.
        states.Add(typeof(PickedUpState), new PickedUpState(this)); //Adds PickedUpState to states.
        states.Add(typeof(ThrownState), new ThrownState(this)); //Adds ThrownState to states.

        GetComponent<ChuckStateMachine>().SetStates(states); //Passes the added states into the state machine's setting function.
    }

    //Ran when the object this script is attached to is first activated.
    public void Start()
    {
        chuckRB = GetComponent<Rigidbody>(); //Gets the rigidbody from the object this script is attached to.
        maxChuckVel = new Vector3(chuckSpeed, 2.0f, chuckSpeed);

        randomDir = UnityEngine.Random.insideUnitCircle; //Sets the Vector2 its initial 2 random floats between -1 and 1.
        //Following just initialise the chicken's variables.
        wanderTimePassed = 0;
        fenceKnockback = 5.0f;

        thrownTimer = 0;
        pickUpHeight = 2.0f;
        fingerFollowSpeed = 200.0f;
    }

    //Function called on update during the WanderState.
    public void Wandering()
    {
        //If the chicken's velocity exceeds its dimension's maximum velocity, decrease it.
        if (chuckRB.velocity.x >= maxChuckVel.x)
        {
            chuckRB.velocity.Set(chuckRB.velocity.x - (chuckSpeed * Time.deltaTime), chuckRB.velocity.y, chuckRB.velocity.z);
        }
        if (chuckRB.velocity.y >= maxChuckVel.y)
        {
            chuckRB.velocity.Set(chuckRB.velocity.x, chuckRB.velocity.y - (chuckSpeed * Time.deltaTime), chuckRB.velocity.z);
        }
        if (chuckRB.velocity.z >= maxChuckVel.z)
        {
            chuckRB.velocity.Set(chuckRB.velocity.x, chuckRB.velocity.y, chuckRB.velocity.z - (chuckSpeed * Time.deltaTime));
        }

        //If the time the chicken has spent wandering since last changing direction is more than or equal to the time the 
        //chicken should spend spend wandering in one direction, set the random direction to a new Vector2 and reset the wander time.
        if (wanderTimePassed >= timeToChangeDir)
        {
            randomDir = UnityEngine.Random.insideUnitCircle;
            wanderTimePassed = 0.0f;
        }

        //Move the chicken's rigidbody in the direction of the current random direction and increase the wander time passed variable.
        chuckRB.AddForce(randomDir.x * chuckSpeed, 0.0f, randomDir.y * chuckSpeed, ForceMode.Force);
        wanderTimePassed += Time.deltaTime;
    }

    //Called when the object's collider touches a different collider.
    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Fence")
        {
            Vector2 pushDir = collider.contacts[0].point - transform.position; //Get position of contact with the fence.
            pushDir = -pushDir.normalized; //Invert and normalise the direction.

            //If the chicken is at the map limit, add an impulse force to it in the opposite direction then change the random direction.
            chuckRB.AddForce(pushDir.x * fenceKnockback, 0.0f, pushDir.y * fenceKnockback, ForceMode.Impulse);
            wanderTimePassed = timeToChangeDir;
        }
    }

    //Called by the PickedUpState's update while BeenPickedUp() returns as true. 
    //Moves the chicken towards the position of the player's finger on the screen.
    public void PickedUp()
    {
        chuckRB.velocity = Vector3.zero; //Sets the chicken's velocity back to 0 so the only movement is the force towards the finger.
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Gets the position of the mouse/finger on the screen as a world co-ordinate.
        touchPos.y = pickUpHeight; //Redefines the y-axis of the touch position as the screen hasn't got a depth.

        chuckRB.AddForce((touchPos - transform.position) * fingerFollowSpeed); //Adds force to the chicken in the direction of the finger/mouse. 
                                                                               //(The difference between the touch position and its current position).
    }

    //Called every update while the chicken is in its PickedUpState. 
    //Checks if the mouse button or the player's finger is [still] down on the chicken. A.K.A. Is the chicken being held. 
    public bool BeenPickedUp()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            RaycastHit touching; //What the ray has collided with.
            Ray touchPos = Camera.main.ScreenPointToRay(Input.mousePosition); //A ray in the direction of the player's touching position from the screen position.

            if (Physics.Raycast(touchPos, out touching)) //If the ray has collided with something...
            {
                if (touching.collider.gameObject.tag == "Chuck") //If what the ray collided with had a tag of "Chuck" (was a chicken), isBeingHeld is true.
                {
                    isBeingHeld = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) //If the player stops touching the screen, the chicken isn't being held.
        {
            isBeingHeld = false;
        }

        return isBeingHeld; //Return the result of if the chicken is being held.
    }

    //Called every update while the chicken is in its ThrownState.
    public bool IsThrown()
    {
        //If the timer is less than the time the chicken should spend in its thrown state, increase the timer and return true to stay in this state.
        if (thrownTimer < thrownTime)
        {
            thrownTimer += Time.deltaTime;
            return true;
        }
        else
        {
            //If the timer is equal to or more than the time the chicken should spend in its thrown state, reset the timer and return false to exit this state.
            thrownTimer = 0;
            return false;
        }
    }
}