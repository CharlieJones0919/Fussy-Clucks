using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;  //Specifiying that the "Random" function call should be from Unity.Engine and not System.

public class Chuck : MonoBehaviour
{
    private ChickyPropertiesController propertiesController;
    private LevelController levelController;
    public ChickyPropertiesController.ConstProps constProps;
    public ChickyPropertiesController.EggProps eggProps;
    public ChickyPropertiesController.ChickyVariableProps chickyProps;

    private GameObject currentModelClone;
    public GameObject chickyModel;
    public GameObject roosirModel;

    public Rigidbody rigidBody; //Chicken's rigidbody.
    public SphereCollider chuckCollider;
    public float currentColliderRadius;

    public Vector3 seedPosition;
    public GameObject activeCoin;
    public int coinDropAmount;

    //Ran before runtime to initialise variables and to initialise the chicken's FSM.
    private void Awake()
    {
        InitializeStateMachine(); //Runs the InitalizeStateMachine() function as defined below.
    }

    //Adds all the behaviour states to the states dictionary and sets these to the state machine.
    private void InitializeStateMachine()
    {
        Dictionary<Type, ChuckBaseState> states = new Dictionary<Type, ChuckBaseState>(); //Dictionary var to hold the states and their types.
        states.Add(typeof(EggState), new EggState(this)); //Adds EggState to states. (Starting State).
        states.Add(typeof(PickedUpState), new PickedUpState(this)); //Adds PickedUpState to states. 
        states.Add(typeof(WanderState), new WanderState(this)); //Adds WanderState to states.
        GetComponent<ChuckStateMachine>().SetStates(states); //Passes the added states into the state machine's setting function.
    }

    public void OnEnable()
    {
        rigidBody = GetComponent<Rigidbody>(); //Gets the rigidbody from the object this script is attached to.  
        chuckCollider = GetComponent<SphereCollider>();
        propertiesController = GameObject.Find("LevelController").GetComponent<ChickyPropertiesController>();
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        constProps = propertiesController.GetConstProps();
    }

    public void Initialize()
    {
        chuckCollider.center = constProps.eggColliderCentre;
        chuckCollider.radius = constProps.eggColliderRadius;
        currentColliderRadius = chuckCollider.radius;

        //Egg's Initial Properties
        eggProps.isEgg = true;
        eggProps.timeUntilHatch = 0.0f;

        //Chicky's Initial Properties
        chickyProps.name = null;
        chickyProps.temp = 25.0f;
        chickyProps.hunger = 100.0f;
        chickyProps.attention = 100.0f;
        chickyProps.rest = 100.0f;

        chickyProps.inNest = false;
        chickyProps.cold = false;
        chickyProps.hot = false;
        chickyProps.hungry = false;
        chickyProps.lonely = false;
        chickyProps.sleepy = false;

        chickyProps.randomDir = UnityEngine.Random.insideUnitCircle; //Sets the Vector2 its initial 2 random floats between -1 and 1.
        chickyProps.wanderTimer = 0.0f;
        chickyProps.thrownTimer = 0.0f;
        chickyProps.isBeingHeld = false;

        chickyProps.timeSinceCoinDrop = 0.0f;
    }

    public void SetType(string type, GameObject model)
    {
        currentModelClone = Instantiate(model, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        currentModelClone.transform.parent = this.transform;

        chickyProps.type = type;
    }

    //Function called on update during the WanderState.
    public void Wandering()
    {
        //If the time the chicken has spent wandering since last changing direction is more than or equal to the time the 
        //chicken should spend spend wandering in one direction, set the random direction to a new Vector2 and reset the wander time.
        if (chickyProps.wanderTimer >= constProps.wanderTimeLimit)
        {
            chickyProps.randomDir = UnityEngine.Random.insideUnitCircle;
            chickyProps.wanderTimer = 0.0f;
        }

        //Move the chicken's rigidbody in the direction of the current random direction and increase the wander time passed variable.
        rigidBody.AddForce(chickyProps.randomDir.x * constProps.speed, 0.0f, chickyProps.randomDir.y * constProps.speed, ForceMode.Force);

        //If the chicken's velocity exceeds its maximum velocity, clamp it within the maximum velocity.
        if (rigidBody.velocity.magnitude > constProps.maxVel)
        {
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, constProps.maxVel);
        }

        chickyProps.wanderTimer += Time.deltaTime;
    }

    //Called when the object's collider touches a different collider.
    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Fence")
        {
            Vector2 pushDir = collider.contacts[0].point - transform.position; //Get position of contact with the fence.
            pushDir = -pushDir.normalized; //Invert and normalise the direction.

            //If the chicken is at the map limit, add an impulse force to it in the opposite direction then change the random direction.
            rigidBody.AddForce(pushDir.x * constProps.fenceKnockback, 0.0f, pushDir.y * constProps.fenceKnockback, ForceMode.Impulse);
            chickyProps.wanderTimer = constProps.wanderTimeLimit;
        }

