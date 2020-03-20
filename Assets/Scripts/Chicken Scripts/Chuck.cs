using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;  //Specifiying that the "Random" function call should be from Unity.Engine and not System.

public class Chuck : MonoBehaviour
{
    private GameObject chickyControllerObject;
    private bool justActivated = false;

    public struct UniqueChickenProps
    {
         public string name;
         public string breed;
         public uint age;
         public uint temp;
         public uint hunger;

         public Vector2 randomDir;       //The random direction on the X and Z axis that the chicken wanders.
         public float wanderTimer;  //Amount of time the chicken has spent wandering since the last direction change.
         public float thrownTimer;              //How long has passed in seconds since the player stopped touching the chicken.
         public bool isBeingHeld;        //Has chicken been clicked/touched. (Global so it can remain true until touch ends).
    };

    private ChickyPropertiesController.BaseProperties chickyConst;
    private UniqueChickenProps chickyVar;

    private GameObject chickyModel;
    private Rigidbody rigidBody;      //Chicken's rigidbody.

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
        rigidBody = GetComponent<Rigidbody>(); //Gets the rigidbody from the object this script is attached to.  

        Initialise();
        chickyControllerObject = GameObject.Find("ChickyVariablesController");
        chickyConst = chickyControllerObject.GetComponent<ChickyPropertiesController>().GetBaseProperties();
    }

    public void SetChickyParams(string name, string breed)
    {
        chickyVar.name = name;
        chickyVar.age = 0;
        chickyVar.temp = 20;
        chickyVar.hunger = 100;
    }

    public void Initialise()
    {
        chickyVar.randomDir = UnityEngine.Random.insideUnitCircle; //Sets the Vector2 its initial 2 random floats between -1 and 1.
        chickyVar.wanderTimer = 0;
        chickyVar.thrownTimer = 0;
        chickyVar.isBeingHeld = false;
    }

    public void SetBreed(string breed, GameObject model)
    {
        chickyVar.breed = breed;
        chickyModel = Instantiate<GameObject>(model, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        chickyModel.transform.parent = this.transform;
    }

    //Function called on update during the WanderState.
    public void Wandering()
    {
        //If the chicken's velocity exceeds its dimension's maximum velocity, decrease it.
        if (rigidBody.velocity.magnitude > chickyConst.maxVel)
        {
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, chickyConst.maxVel);
        }

        //If the time the chicken has spent wandering since last changing direction is more than or equal to the time the 
        //chicken should spend spend wandering in one direction, set the random direction to a new Vector2 and reset the wander time.
        if (chickyVar.wanderTimer >= chickyConst.wanderTimeLimit)
        {
            chickyVar.randomDir = UnityEngine.Random.insideUnitCircle;
            chickyVar.wanderTimer = 0.0f;
        }

        //Move the chicken's rigidbody in the direction of the current random direction and increase the wander time passed variable.
        rigidBody.AddForce(chickyVar.randomDir.x * chickyConst.speed, 0.0f, chickyVar.randomDir.y * chickyConst.speed, ForceMode.Force);
        chickyVar.wanderTimer += Time.deltaTime;
    }

    //Called when the object's collider touches a different collider.
    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Fence")
        {
            Vector2 pushDir = collider.contacts[0].point - transform.position; //Get position of contact with the fence.
            pushDir = -pushDir.normalized; //Invert and normalise the direction.

            //If the chicken is at the map limit, add an impulse force to it in the opposite direction then change the random direction.
            rigidBody.AddForce(pushDir.x * chickyConst.fenceKnockback, 0.0f, pushDir.y * chickyConst.fenceKnockback, ForceMode.Impulse);
            chickyVar.wanderTimer = chickyConst.wanderTimeLimit;
        }
    }

    //Called by the PickedUpState's update while BeenPickedUp() returns as true. 
    //Moves the chicken towards the position of the player's finger on the screen.
    public void PickedUp()
    {
        rigidBody.velocity = Vector3.zero; //Sets the chicken's velocity back to 0 so the only movement is the force towards the finger.
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Gets the position of the mouse/finger on the screen as a world co-ordinate.
        touchPos.y = chickyConst.pickUpHeight; //Redefines the y-axis of the touch position as the screen hasn't got a depth.

        rigidBody.AddForce((touchPos - transform.position) * chickyConst.fingerFollowSpeed); //Adds force to the chicken in the direction of the finger/mouse. 
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
                    Debug.Log("AAAA");
                    chickyVar.isBeingHeld = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) //If the player stops touching the screen, the chicken isn't being held.
        {
            chickyVar.isBeingHeld = false;
        }

        return chickyVar.isBeingHeld; //Return the result of if the chicken is being held.
    }

    //Called every update while the chicken is in its ThrownState.
    public bool IsThrown()
    {
        //If the timer is less than the time the chicken should spend in its thrown state, increase the timer and return true to stay in this state.
        if (chickyVar.thrownTimer < chickyConst.thrownTime)
        {
            chickyVar.thrownTimer += Time.deltaTime;
            return true;
        }
        else
        {
            //If the timer is equal to or more than the time the chicken should spend in its thrown state, reset the timer and return false to exit this state.
            chickyVar.thrownTimer = 0;
            return false;
        }
    }
}