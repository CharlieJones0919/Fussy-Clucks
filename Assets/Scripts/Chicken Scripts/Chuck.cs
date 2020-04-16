using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;  //Specifiying that the "Random" function call should be from Unity.Engine and not System.

public class Chuck : MonoBehaviour
{
    private ChickyPropertiesController propertiesController;
    private LevelLoader levelLoader;
    private ChickyPropertiesController.ConstProps constProps;
    public ChickyPropertiesController.EggProps eggProps;
    public ChickyPropertiesController.ChickyVariableProps chickyProps;

    private GameObject currentModelClone;
    public GameObject chickyModel;
    public GameObject roosirModel;

    public Rigidbody rigidBody; //Chicken's rigidbody.
    public SphereCollider chuckCollider;
    public float currentColliderRadius;

    //Ran before runtime to initialise variables and to initialise the chicken's FSM.
    private void Awake()
    {
        InitializeStateMachine(); //Runs the InitalizeStateMachine() function as defined below.
    }

    //Adds all the behaviour states to the states dictionary and sets these to the state machine.
    private void InitializeStateMachine()
    {
        Dictionary<Type, ChuckBaseState> states = new Dictionary<Type, ChuckBaseState>(); //Dictionary var to hold the states and their types.
        states.Add(typeof(PickedUpState), new PickedUpState(this)); //Adds PickedUpState to states. (Starting State).
        states.Add(typeof(ThrownState), new ThrownState(this)); //Adds ThrownState to states. 
        states.Add(typeof(EggState), new EggState(this)); //Adds EggState to states.
        states.Add(typeof(WanderState), new WanderState(this)); //Adds WanderState to states.
        GetComponent<ChuckStateMachine>().SetStates(states); //Passes the added states into the state machine's setting function.
    }