        if (collider.gameObject.tag == "Chuck")
        {
            if (chickyProps.attention < 100)
            {
                chickyProps.attention++;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Seed")
        {
            if ((!eggProps.isEgg) && ((chickyProps.hunger < (100 - constProps.seedFeedAmount))))
            {
                levelController.DeactivateObject(collider.gameObject, levelController.levelSeed);

                if ((chickyProps.hunger + constProps.seedFeedAmount) > 100)
                {
                    chickyProps.hunger = 100;
                }
                else
                {
                    chickyProps.hunger += constProps.seedFeedAmount;
                }

                levelController.seedAvailable = false;
            }
        }

        if (collider.gameObject.tag == "InsideHutch")
        {
            if (!eggProps.isEgg)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
                chickyProps.inHutch = true;
            }
        }
        else if (collider.gameObject.tag == "Nest")
        {
            if (!collider.gameObject.GetComponent<NestOccupation>().nestOccupied || collider.gameObject.GetComponent<NestOccupation>().chickyInNest == transform.gameObject)
            {
                rigidBody.velocity = Vector3.zero;
                transform.position = new Vector3(collider.transform.position.x, 0.5f, collider.transform.position.z);
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

                chickyProps.inNest = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    { 
        if (collider.gameObject.tag == "InsideHutch")
        {
            if (!eggProps.isEgg)
            {
                rigidBody.constraints = RigidbodyConstraints.None;
                chickyProps.inHutch = false;
            }
        }
        else if (collider.gameObject.tag == "Nest")
        {
            rigidBody.constraints = RigidbodyConstraints.None;
            chickyProps.inNest = false;
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
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //A vector of the player's touching position from the screen position into the world position.
            touchPosition.y = transform.position.y;

            if (((touchPosition.x > (transform.position.x - currentColliderRadius))) && (touchPosition.x < (transform.position.x + currentColliderRadius))
            && ((touchPosition.y > (transform.position.y - currentColliderRadius)) && (touchPosition.y < (transform.position.y + currentColliderRadius)))
            && ((touchPosition.z > (transform.position.z - currentColliderRadius))) && (touchPosition.z < (transform.position.z + currentColliderRadius)))
            {
                chickyProps.isBeingHeld = true;
                levelController.uiOutput.whichChicky = transform.gameObject;
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

    public void CheckStats()
    {
        //Check Temperature
        if (eggProps.isEgg)
        {
            CheckTemperature(constProps.eggColdLimit, constProps.eggHotLimit, constProps.eggFreezeDeathTemp, constProps.eggOverheatDeathTemp);
        }
        else
        {
            CheckTemperature(constProps.chuckColdLimit, constProps.chuckHotLimit, constProps.chuckFreezeDeathTemp, constProps.chuckOverheatDeathTemp);
            //Check Hunger
            CheckHunger();
            //Check Attention
            CheckAttention();
            //Check Rest
            CheckRest();
        }

        if (levelController.uiOutput.whichChicky == transform.gameObject)
        {
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.tempText, chickyProps.temp);
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.hungryText, chickyProps.hunger);
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.attentionText, chickyProps.attention);
            levelController.uiOutput.SetTextOutput(levelController.uiOutput.sleepText, chickyProps.rest);
            levelController.uiOutput.SetNameOutput(chickyProps.name); 
        }
    }

    private void CheckTemperature(float coldLimit, float heatLimit, float coldDeath, float heatDeath)
    {
        if ((chickyProps.inNest) && (chickyProps.temp < 100.0f))
        {
            chickyProps.temp += Time.deltaTime * 0.5f;
        }
        else if ((!chickyProps.inNest) && (chickyProps.temp > 0.0f))
        {
            if (eggProps.isEgg)
            {
                chickyProps.temp -= Time.deltaTime ;
            }
            else
            {
                chickyProps.temp -= Time.deltaTime * 0.15f;
            }
        }

        if (chickyProps.temp <= coldLimit)
        {
            chickyProps.cold = true;
            chickyProps.hot = false;

            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.tempIcon, levelController.uiOutput.tempCold); }

            if (chickyProps.temp <= coldDeath)
            {
                KillChuck("A chicken died from the cold. (Put it in a nest next time).");
            }
        }
        else if (chickyProps.temp >= heatLimit)
        {
            chickyProps.cold = false;
            chickyProps.hot = true;

            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.tempIcon, levelController.uiOutput.tempHot); }

            if (chickyProps.temp >= heatDeath)
            {
                KillChuck("A chicken died from overheating. (Don't leave your chickens in the nest so long next time).");
            }
        }
        else
        {
            chickyProps.cold = false;
            chickyProps.hot = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.tempIcon, levelController.uiOutput.temp); }
        }
    }

    private void CheckHunger()
    {
        if (chickyProps.hunger > 0.0f)
        {
            chickyProps.hunger -= Time.deltaTime;
        }

        if (chickyProps.hunger < 20.0f)
        {
            chickyProps.hungry = true;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.hungryIcon, levelController.uiOutput.hungerLow); }

            if (chickyProps.hunger <= 0)
            {
                KillChuck("A chicken died of hunger.");
            }
        }
        else
        {
            chickyProps.hungry = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.hungryIcon, levelController.uiOutput.hunger); }
        }
    }

    private void CheckAttention()
    {
        if ((chickyProps.isBeingHeld) && (chickyProps.attention < 100.0f))
        {
            chickyProps.attention += Time.deltaTime * 2;
        }
        else if ((!chickyProps.isBeingHeld) && (chickyProps.attention > 0.0f))
        {
            chickyProps.attention -= Time.deltaTime;
        }

        if (chickyProps.attention < 25.0f)
        {
            chickyProps.lonely = true;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.attentionIcon, levelController.uiOutput.attentionLow); }
        }
        else
        {
            chickyProps.lonely = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.attentionIcon, levelController.uiOutput.attention); }
        }
    }

    private void CheckRest()
    {
        if ((chickyProps.inHutch) && (chickyProps.rest < 100.0f))
        {
            chickyProps.rest += Time.deltaTime;
        }
        else if ((!chickyProps.inHutch) && (chickyProps.rest > 0.0f))
        {
            chickyProps.rest -= Time.deltaTime * 0.5f;
        }

        if (chickyProps.rest < 60.0f)
        {
            chickyProps.sleepy = true;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.sleepIcon, levelController.uiOutput.sleepLow); }
        }
        else
        {
            chickyProps.sleepy = false;
            if (levelController.uiOutput.whichChicky == transform.gameObject) { levelController.uiOutput.SetPropetyIcon(levelController.uiOutput.sleepIcon, levelController.uiOutput.sleep); }
        }
    }

    public void SetSeedAsAvailable(Vector3 newSeedPos)
    {
        seedPosition = newSeedPos;
        levelController.seedAvailable = true;
    }

    public bool SeedAvailable()
    {
        return levelController.seedAvailable;
    }

    public void GoToSeed()
    {
        rigidBody.AddForce((seedPosition - transform.position).normalized * constProps.speed);
        if (rigidBody.velocity.magnitude > constProps.maxVel)
        {
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, constProps.maxVel);
        }
    }

    public void CheckCoinDrop()
    {
        if (chickyProps.timeSinceCoinDrop >= constProps.timeBetweenCoinDrop)
        {
            chickyProps.timeSinceCoinDrop = 0.0f;

            foreach (KeyValuePair<GameObject, bool> coin in levelController.levelCoins)
            {
                if (coin.Value == true)
                {
                    activeCoin = coin.Key;
                    levelController.levelCoins[activeCoin] = false;

                    activeCoin.transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                    activeCoin.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            chickyProps.timeSinceCoinDrop += Time.deltaTime;
        }
    }

    public void CheckCoinPickUp()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //A vector of the player's touching position from the screen position into the world position.
            touchPosition.y = 0.0f;

            foreach (KeyValuePair<GameObject, bool> coin in levelController.levelCoins)
            {
                if (coin.Value == false)
                {
                    activeCoin = coin.Key;

                    if (((touchPosition.x > (activeCoin.transform.position.x - 2.0f)) && (touchPosition.x < (activeCoin.transform.position.x + 2.0f)))
                         && ((touchPosition.z > (activeCoin.transform.position.z - 2.0f)) && (touchPosition.z < (activeCoin.transform.position.z + 2.0f))))
                    {
                        coinDropAmount = constProps.maxCoinDropAmount;

                        if (chickyProps.cold || chickyProps.hot) { coinDropAmount--; }
                        if (chickyProps.hungry) { coinDropAmount--; }
                        if (chickyProps.lonely) { coinDropAmount--; }
                        if (chickyProps.sleepy) { coinDropAmount--; }

                        if (chickyProps.type == "Roosir")
                        {
                            coinDropAmount = coinDropAmount * 2;
                        }

                        levelController.DeactivateObject(activeCoin, levelController.levelCoins);
                        levelController.finances.AddGold(coinDropAmount);
                        levelController.CheckLevelSuccess();
                        break;
                    }
                }
            }
        }
    }

    private void KillChuck(string deathMessage)
    {
        GameObject oldModelClone = currentModelClone;
        Destroy(oldModelClone);

        levelController.DeactivateObject(transform.gameObject, levelController.levelChickens, deathMessage);
    }
}