    public void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); //Gets the rigidbody from the object this script is attached to.  
        chuckCollider = GetComponent<SphereCollider>();
        propertiesController = GameObject.Find("LevelController").GetComponent<ChickyPropertiesController>();
        levelLoader = GameObject.Find("LevelController").GetComponent<LevelLoader>();
        constProps = propertiesController.GetConstProps();
        Initialize();
    }

    public void Initialize()
    {
        chuckCollider.center = constProps.eggColliderCentre;
        chuckCollider.radius = constProps.eggColliderRadius;
        currentColliderRadius = chuckCollider.radius;

        //Egg's Initial Properties
        eggProps.isEgg = true;
        eggProps.timeUntilHatch = 0.0f;
        eggProps.health = 100;
        eggProps.inNest = false;

        //Chicky's Initial Properties
        chickyProps.age = 0;
        chickyProps.temp = 25;
        chickyProps.hunger = 100;

        chickyProps.randomDir = UnityEngine.Random.insideUnitCircle; //Sets the Vector2 its initial 2 random floats between -1 and 1.
        chickyProps.wanderTimer = 0.0f;
        chickyProps.thrownTimer = 0.0f;
        chickyProps.isBeingHeld = false;
    }

    public void SetType(string type, GameObject model)
    {
        if (chickyProps.type != "")
        {
            GameObject oldModelClone = currentModelClone;
            currentModelClone = Instantiate(model, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            currentModelClone.transform.parent = this.transform;
            Destroy(oldModelClone);
        }
        else
        {
            currentModelClone = Instantiate(model, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            currentModelClone.transform.parent = this.transform;
        }
    
        chickyProps.type = type;
    }

    //Function called on update during the WanderState.
    public void Wandering()
    {
        //If the chicken's velocity exceeds its maximum velocity, decrease it.
        if (rigidBody.velocity.magnitude > constProps.maxVel)
        {
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, constProps.maxVel);
        }

        //If the time the chicken has spent wandering since last changing direction is more than or equal to the time the 
        //chicken should spend spend wandering in one direction, set the random direction to a new Vector2 and reset the wander time.
        if (chickyProps.wanderTimer >= constProps.wanderTimeLimit)
        {
            chickyProps.randomDir = UnityEngine.Random.insideUnitCircle;
            chickyProps.wanderTimer = 0.0f;
        }

        //Move the chicken's rigidbody in the direction of the current random direction and increase the wander time passed variable.
        rigidBody.AddForce(chickyProps.randomDir.x * constProps.speed, 0.0f, chickyProps.randomDir.y * constProps.speed, ForceMode.Force);
        chickyProps.wanderTimer += Time.deltaTime;
    }

    //Called when the object's collider touches a different collider.
    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Fence")
        {
            Vector2 pushDir = collider.contacts[0].point - transform.position; //Get position of contact with the fence.
            pushDir = -pushDir.normalized; //Invert and normalise the direction.

            //If the chicken is at the map limit, add an impulse force to it in the opposite direction then change the random direction.
            rigidBody.AddForce(pushDir.x * constProps.fenceKnockback, 0.0f, pushDir.y * constProps.fenceKnockback, ForceMode.Impulse);
            chickyProps.wanderTimer = constProps.wanderTimeLimit;
        }
        else if (eggProps.isEgg && (collider.gameObject.tag == "Nest"))
        {
            transform.position = collider.transform.position;
            eggProps.inNest = true;
        }
    }

    //Called by the PickedUpState's update while BeenPickedUp() returns as true. 
    //Moves the chicken towards the position of the player's finger on the screen.
    public void PickedUp()
    {
        if (eggProps.isEgg)
        {
            IsEggStill();
        }

        rigidBody.velocity = Vector3.zero; //Sets the chicken's velocity back to 0 so the only movement is the force towards the finger.
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Gets the position of the mouse/finger on the screen as a world co-ordinate.
        touchPos.y = constProps.pickUpHeight; //Redefines the y-axis of the touch position as the screen hasn't got a depth.

        rigidBody.AddForce((touchPos - transform.position) * constProps.fingerFollowSpeed); //Adds force to the chicken in the direction of the finger/mouse. 
                                                                                             //(The difference between the touch position and its current position).
    }

    //Called every update while the chicken is in its PickedUpState. 
    //Checks if the mouse button or the player's finger is [still] down on the chicken. A.K.A. Is the chicken being held. 
    public bool BeenPickedUp()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //A ray in the direction of the player's touching position from the screen position.
            touchPosition.y = 0.2f;

            if (((touchPosition.x > (transform.position.x - currentColliderRadius)) && (touchPosition.x < (transform.position.x + currentColliderRadius)))
            && ((touchPosition.y > (transform.position.y - currentColliderRadius)) && (touchPosition.y < (transform.position.y + currentColliderRadius)))
            && ((touchPosition.z > (transform.position.z - currentColliderRadius)) && (touchPosition.z < (transform.position.z + currentColliderRadius))))
            {
                chickyProps.isBeingHeld = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) //If the player stops touching the screen, the chicken isn't being held.
        {
            chickyProps.isBeingHeld = false;
        }

        return chickyProps.isBeingHeld; //Return the result of if the chicken is being held.
    }

    //Called every update while the chicken is in its ThrownState.
    public bool IsThrown()
    {
        //If the timer is less than the time the chicken should spend in its thrown state, increase the timer and return true to stay in this state.
        if (chickyProps.thrownTimer < constProps.thrownTime)
        {
            chickyProps.thrownTimer += Time.deltaTime;
            return true;
        }
        else
        {
            //If the timer is equal to or more than the time the chicken should spend in its thrown state, reset the timer and return false to exit this state.
            chickyProps.thrownTimer = 0;
            return false;
        }
    }

    public bool IsEggStill()
    {
        //If the timer is less than the time the chicken should spend as an egg, increase the timer and return true to stay in this state.
        if (eggProps.timeUntilHatch < constProps.timeToStayEgg)
        {
            eggProps.timeUntilHatch += Time.deltaTime;
            return true;
        }
        else 
        {
            //If the timer is equal to or more than the time the chicken should spend in as an egg, reset the timer and return false to exit this state.
            eggProps.timeUntilHatch = 0;
            return false;
        }
    }

    public void Hatch()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 5.0f, transform.position.z);
        chuckCollider.center = constProps.chickyColliderCentre;
        chuckCollider.radius = constProps.chickyColliderRadius;
        currentColliderRadius = chuckCollider.radius;

        switch (chickyProps.type)
        {
            case ("Chicky Egg"):
                SetType("Chicky", chickyModel);
                break;
            case ("Roosir Egg"):
                SetType("Roosir", roosirModel);
                break;
            default:
                Debug.Log("Unrecognised Chuck Type");
                break;
        }
    }

    //////////public void IncreaseTemperature()
    //////////{
  

    //////////}

    //////////public void DecreaseTemperature()
    //////////{
    //////////    chickyProps.temp -= Time.deltaTime;

    //////////    if (eggProps.isEgg)
    //////////    {
    //////////        if (chickyProps.temp <= constProps.eggColdLimit)
    //////////        {
    //////////            //Do Something

    //////////            if (chickyProps.temp <= constProps.eggFreezeDeathTemp)
    //////////            {
    //////////                //Die
    //////////            }
    //////////        }            
    //////////    }
    //////////}
}